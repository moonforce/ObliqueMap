using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using Crosstales.FB;
using System.IO;
using System.Linq;
using System.Drawing;
using System;
using System.Xml;
using Point = OpenCVForUnity.CoreModule.Point;
using System.Text;
using ObjLoaderLY;
using TMPro;
using AsImpL;

public class ProjectCtrl : Singleton<ProjectCtrl>
{
    public string CompleteUvCommentLine { get; } = "# This Obj File Has Complete UVs";
    public string EmptyUv { get; } = "vt 0 0";
    const int DoubleSignificantDigits = 17;
    const int m_thumb_width = 240;

    protected ProjectCtrl() { }

    public TextMeshProUGUI m_ProjectName;

    public Transform ModelContainer;

    public float ModelDeltaX { get; set; }
    public float ModelDeltaY { get; set; }

    public TreeView Tree { get; set; }
    public Dictionary<string, CameraHandler> CameraHandlers { get; set; } = new Dictionary<string, CameraHandler>();
    public TreeNode<TreeViewItem> ObliqueImagesTreeNode { get; set; } = new TreeNode<TreeViewItem>(new TreeViewItem("航拍影像"));
    public TreeNode<TreeViewItem> ModelsTreeNode { get; set; } = new TreeNode<TreeViewItem>(new TreeViewItem("三维模型"));
    public TreeNode<TreeViewItem> SceneriesTreeNode { get; set; } = new TreeNode<TreeViewItem>(new TreeViewItem("地景模型"));

    public DataExchange<string> ProjectPath = new DataExchange<string>();

    void Start()
    {
        Tree = transform.GetComponentInChildren<TreeView>();
        Tree.Init();
        SetTreeNodes();
        ProjectPath.OnDataChanged += SetProjectName;
    }

    void SetProjectName(string projectName)
    {
        m_ProjectName.text = projectName;
    }

    void testData()
    {
        //AddObliqueImages(@"E:\倾斜贴图测试数据\jpg");
        //AddModels(@"E:\倾斜贴图测试数据\obj");
        //ParseSmart3DXml(@"E:\倾斜贴图测试数据\A7 -export.xml");
    }

    void SetTreeNodes()
    {
        var nodes = new ObservableList<TreeNode<TreeViewItem>>();
        nodes.Add(ObliqueImagesTreeNode);
        nodes.Add(ModelsTreeNode);
        nodes.Add(SceneriesTreeNode);
        ObliqueImagesTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        ModelsTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        SceneriesTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        Tree.Nodes = nodes;
    }

    private void ClearProject()
    {
        ClearCameraHandlers();
        ClearObliqueImages();
        ClearModels();
        ClearSceneries();
        if (ProjectStage.Instance.FaceChosed || ProjectStage.Instance.FaceEditting)
        {
            MeshAnaliser.Instance.ResetChoice();
        }
    }

    public void ClearCameraHandlers()
    {
        CameraHandlers.Clear();
    }

    public void ClearObliqueImages()
    {
        ObliqueImagesTreeNode.IsExpanded = false;
        ObliqueImagesTreeNode.Nodes.Clear();
    }

    public void ClearWhenDeleteObliqueImage()
    {
        if (ProjectStage.Instance.FaceChosed || ProjectStage.Instance.FaceEditting)
        {
            MeshAnaliser.Instance.ResetChoice();
            //OrbitCamera.Instance.ReplaceModel();
        }
        TextureHandler.Instance.ResetContent();
    }

    public void ClearModels() //隐藏了Grid
    {
        ModelsTreeNode.IsExpanded = false;
        ModelsTreeNode.Nodes.Clear();
        OrbitCamera.Instance.ResetGrid();
        for (int i = 0; i < ModelContainer.childCount; ++i)
        {
            DestroyGameObject(ModelContainer.GetChild(i).gameObject);
        }
    }

    public void ClearWhenDeleteModel()
    {
        OrbitCamera.Instance.ResetGrid();
        MeshAnaliser.Instance.ResetChoice();
    }

    public void ClearSceneries()
    {
        SceneriesTreeNode.IsExpanded = false;
        SceneriesTreeNode.Nodes.Clear();
        for (int i = 0; i < ObjLoadManger.Instance.transform.childCount; ++i)
        {
            DestroyGameObject(ObjLoadManger.Instance.transform.GetChild(i).gameObject);
        }
    }

    public void DestroyGameObject(GameObject go)
    {
        foreach (var mr in go.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (var mat in mr.materials)
            {
                Destroy(mat.mainTexture);
            }
        }
        Destroy(go);
        Resources.UnloadUnusedAssets();
    }

    public void CreateProjectBtnClick()
    {
        if (!string.IsNullOrEmpty(ProjectPath.DataValue) && ProjectPath.DataValue.EndsWith("*"))
        {
            MessageBoxCtrl.Instance.Show("是否保存当前工程？");
            MessageBoxCtrl.Instance.EnsureHandle += EnsureCreateSave;
            MessageBoxCtrl.Instance.CancelHandle += CancelCreateSave;
        }
        else
        {
            ClearProject();
            CreateProject();
        }
    }

    private void CreateProject()
    {
        ProjectPath.DataValue = FileBrowser.SaveFile("新建工程", null, "NewProject", "omp");
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("创建工程失败，未选择路径");
        }
    }

    public void EnsureCreateSave()
    {
        SaveProject();
        ClearProject();
        CreateProject();
        MessageBoxCtrl.Instance.EnsureHandle -= EnsureCreateSave;
        MessageBoxCtrl.Instance.CancelHandle -= CancelCreateSave;
    }

    public void CancelCreateSave()
    {
        ClearProject();
        CreateProject();
        MessageBoxCtrl.Instance.EnsureHandle -= EnsureCreateSave;
        MessageBoxCtrl.Instance.CancelHandle -= CancelCreateSave;
    }

    public void OpenProjectBtnClick()
    {
        if (!string.IsNullOrEmpty(ProjectPath.DataValue) && ProjectPath.DataValue.EndsWith("*"))
        {
            MessageBoxCtrl.Instance.Show("是否保存当前工程？");
            MessageBoxCtrl.Instance.EnsureHandle += EnsureOpenSave;
            MessageBoxCtrl.Instance.CancelHandle += CancelOpenSave;
        }
        else
        {
            ClearProject();
            OpenProject();
        }
    }

    private void OpenProject()
    {
        ProjectPath.DataValue = FileBrowser.OpenSingleFile("打开工程", null, "omp");
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("打开工程失败，未选择路径");
            return;
        }
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create(ProjectPath.DataValue, set));
        XmlNode projectNode = xml.SelectSingleNode("Project");
        SettingsPanelCtrl.Instance.SetDeltaX(projectNode.Attributes["DeltaX"].Value);
        SettingsPanelCtrl.Instance.SetDeltaY(projectNode.Attributes["DeltaY"].Value);
        SettingsPanelCtrl.Instance.SetPointRadius(projectNode.Attributes["PointRadius"].Value);
        SettingsPanelCtrl.Instance.SetLineWidth(projectNode.Attributes["LineWidth"].Value);
        XmlNodeList ObliqueImageNodes = xml.SelectSingleNode("//ObliqueImages").SelectNodes("ObliqueImage");
        List<FileInfo> imageFileInfos = new List<FileInfo>();
        foreach (XmlNode ObliqueImageNode in ObliqueImageNodes)
        {
            if (File.Exists(ObliqueImageNode.InnerText))
                imageFileInfos.Add(new FileInfo(ObliqueImageNode.InnerText));
        }
        if (imageFileInfos.Count > 0)
        {
            StartCoroutine(AddObliqueImages(imageFileInfos));
        }
        XmlNodeList Models = xml.SelectSingleNode("//Models").SelectNodes("Model");
        List<FileInfo> modelFileInfos = new List<FileInfo>();
        foreach (XmlNode Model in Models)
        {
            if (File.Exists(Model.InnerText))
                modelFileInfos.Add(new FileInfo(Model.InnerText));
        }
        if (modelFileInfos.Count > 0)
        {
            StartCoroutine(AddModels(modelFileInfos, 1f));
        }
        XmlNodeList Sceneries = xml.SelectSingleNode("//Sceneries").SelectNodes("Scenery");
        List<FileInfo> sceneryFileInfos = new List<FileInfo>();
        foreach (XmlNode Scenery in Sceneries)
        {
            if (File.Exists(Scenery.InnerText))
                sceneryFileInfos.Add(new FileInfo(Scenery.InnerText));
        }
        if (sceneryFileInfos.Count > 0)
        {
            StartCoroutine(AddSceneries(sceneryFileInfos, 2f));
        }
        ParsePhotogroups(xml, false);
    }

    public void EnsureOpenSave()
    {
        SaveProject();
        ClearProject();
        OpenProject();
        MessageBoxCtrl.Instance.EnsureHandle -= EnsureOpenSave;
        MessageBoxCtrl.Instance.CancelHandle -= CancelOpenSave;
    }

    public void CancelOpenSave()
    {
        ClearProject();
        OpenProject();
        MessageBoxCtrl.Instance.EnsureHandle -= EnsureOpenSave;
        MessageBoxCtrl.Instance.CancelHandle -= CancelOpenSave;
    }

    public void CloseProjectBtnClick()
    {
        if (!string.IsNullOrEmpty(ProjectPath.DataValue) && ProjectPath.DataValue.EndsWith("*"))
        {
            MessageBoxCtrl.Instance.Show("是否保存当前工程？");
            MessageBoxCtrl.Instance.EnsureHandle += EnsureCloseSave;
            MessageBoxCtrl.Instance.CancelHandle += CancelCloseSave;
            ProjectPath.DataValue = string.Empty;
        }
        else
        {
            ClearProject();
            ProjectPath.DataValue = string.Empty;
        }
    }

    public void EnsureCloseSave()
    {
        SaveProject();
        ClearProject();
        MessageBoxCtrl.Instance.EnsureHandle -= EnsureCloseSave;
        MessageBoxCtrl.Instance.CancelHandle -= CancelCloseSave;
    }

    public void CancelCloseSave()
    {
        ClearProject();
        MessageBoxCtrl.Instance.EnsureHandle -= EnsureCloseSave;
        MessageBoxCtrl.Instance.CancelHandle -= CancelCloseSave;
    }

    public void SaveProjectBtnClick()
    {
        SaveProject();
    }

    public void SaveAsProjectBtnClick()
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("另存工程失败，还未创建或打开");
            return;
        }
        ProjectPath.DataValue = FileBrowser.SaveFile("工程另存为……", null, "NewProjectAlias", "omp");
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("另存工程失败，未选择路径");
            return;
        }
        SaveProject(true);
    }

    private void SaveProject(bool saveAs = false)
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("保存工程失败，还未创建或打开");
            return;
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));
        XmlElement projectNode = xmlDoc.CreateElement("Project");
        projectNode.SetAttribute("DeltaX", SettingsPanelCtrl.Instance.DeltaX.ToString());
        projectNode.SetAttribute("DeltaY", SettingsPanelCtrl.Instance.DeltaY.ToString());
        projectNode.SetAttribute("LineWidth", SettingsPanelCtrl.Instance.LineWidth.ToString());
        projectNode.SetAttribute("PointRadius", SettingsPanelCtrl.Instance.PointRadius.ToString());
        xmlDoc.AppendChild(projectNode);
        AddRootLeaf(xmlDoc, "ObliqueImages", "ObliqueImage", GetChildrenFromRootNode(ObliqueImagesTreeNode));
        AddRootLeaf(xmlDoc, "Models", "Model", GetChildrenFromRootNode(ModelsTreeNode));
        AddRootLeaf(xmlDoc, "Sceneries", "Scenery", GetChildrenFromRootNode(SceneriesTreeNode));
        var photogroupsNode = CreateNode(xmlDoc, xmlDoc.SelectSingleNode("Project"), "Photogroups");
        foreach (var cameraHandler in CameraHandlers.Values)
        {
            var photogroupNode = CreateNode(xmlDoc, photogroupsNode, "Photogroup");
            CreateNode(xmlDoc, photogroupNode, "Name", cameraHandler.Name);
            var imageDimensionsNode = CreateNode(xmlDoc, photogroupNode, "ImageDimensions");
            CreateNode(xmlDoc, photogroupNode, "FocalLength", Utills.DoubleToStringSignificantDigits(cameraHandler.FocalLength, DoubleSignificantDigits));
            var principalPointNode = CreateNode(xmlDoc, photogroupNode, "PrincipalPoint");
            var distortionNode = CreateNode(xmlDoc, photogroupNode, "Distortion");
            CreateNode(xmlDoc, imageDimensionsNode, "Width", cameraHandler.Width.ToString());
            CreateNode(xmlDoc, imageDimensionsNode, "Height", cameraHandler.Height.ToString());
            CreateNode(xmlDoc, principalPointNode, "x", Utills.DoubleToStringSignificantDigits(cameraHandler.PrincipalPoint.x, DoubleSignificantDigits));
            CreateNode(xmlDoc, principalPointNode, "y", Utills.DoubleToStringSignificantDigits(cameraHandler.PrincipalPoint.y, DoubleSignificantDigits));
            foreach (var DistCoeff in cameraHandler.DistCoeffs)
            {
                CreateNode(xmlDoc, distortionNode, DistCoeff.Key, Utills.DoubleToStringSignificantDigits(DistCoeff.Value, DoubleSignificantDigits));
            }
            foreach (var image in cameraHandler.Images.Values)
            {
                var photoNode = CreateNode(xmlDoc, photogroupNode, "Photo");
                CreateNode(xmlDoc, photoNode, "ImagePath", image.File.FullName);
                var poseNode = CreateNode(xmlDoc, photoNode, "Pose");
                var rotationNode = CreateNode(xmlDoc, poseNode, "Rotation");
                CreateNode(xmlDoc, rotationNode, "Omega", Utills.DoubleToStringSignificantDigits(image.Omega, DoubleSignificantDigits));
                CreateNode(xmlDoc, rotationNode, "Phi", Utills.DoubleToStringSignificantDigits(image.Phi, DoubleSignificantDigits));
                CreateNode(xmlDoc, rotationNode, "Kappa", Utills.DoubleToStringSignificantDigits(image.Kappa, DoubleSignificantDigits));
                var centerNode = CreateNode(xmlDoc, poseNode, "Center");
                CreateNode(xmlDoc, centerNode, "x", Utills.DoubleToStringSignificantDigits(image.X, DoubleSignificantDigits));
                CreateNode(xmlDoc, centerNode, "y", Utills.DoubleToStringSignificantDigits(image.Y, DoubleSignificantDigits));
                CreateNode(xmlDoc, centerNode, "z", Utills.DoubleToStringSignificantDigits(image.Z, DoubleSignificantDigits));
            }
        }
        if (ProjectPath.DataValue.EndsWith("*") || saveAs)
        {
            ProjectPath.DataValue = ProjectPath.DataValue.TrimEnd('*');
            xmlDoc.Save(ProjectPath.DataValue);
        }
    }

    public void ModifyProjectPath(bool showMessageBox = true)
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            if (showMessageBox)
                MessageBoxCtrl.Instance.Show("空工程");
            return;
        }
        if (!ProjectPath.DataValue.EndsWith("*"))
        {
            ProjectPath.DataValue += "*";
        }
    }

    private void AddRootLeaf(XmlDocument xmlDoc, string rootName, string leafName, List<string> elements)
    {
        var rootNode = CreateNode(xmlDoc, xmlDoc.SelectSingleNode("Project"), rootName);
        foreach (string element in elements)
        {
            CreateNode(xmlDoc, rootNode, leafName, element);
        }
    }

    public void AddObliqueImagesBtnClick()
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("请先创建或打开工程");
            return;
        }
        string folderPath = FileBrowser.OpenSingleFolder("选择航拍影像文件夹");
        if (string.IsNullOrEmpty(folderPath))
        {
            MessageBoxCtrl.Instance.Show("选择航拍影像文件夹失败，\n未选择路径");
            return;
        }
        string[] extensions = new[] { ".jpg" };
        DirectoryInfo dinfo = new DirectoryInfo(folderPath);
        List<FileInfo> fileInfos = dinfo.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();
        List<string> obliqueImages = GetChildrenFromRootNode(ObliqueImagesTreeNode);
        for (int i = fileInfos.Count - 1; i >= 0; i--)
        {
            if (obliqueImages.Contains(fileInfos[i].FullName))
            {
                fileInfos.Remove(fileInfos[i]);
            }
        }
        fileInfos.Reverse();
        if (fileInfos.Count > 0)
        {
            ModifyProjectPath();
            StartCoroutine(AddObliqueImages(fileInfos));
        }
    }

    public IEnumerator AddObliqueImages(List<FileInfo> fileInfos, float waitTime = 0)
    {
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        yield return new WaitUntil(ProgressbarCtrl.Instance.isFinished);

        ProgressbarCtrl.Instance.Show("正在创建倾斜影像缩略图……");
        ProgressbarCtrl.Instance.ResetMaxCount(fileInfos.Count);

        foreach (var file in fileInfos)
        {
            string thumb_path = file.DirectoryName + "/thumb/";
            if (!Directory.Exists(thumb_path))
            {
                Directory.CreateDirectory(thumb_path);
            }
            yield return StartCoroutine(StartCreatingThumbs(file));
        }
    }

    IEnumerator StartCreatingThumbs(FileInfo fileInfo)
    {
        string thumb_path = fileInfo.DirectoryName + "/thumb/";
        if (!Directory.Exists(thumb_path))
        {
            Directory.CreateDirectory(thumb_path);
        }
        TreeViewItem item = new TreeViewItem(fileInfo.FullName);
        item.LocalizedName = fileInfo.Name;
        ObliqueImagesTreeNode.Nodes.Add(new TreeNode<TreeViewItem>(item));
        string thumb_name = thumb_path + fileInfo.Name;
        if (!File.Exists(thumb_name))
        {
            Image image = Image.FromFile(fileInfo.FullName);
            int thumb_height = (int)(m_thumb_width / (float)image.Width * image.Height + 0.5f);
            Image thumb = image.GetThumbnailImage(m_thumb_width, thumb_height, null, IntPtr.Zero);
            thumb.Save(thumb_name);
        }
        ProgressbarCtrl.Instance.ProgressPlusPlus();
        yield return null;
    }

    public void AddModelsBtnClick()
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("请先创建或打开工程");
            return;
        }
        string[] modelFiles;
        modelFiles = FileBrowser.OpenFiles("选择三维模型文件夹", null, "obj");
        if (modelFiles.Length == 0)
        {
            MessageBoxCtrl.Instance.Show("未选择模型文件");
            return;
        }
        List<FileInfo> fileInfos = new List<FileInfo>();
        foreach (string modelFile in modelFiles)
        {
            fileInfos.Add(new FileInfo(modelFile));
        }
        List<string> models = GetChildrenFromRootNode(ModelsTreeNode);
        for (int i = fileInfos.Count - 1; i >= 0; i--)
        {
            if (models.Contains(fileInfos[i].FullName))
            {
                fileInfos.Remove(fileInfos[i]);
            }
        }
        fileInfos.Reverse();
        if (fileInfos.Count > 0)
        {
            ModifyProjectPath();
        }
        StartCoroutine(AddModels(fileInfos));
    }

    public IEnumerator AddModels(List<FileInfo> fileInfos, float waitTime = 0)
    {
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        yield return new WaitUntil(ProgressbarCtrl.Instance.isFinished);
        ProgressbarCtrl.Instance.Show("正在解析三维模型……");
        ProgressbarCtrl.Instance.ResetMaxCount(fileInfos.Count);

        foreach (var fileInfo in fileInfos)
        {
            yield return StartCoroutine(StartParsingModels(fileInfo));
        }
    }

    IEnumerator StartParsingModels(FileInfo fileInfo)
    {
        StreamReader sr = new StreamReader(fileInfo.FullName);
        string firstLine = sr.ReadLine();

        bool isCompleteUvModel = (firstLine == CompleteUvCommentLine);
        bool createNewWhiteModel = true;
        string loadFileName = fileInfo.FullName;
        if (!isCompleteUvModel)
        {
            loadFileName = fileInfo.DirectoryName + "/CompleteUvModel/" + fileInfo.Name;
            if (File.Exists(loadFileName))
                createNewWhiteModel = false;
        }
        if (isCompleteUvModel)
        {
            yield return LoadModelFile(fileInfo);
        }
        else if (!createNewWhiteModel)
        {
            yield return LoadModelFile(fileInfo, false);
        }
        else
        {
            // 创建完整uv的白模，已经处理过的白模不会被再次加载
            string directoryName = Path.GetDirectoryName(loadFileName);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            string loadedText = File.ReadAllText(fileInfo.FullName);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(CompleteUvCommentLine);
            string[] lines = loadedText.Split("\n".ToCharArray());
            char[] separators = new char[] { ' ', '\t' };
            int numVertex = 0;
            int numNormal = 0;
            int numUv = 0;
            List<string> faceLines = new List<string>();
            for (int j = 0; j < lines.Length; j++)
            {
                string line = lines[j].Trim();
                if (line.Length > 0 && line[0] == '#')
                {
                    // comment line
                    continue;
                }
                string[] p = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length == 0)
                {
                    // empty line
                    continue;
                }
                switch (p[0])
                {
                    case "v":
                        numVertex++;
                        sb.AppendLine(line);
                        break;
                    case "vn":
                        numNormal++;
                        sb.AppendLine(line);
                        break;
                    case "g":
                        sb.AppendLine(line);
                        break;
                    case "f":
                        {
                            //numVerts += p.Length - 1;
                            faceLines.Add(line);
                        }
                        break;
                }
            }

            List<string> faces = new List<string>();
            foreach (string oldFace in faceLines)
            {
                List<string> vertexes = oldFace.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
                vertexes.RemoveAt(0);
                string newFace = "f ";
                Dictionary<string, int> uniqueVertexes = new Dictionary<string, int>();
                foreach (string vertex in vertexes)
                {
                    if (!uniqueVertexes.ContainsKey(vertex))
                    {
                        numUv++;
                        uniqueVertexes.Add(vertex, numUv);
                    }
                    string[] vertexElements = vertex.Split('/');
                    newFace += (int.Parse(vertexElements[0]) + numVertex + 1).ToString() + '/' + uniqueVertexes[vertex].ToString() + '/' + (int.Parse(vertexElements[2]) + numNormal + 1).ToString() + ' ';
                }
                faces.Add(newFace);
            }
            for (int j = 0; j < numUv; ++j)
            {
                sb.AppendLine(EmptyUv);
            }
            foreach (var face in faces)
            {
                sb.AppendLine(face);
            }

            File.WriteAllText(loadFileName, sb.ToString());
            yield return LoadModelFile(fileInfo, false);
        }
    }

    IEnumerator LoadModelFile(FileInfo file, bool isCompleteUvModel = true)
    {
        TreeViewItem item = new TreeViewItem(file.FullName);
        item.LocalizedName = file.Name;
        ModelsTreeNode.Nodes.Add(new TreeNode<TreeViewItem>(item));
        ObjImportHandler objImportHandler = new ObjImportHandler();
        StartCoroutine(objImportHandler.Load(file.Name, file.FullName, ModelContainer, isCompleteUvModel));
        yield return null;
    }

    public void AddSceneryBtnClick()
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("请先创建或打开工程");
            return;
        }
        string filePath = FileBrowser.OpenSingleFile("选择地景模型文件", null, "obj");
        if (filePath.Length == 0)
        {
            MessageBoxCtrl.Instance.Show("未选择地景模型文件");
            return;
        }
        List<string> sceneries = GetChildrenFromRootNode(SceneriesTreeNode);
        if (sceneries.Contains(filePath))
        {
            MessageBoxCtrl.Instance.Show("已经加载过此地景模型");
            return;
        }
        MessageBoxCtrl.Instance.Show("正在加载地景模型……", false, false);
        ObjLoadManger.Instance.GetComponent<ObjectImporter>().ImportedModel += ObjImporter_ImportedSingleModel;
        StartCoroutine(ObjLoadManger.Instance.ImportModelAsync(Path.GetFileName(filePath), filePath));
    }

    public IEnumerator AddSceneries(List<FileInfo> fileInfos, float waitTime = 0)
    {
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        yield return new WaitUntil(ProgressbarCtrl.Instance.isFinished);

        ProgressbarCtrl.Instance.Show("正在加载地景模型……");
        ProgressbarCtrl.Instance.ResetMaxCount(fileInfos.Count);

        ObjLoadManger.Instance.GetComponent<ObjectImporter>().ImportedModel += ObjImporter_ImportedModel;
        foreach (var file in fileInfos)
        {
            yield return StartCoroutine(ObjLoadManger.Instance.ImportModelAsync(Path.GetFileName(file.Name), file.FullName));
        }
    }

    private void ObjImporter_ImportedModel(GameObject go, string path)
    {
        ProgressbarCtrl.Instance.ProgressPlusPlus();
        FileInfo fileInfo = new FileInfo(path);
        TreeViewItem item = new TreeViewItem(fileInfo.FullName);
        item.LocalizedName = fileInfo.Name;
        SceneriesTreeNode.Nodes.Add(new TreeNode<TreeViewItem>(item));
        if (ProgressbarCtrl.Instance.isFinished())
        {
            ObjLoadManger.Instance.GetComponent<ObjectImporter>().ImportedModel -= ObjImporter_ImportedModel;
        }
    }

    private void ObjImporter_ImportedSingleModel(GameObject go, string path)
    {
        ModifyProjectPath();
        FileInfo fileInfo = new FileInfo(path);
        TreeViewItem item = new TreeViewItem(fileInfo.FullName);
        item.LocalizedName = fileInfo.Name;
        SceneriesTreeNode.Nodes.Add(new TreeNode<TreeViewItem>(item));
        ObjLoadManger.Instance.GetComponent<ObjectImporter>().ImportedModel -= ObjImporter_ImportedSingleModel;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ParseSmart3DXmlBtnClick()
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("请先创建或打开工程");
            return;
        }
        string path = FileBrowser.OpenSingleFile("选择相机参数文件", null, "xml");
        if (string.IsNullOrEmpty(path))
        {
            MessageBoxCtrl.Instance.Show("未选择Smart3D空三数据文件");
            return;
        }
        ModifyProjectPath();
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create(path, set));
        ParsePhotogroups(xml, true);
    }

    public void ParseQZSYBtnClick()
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("请先创建或打开工程");
            return;
        }
        string camera_path = FileBrowser.OpenSingleFile("选择相机参数文件", null, "txt");
        if (string.IsNullOrEmpty(camera_path))
        {
            MessageBoxCtrl.Instance.Show("未选择相机参数文件");
            return;
        }
        string pos_path = FileBrowser.OpenSingleFile("选择影像参数文件", null, "txt");
        if (string.IsNullOrEmpty(pos_path))
        {
            MessageBoxCtrl.Instance.Show("未选择影像参数文件");
            return;
        }
        ModifyProjectPath();
        ParseQZSYPhotogroups(camera_path, pos_path);
    }

    private void ParsePhotogroups(XmlDocument xml, bool isSmart3D)
    {
        //CameraHandlers.Clear();
        XmlNodeList photogroups = xml.SelectSingleNode("//Photogroups").SelectNodes("Photogroup");
        foreach (XmlNode photogroup in photogroups)
        {
            CameraHandler cameraHandler;
            string name = photogroup.SelectSingleNode("Name").InnerText;
            int width;
            int height;
            if (!CameraHandlers.TryGetValue(name, out cameraHandler))
            {
                width = int.Parse(photogroup.SelectSingleNode("ImageDimensions/Width").InnerText);
                height = int.Parse(photogroup.SelectSingleNode("ImageDimensions/Height").InnerText);
                double focalLength = double.Parse(photogroup.SelectSingleNode("FocalLength").InnerText);
                Point principalPoint = new Point(double.Parse(photogroup.SelectSingleNode("PrincipalPoint/x").InnerText), double.Parse(photogroup.SelectSingleNode("PrincipalPoint/y").InnerText));
                if (isSmart3D)
                {
                    double sensorSize = double.Parse(photogroup.SelectSingleNode("SensorSize").InnerText);
                    cameraHandler = new CameraHandler(name, width, height, focalLength, sensorSize, principalPoint, photogroup.SelectSingleNode("Distortion"));
                }
                else
                {
                    cameraHandler = new CameraHandler(name, width, height, focalLength, principalPoint, photogroup.SelectSingleNode("Distortion"));
                }                   
                CameraHandlers.Add(name, cameraHandler);
            }
            else
            {
                width = cameraHandler.Width;
                height = cameraHandler.Height;
            }
            foreach (XmlNode photoNode in photogroup.SelectNodes("Photo"))
            {
                cameraHandler.AddImageInfo(photoNode, width, height);
            }
        }
        if (isSmart3D)
            MessageBoxCtrl.Instance.Show("解析Smart3D空三数据成功");
    }

    private void ParseQZSYPhotogroups(string camera_path, string pos_path)
    {
        //CameraHandlers.Clear();
        int width;
        int height;
        string loadedText = File.ReadAllText(camera_path, Encoding.GetEncoding("GBK"));
        string[] lines = loadedText.Split("\n".ToCharArray());
        string line = lines[1];
        line = line.Replace("\r", "");
        string[] words = System.Text.RegularExpressions.Regex.Split(line, @"\s+");
        CameraHandler cameraHandler;
        string name = words[0];

        if (!CameraHandlers.TryGetValue(name, out cameraHandler))
        {
            width = int.Parse(words[1]);
            height = int.Parse(words[2]);
            double focalLength = double.Parse(words[4]);
            Point principalPoint = new Point(double.Parse(words[5]), double.Parse(words[6]));
            cameraHandler = new CameraHandler(name, width, height, focalLength, principalPoint);
            CameraHandlers.Add(name, cameraHandler);
        }
        else
        {
            width = cameraHandler.Width;
            height = cameraHandler.Height;
        }
        loadedText = File.ReadAllText(pos_path, Encoding.GetEncoding("GBK"));
        lines = loadedText.Split("\n".ToCharArray());
        for (int i = 0; i < lines.Count(); ++i)
        {
            line = lines[i];
            if (string.IsNullOrEmpty(line))
                continue;
            if (line[0] == '#')
                continue;
            line = line.Replace("\r", "");            
            cameraHandler.AddImageInfo(line, width, height);
        }
        MessageBoxCtrl.Instance.Show("解析添加相机内外参成功");
    }

    public List<ImageInfo> ProjectPoints(Dictionary<int, Vector3> points, Vector3 faceNormal)
    {
        List<ImageInfo> imageInfos = new List<ImageInfo>();
        foreach (var cameraHandler in CameraHandlers)
        {
            imageInfos.AddRange(cameraHandler.Value.ProjectPoints(points, faceNormal));
        }
        return imageInfos;
    }

    public XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value = null)
    {
        XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
        if (value != null)
            node.InnerText = value;
        parentNode.AppendChild(node);
        return node;
    }

    public List<string> GetObliqueImageList()
    {
        return GetChildrenFromRootNode(ObliqueImagesTreeNode);
    }

    private List<string> GetChildrenFromRootNode(TreeNode<TreeViewItem> treeNode)
    {
        List<string> fileInfos = new List<string>();
        foreach (var node in treeNode.Nodes)
        {
            fileInfos.Add(node.Item.Name);
        }
        return fileInfos;
    }
}
