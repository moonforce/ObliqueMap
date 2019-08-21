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

    private TreeView m_Tree;

    public Dictionary<string, CameraHandler> CameraHandlers { get; set; } = new Dictionary<string, CameraHandler>();
    public List<string> ObliqueImages { get; set; } = new List<string>();
    public List<string> Models { get; set; } = new List<string>();
    public List<string> Sceneries { get; set; } = new List<string>();
    public TreeNode<TreeViewItem> ObliqueImagesTreeNode { get; set; } = new TreeNode<TreeViewItem>(new TreeViewItem("航拍斜片"));
    public TreeNode<TreeViewItem> ModelsTreeNode { get; set; } = new TreeNode<TreeViewItem>(new TreeViewItem("三维模型"));
    public TreeNode<TreeViewItem> SceneryTreeNode { get; set; } = new TreeNode<TreeViewItem>(new TreeViewItem("地面模型"));
    public DataExchange<string> ProjectPath = new DataExchange<string>();

    void Start()
    {
        m_Tree = transform.GetComponentInChildren<TreeView>();
        m_Tree.Init();
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
        nodes.Add(SceneryTreeNode);
        ObliqueImagesTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        ModelsTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        SceneryTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        m_Tree.Nodes = nodes;
    }

    private void ClearProject()
    {
        CameraHandlers.Clear();
        ObliqueImages.Clear();
        Models.Clear();
        Sceneries.Clear();
        ObliqueImagesTreeNode.IsExpanded = false;
        ObliqueImagesTreeNode.Nodes.Clear();
        ModelsTreeNode.IsExpanded = false;
        ModelsTreeNode.Nodes.Clear();
        SceneryTreeNode.IsExpanded = false;
        SceneryTreeNode.Nodes.Clear();
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
        XmlNodeList ObliqueImageNodes = xml.SelectSingleNode("//ObliqueImages").SelectNodes("ObliqueImage");
        List<FileInfo> imageFileInfos = new List<FileInfo>();
        foreach (XmlNode ObliqueImageNode in ObliqueImageNodes)
        {
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
            modelFileInfos.Add(new FileInfo(Model.InnerText));
        }
        if (modelFileInfos.Count > 0)
            StartCoroutine(AddModels(modelFileInfos, true));
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
        SaveProject();
    }

    private void SaveProject()
    {
        if (string.IsNullOrEmpty(ProjectPath.DataValue))
        {
            MessageBoxCtrl.Instance.Show("保存工程失败，还未创建或打开");
            return;
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));
        xmlDoc.AppendChild(xmlDoc.CreateElement("Project"));
        AddRootLeaf(xmlDoc, "ObliqueImages", "ObliqueImage", ObliqueImages);
        AddRootLeaf(xmlDoc, "Models", "Model", Models);
        AddRootLeaf(xmlDoc, "Sceneries", "Scenery", Sceneries);
        var photogroupsNode = CreateNode(xmlDoc, xmlDoc.SelectSingleNode("Project"), "Photogroups");
        foreach (var cameraHandler in CameraHandlers.Values)
        {
            var photogroupNode = CreateNode(xmlDoc, photogroupsNode, "Photogroup");
            CreateNode(xmlDoc, photogroupNode, "Name", cameraHandler.Name);
            var imageDimensionsNode = CreateNode(xmlDoc, photogroupNode, "ImageDimensions");
            CreateNode(xmlDoc, photogroupNode, "FocalLength", Utills.DoubleToStringSignificantDigits(cameraHandler.OrigonalFocalLength, DoubleSignificantDigits));
            CreateNode(xmlDoc, photogroupNode, "SensorSize", Utills.DoubleToStringSignificantDigits(cameraHandler.SensorSize, DoubleSignificantDigits));
            var principalPointNode = CreateNode(xmlDoc, photogroupNode, "PrincipalPoint");
            var distortionNode = CreateNode(xmlDoc, photogroupNode, "Distortion");
            CreateNode(xmlDoc, photogroupNode, "AspectRatio", Utills.DoubleToStringSignificantDigits(cameraHandler.AspectRatio, DoubleSignificantDigits));
            CreateNode(xmlDoc, imageDimensionsNode, "Width", cameraHandler.Width.ToString());
            CreateNode(xmlDoc, imageDimensionsNode, "Height", cameraHandler.Height.ToString());
            CreateNode(xmlDoc, principalPointNode, "x", Utills.DoubleToStringSignificantDigits(cameraHandler.PrincipalPoint.x, DoubleSignificantDigits));
            CreateNode(xmlDoc, principalPointNode, "y", Utills.DoubleToStringSignificantDigits(cameraHandler.PrincipalPoint.y, DoubleSignificantDigits));
            foreach (var DistCoeff in cameraHandler.DistCoeffs)
            {
                CreateNode(xmlDoc, distortionNode, DistCoeff.Key, Utills.DoubleToStringSignificantDigits(DistCoeff.Value, DoubleSignificantDigits));
            }
            foreach (var image in cameraHandler.Images)
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
        if (ProjectPath.DataValue.EndsWith("*"))
        {
            ProjectPath.DataValue = ProjectPath.DataValue.TrimEnd('*');
            xmlDoc.Save(ProjectPath.DataValue);
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
            MessageBoxCtrl.Instance.Show("选择航拍影像文件夹失败，未选择路径");
            return;
        }
        string[] extensions = new[] { ".jpg" };
        DirectoryInfo dinfo = new DirectoryInfo(folderPath);
        List<FileInfo> fileInfos = dinfo.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();
        for (int i = fileInfos.Count - 1; i >= 0; i--)
        {
            if (ObliqueImages.Contains(fileInfos[i].FullName))
            {
                fileInfos.Remove(fileInfos[i]);
            }
        }
        fileInfos.Reverse();
        if (fileInfos.Count > 0)
        {
            if (!ProjectPath.DataValue.EndsWith("*"))
            {
                ProjectPath.DataValue += "*";
            }
            StartCoroutine(AddObliqueImages(fileInfos));
        }
    }

    public IEnumerator AddObliqueImages(List<FileInfo> fileInfos, bool wait = false)
    {
        if (wait)
        {
            yield return new WaitForSeconds(1);
        }
        yield return new WaitUntil(ProgressbarCtrl.Instance.isFinished);

        ProgressbarCtrl.Instance.Show("正在创建倾斜影像缩略图……");
        ProgressbarCtrl.Instance.ResetMaxCount(fileInfos.Count);

        foreach (var file in fileInfos)
        {
            ObliqueImages.Add(file.FullName);
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
        for (int i = fileInfos.Count - 1; i >= 0; i--)
        {
            if (Models.Contains(fileInfos[i].FullName))
            {
                fileInfos.Remove(fileInfos[i]);
            }
        }
        fileInfos.Reverse();
        if (fileInfos.Count > 0)
        {
            if (!ProjectPath.DataValue.EndsWith("*"))
            {
                ProjectPath.DataValue += "*";
            }
        }
        StartCoroutine(AddModels(fileInfos));
    }

    public IEnumerator AddModels(List<FileInfo> fileInfos, bool wait = false)
    {
        if (wait)
        {
            yield return new WaitForSeconds(1);
        }
        yield return new WaitUntil(ProgressbarCtrl.Instance.isFinished);
        ProgressbarCtrl.Instance.Show("正在解析三维模型……");
        ProgressbarCtrl.Instance.ResetMaxCount(fileInfos.Count);

        foreach (var fileInfo in fileInfos)
        {
            Models.Add(fileInfo.FullName);
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
        string filePath = FileBrowser.OpenSingleFile();
        if (filePath.Length == 0)
            return;
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
        if (!ProjectPath.DataValue.EndsWith("*"))
        {
            ProjectPath.DataValue += "*";
        }
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create(path, set));
        ParsePhotogroups(xml);
    }

    private void ParsePhotogroups(XmlDocument xml, bool showMessage = true)
    {
        CameraHandlers.Clear();
        XmlNodeList photogroups = xml.SelectSingleNode("//Photogroups").SelectNodes("Photogroup");
        foreach (XmlNode photogroup in photogroups)
        {
            List<ImageInfo> imageInfos = new List<ImageInfo>();
            foreach (XmlNode photoNode in photogroup.SelectNodes("Photo"))
            {
                imageInfos.Add(new ImageInfo(new FileInfo(photoNode.SelectSingleNode("ImagePath").InnerText),
                    double.Parse(photoNode.SelectSingleNode("Pose/Rotation/Omega").InnerText),
                    double.Parse(photoNode.SelectSingleNode("Pose/Rotation/Phi").InnerText),
                    double.Parse(photoNode.SelectSingleNode("Pose/Rotation/Kappa").InnerText),
                    double.Parse(photoNode.SelectSingleNode("Pose/Center/x").InnerText),
                    double.Parse(photoNode.SelectSingleNode("Pose/Center/y").InnerText),
                    double.Parse(photoNode.SelectSingleNode("Pose/Center/z").InnerText)));
            }

            CameraHandler cameraHandler;
            string name = photogroup.SelectSingleNode("Name").InnerText;
            if (!CameraHandlers.TryGetValue(name, out cameraHandler))
            {
                int width = int.Parse(photogroup.SelectSingleNode("ImageDimensions/Width").InnerText);
                int height = int.Parse(photogroup.SelectSingleNode("ImageDimensions/Height").InnerText);
                double focalLength = double.Parse(photogroup.SelectSingleNode("FocalLength").InnerText);
                double sensorSize = double.Parse(photogroup.SelectSingleNode("SensorSize").InnerText);
                Point principalPoint = new Point(double.Parse(photogroup.SelectSingleNode("PrincipalPoint/x").InnerText), double.Parse(photogroup.SelectSingleNode("PrincipalPoint/y").InnerText));
                double aspectRatio = double.Parse(photogroup.SelectSingleNode("AspectRatio").InnerText);
                cameraHandler = new CameraHandler(name, width, height, focalLength, sensorSize, principalPoint, photogroup.SelectSingleNode("Distortion"), aspectRatio);
                CameraHandlers.Add(name, cameraHandler);
            }
            cameraHandler.Images.AddRange(imageInfos);
        }
        if (showMessage)
            MessageBoxCtrl.Instance.Show("解析Smart3D空三数据成功");
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

    public void ClearUselessTextures()
    {
        foreach (MeshRenderer meshRenderer in ModelContainer.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (meshRenderer.name == "ModelGridPlane")
                continue;
            SubMeshInfo subMeshInfo = meshRenderer.GetComponent<SubMeshInfo>();
            string prefix = Path.GetFileNameWithoutExtension(subMeshInfo.name);
            string postfix = "*.jpg";
            List<string> imageFiles = Directory.GetFiles(Path.GetDirectoryName(subMeshInfo.FilePath), prefix + postfix).ToList();
            // 如果引用的是materials，会给所有的材质名增加(Instanced)后缀
            foreach (Material material in meshRenderer.sharedMaterials)
            {
                string matImagePath = Path.GetDirectoryName(subMeshInfo.FilePath) + '\\' + material.name;
                imageFiles.Remove(matImagePath);
            }
            foreach (string imageFile in imageFiles)
            {
                File.Delete(imageFile);
            }
        }      
    }

    public static XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value = null)
    {
        XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
        if (value != null)
            node.InnerText = value;
        parentNode.AppendChild(node);
        return node;
    }
}
