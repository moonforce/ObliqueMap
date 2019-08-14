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
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.UnityUtils;
using Point = OpenCVForUnity.CoreModule.Point;
using UnityEngine.Networking;
using System.Text;
using ObjLoaderLY;

public class ProjectCtrl : Singleton<ProjectCtrl>
{
    protected ProjectCtrl() { }

    string m_ProjectPath;
    int DoubleSignificantDigits = 17;

    public Transform ModelContainer;

    public string CompleteUvCommentLine { get; } = "# This Obj File Has Complete UVs";
    public string EmptyUv { get; } = "vt 0 0 0";

    const int m_thumb_width = 240;
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

    public bool Debug = true;

    void Start()
    {
        m_Tree = transform.GetComponentInChildren<TreeView>();
        m_Tree.Init();
        SetTreeNodes();

        if (Debug)
        {
            Invoke("testData", 1);
        }
    }

    void testData()
    {
        AddObliqueImages(@"E:\倾斜贴图测试数据\jpg");
        AddModels(@"E:\倾斜贴图测试数据\obj");
        ParseSmart3DXml(@"E:\倾斜贴图测试数据\A7 -export.xml");
    }

    void SetTreeNodes()
    {
        var nodes = new ObservableList<TreeNode<TreeViewItem>>();
        nodes.Add(ObliqueImagesTreeNode);
        nodes.Add(ModelsTreeNode);
        nodes.Add(SceneryTreeNode);
        ObliqueImagesTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        ModelsTreeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        m_Tree.Nodes = nodes;
    }

    public void CreateProject()
    {
        m_ProjectPath = FileBrowser.SaveFile("NewProject", "omp");
        if (string.IsNullOrEmpty(m_ProjectPath))
            MessageBoxCtrl.Instance.Show("创建工程失败，未选择路径");
    }

    public void SaveProject()
    {
        if (string.IsNullOrEmpty(m_ProjectPath))
            return;
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
        xmlDoc.Save(m_ProjectPath);
    }

    private void AddRootLeaf(XmlDocument xmlDoc, string rootName, string leafName, List<string> elements)
    {
        var rootNode = CreateNode(xmlDoc, xmlDoc.SelectSingleNode("Project"), rootName);
        foreach (string element in elements)
        {
            CreateNode(xmlDoc, rootNode, leafName, element);
        }
    }

    public void AddObliqueImages(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = FileBrowser.OpenSingleFolder("选择航拍影像文件夹");
            if (folderPath.Length == 0)
                return;
        }
        string[] extensions = new[] { ".jpg" };
        DirectoryInfo dinfo = new DirectoryInfo(folderPath);
        List<FileInfo> files = dinfo.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();
        for (int i = files.Count - 1; i >= 0; i--)
        {
            if (ObliqueImages.Contains(files[i].FullName))
            {
                files.Remove(files[i]);
            }
            else
            {
                ObliqueImages.Add(files[i].FullName);
            }
        }
        if (files.Count > 0)
        {
            string thumb_path = folderPath + "/thumb/";
            bool haveThumb = true;
            if (!Directory.Exists(thumb_path))
            {
                haveThumb = false;
                Directory.CreateDirectory(thumb_path);
            }
            ProgressbarCtrl.Instance.Show("正在创建倾斜影像缩略图……");
            StartCoroutine(StartCreatingThumbs(thumb_path, haveThumb, files));
        }
    }

    IEnumerator StartCreatingThumbs(string thumb_path, bool haveThumb, List<FileInfo> files)
    {
        int fileCount = files.Count;
        for (int i = 0; i < fileCount; ++i)
        {
            TreeViewItem item = new TreeViewItem(files[i].FullName);
            item.LocalizedName = files[i].Name;
            ObliqueImagesTreeNode.Nodes.Add(new TreeNode<TreeViewItem>(item));
            string thumb_name = thumb_path + files[i].Name;
            if (!haveThumb || !File.Exists(thumb_name))
            {
                Image image = Image.FromFile(files[i].FullName);
                int thumb_height = (int)(m_thumb_width / (float)image.Width * image.Height + 0.5f);
                Image thumb = image.GetThumbnailImage(m_thumb_width, thumb_height, null, IntPtr.Zero);
                thumb.Save(thumb_name);
            }
            ProgressbarCtrl.Instance.SetProgressbar((int)((i + 1) * 100f / fileCount + 0.5f));
            yield return null;
        }
        ProgressbarCtrl.Instance.Hide();
    }

    public void AddModels(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = FileBrowser.OpenSingleFolder("选择三维模型文件夹");
            if (folderPath.Length == 0)
                return;
        }
        string[] extensions = new[] { ".obj" };
        DirectoryInfo dinfo = new DirectoryInfo(folderPath);
        List<FileInfo> files = dinfo.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();
        for (int i = files.Count - 1; i >= 0; i--)
        {
            if (Models.Contains(files[i].FullName))
            {
                files.Remove(files[i]);
            }
            else
            {
                Models.Add(files[i].FullName);
            }
        }
        if (files.Count > 0)
        {
            ProgressbarCtrl.Instance.Show("正在解析三维模型……");
            ProgressbarCtrl.Instance.ResetMaxCount(files.Count);
            StartCoroutine(StartParsingModels(files));
        }
    }

    IEnumerator StartParsingModels(List<FileInfo> files)
    {
        int fileCount = files.Count;
        for (int i = 0; i < fileCount; ++i)
        {
            StreamReader sr = new StreamReader(files[i].FullName);
            string firstLine = sr.ReadLine();

            bool isCompleteUvModel = (firstLine == CompleteUvCommentLine);
            bool createNewWhiteModel = true;
            string loadFileName = files[i].FullName;
            if (!isCompleteUvModel)
            {
                loadFileName = files[i].DirectoryName + "/CompleteUvModel/" + files[i].Name;
                if (File.Exists(loadFileName))
                    createNewWhiteModel = false;
            }
            if (isCompleteUvModel)
            {
                yield return LoadModelFile(files[i]);
            }
            else if (!createNewWhiteModel)
            {
                yield return LoadModelFile(files[i], false);
            }
            else
            {
                // 创建完整uv的白模，已经处理过的白模不会被再次加载
                string directoryName = Path.GetDirectoryName(loadFileName);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                string loadedText = File.ReadAllText(files[i].FullName);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(CompleteUvCommentLine);
                string[] lines = loadedText.Split("\n".ToCharArray());
                char[] separators = new char[] { ' ', '\t' };
                int numVerts = 0;
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
                            sb.AppendLine(line);
                            break;
                        case "vn":
                            sb.AppendLine(line);
                            break;
                        case "g":
                            sb.AppendLine(line);
                            break;
                        case "f":
                            {
                                numVerts += p.Length - 1;
                                faceLines.Add(line);
                            }
                            break;
                    }
                }
                for (int j = 0; j < numVerts; ++j)
                {
                    sb.AppendLine(EmptyUv);
                }
                int uvIndex = 0;
                foreach (string oldFace in faceLines)
                {
                    List<string> vertexes = oldFace.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
                    vertexes.RemoveAt(0);
                    string newFace = "f ";
                    foreach (string vertex in vertexes)
                    {
                        string[] vertexElements = vertex.Split('/');
                        newFace += vertexElements[0] + '/' + (uvIndex++ - numVerts).ToString() + '/' + vertexElements[2] + ' ';
                    }
                    sb.AppendLine(newFace);
                }
                File.WriteAllText(loadFileName, sb.ToString());
                yield return LoadModelFile(files[i], false);
            }
        }
        //ProgressbarCtrl.Instance.Hide();
    }

    IEnumerator LoadModelFile(FileInfo file, bool isCompleteUvModel = true)
    {
        TreeViewItem item = new TreeViewItem(file.FullName);
        item.LocalizedName = file.Name;
        ModelsTreeNode.Nodes.Add(new TreeNode<TreeViewItem>(item));
        ObjImportHandler objImportHandler = new ObjImportHandler();
        StartCoroutine(objImportHandler.Load(file.Name, file.FullName, ModelContainer, isCompleteUvModel));
        //ObjLoadManger.Instance.ImportModelAsync(item.LocalizedName, item.Name, isWhiteModel);
        yield return null;
    }

    public void AddScenery()
    {
        string filePath = FileBrowser.OpenSingleFile();
        if (filePath.Length == 0)
            return;
    }

    public void ParseSmart3DXml(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = FileBrowser.OpenSingleFile("选择相机参数文件", null, "xml");
            if (path.Length == 0)
                return;
        }
        CameraHandlers.Clear();
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create(path, set));
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

    public static XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value = null)
    {
        XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
        if (value != null)
            node.InnerText = value;
        parentNode.AppendChild(node);
        return node;
    }    
}
