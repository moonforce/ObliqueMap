using UnityEngine;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using System.IO;
using System.Linq;

public class MeshAnaliser : Singleton<MeshAnaliser>
{
    protected MeshAnaliser() { }

    static Shader OutlineShader;
    static Shader NormalShader;

    Camera m_MainCamera;
    Transform m_MainLight;
    Quaternion m_MainLightOrigonQuaternion;

    public Texture2D WhiteTexture2D;

    public SubMeshInfo ClickedSubMeshInfo { get; set; }
    public int ClickedSubMeshIndex { get; set; }
    public Material ClickedMaterial { get; set; }
    public Mesh ClickedMesh { get; set; }

    private void Start()
    {
        m_MainCamera = Camera.main;
        m_MainLight = GameObject.Find("MainLight").transform;
        m_MainLightOrigonQuaternion = m_MainLight.rotation;

        OutlineShader = Shader.Find("Outline/VerticsOutline_Always");
        NormalShader = Shader.Find("Standard");
    }

    void OnGUI()
    {
        if (Event.current.isMouse && Event.current.button == 0 && Event.current.clickCount == 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_MainCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (9 == hit.transform.gameObject.layer) //Model layer
                {
                    if (ProjectStage.Instance.FaceEditting)
                    {
                        ResetChoice();
                    }
                    ProjectStage.Instance.FaceChosed = true;
                    m_MainLight.rotation = Quaternion.LookRotation(-hit.normal) * Quaternion.Euler(45, 0, 45);
                    ClickedMesh = Utills.GetMeshOfGameobject(hit.transform.gameObject);

                    int[] hittedTriangle = new int[]
                    {
                    ClickedMesh.triangles[hit.triangleIndex * 3],
                    ClickedMesh.triangles[hit.triangleIndex * 3 + 1],
                    ClickedMesh.triangles[hit.triangleIndex * 3 + 2]
                    };
                    bool find = false;
                    for (int i = 0; i < ClickedMesh.subMeshCount; i++)
                    {
                        int[] subMeshTris = ClickedMesh.GetTriangles(i);
                        for (int j = 0; j < subMeshTris.Length; j += 3)
                        {
                            if (subMeshTris[j] == hittedTriangle[0] &&
                                subMeshTris[j + 1] == hittedTriangle[1] &&
                                subMeshTris[j + 2] == hittedTriangle[2])
                            {
                                ClickedSubMeshIndex = i;
                                HighlightMaterial();
                                find = true;                                
                                break;
                            }
                        }
                        if (find)
                            break;
                    }                    
                }
            }
            else
            {
                if (m_MainCamera.pixelRect.Contains(Input.mousePosition))
                    ResetChoice();
            }
        }
        //else if (Editting
        //    && m_MainCamera.pixelRect.Contains(Input.mousePosition)
        //    && Event.current.isMouse && Event.current.button == 1 && Event.current.clickCount == 2)
        //{
        //    ResetChoice();
        //}
    }

    public void StartEditting()
    {
        TextureHandler.Instance.ResetContent();
        ClickedSubMeshInfo = ObliqueMapTreeView.CurrentGameObject.GetComponent<SubMeshInfo>();
        List<int> indecies = ClickedSubMeshInfo.FaceNewIndexLists[ClickedSubMeshIndex];
        Vector3[] allVertices = ClickedMesh.vertices;
        Dictionary<int, Vector3> subMeshVertices = new Dictionary<int, Vector3>();
        foreach (int index in indecies)
        {
            if (!subMeshVertices.ContainsKey(index))
            {
                //注意y/z的互换
                subMeshVertices.Add(index, new Vector3(allVertices[index].x + 0, allVertices[index].z + 0, allVertices[index].y));
            }
        }
        Vector3 faceNormal = new Vector3(ClickedMesh.normals[indecies[0]].x, ClickedMesh.normals[indecies[0]].y, ClickedMesh.normals[indecies[0]].z);
        List<ImageInfo> imageInfos = ProjectCtrl.Instance.ProjectPoints(subMeshVertices, faceNormal);
        imageInfos = imageInfos.OrderBy(it => it.DirectionDot).ToList();

        for (int i = imageInfos.Count - 1; i >= 0; i--)
        {
            //路径不同
            string path = ProjectCtrl.Instance.GetObliqueImageList().Find(o => Path.GetFileName(o) == imageInfos[i].File.Name);
            if (path == null)
                imageInfos.Remove(imageInfos[i]);
            else
                imageInfos[i].File = new FileInfo(path);
            //路径相同
            //if (!ProjectCtrl.Instance.ObliqueImages.Contains(imageInfos[i].File.FullName))
            //{
            //    imageInfos.Remove(imageInfos[i]);
            //}
        }
        ImageGallery.Instance.ClearContents();
        for (int i = 0; i < imageInfos.Count; ++i)
        {
            ImageGallery.Instance.AddImage(imageInfos[i], ClickedSubMeshInfo.LineIndexLists[ClickedSubMeshIndex], i);
        }
        ImageGallery.Instance.GetComponent<AutoResizeImageGallerySize>().UpdateLayout();
    }

    public void ResetChoice()
    {
        ProjectStage.Instance.FaceChosed = false;
        ProjectStage.Instance.FaceEditting = false;
        m_MainLight.rotation = m_MainLightOrigonQuaternion;
        if (ClickedMaterial)
        {
            SwitchShader(NormalShader);
        }
        ClickedMaterial = null;
        ClickedSubMeshIndex = -1;
        ClickedMesh = null;
        ImageGallery.Instance.ClearContents();
        TextureHandler.Instance.ResetContent();
    }

    private void HighlightMaterial()
    {
        //修改判断条件
        if (ClickedMaterial)
        {
            SwitchShader(NormalShader);
        }
        ClickedMaterial = ObliqueMapTreeView.CurrentGameObject.GetComponentInChildren<MeshRenderer>().sharedMaterials[ClickedSubMeshIndex];
        SwitchShader(OutlineShader);
    }

    public void DestroyClickedMainTexture(bool reset = false)
    {
        if (ClickedMaterial.mainTexture != null && ClickedMaterial.mainTexture != WhiteTexture2D)
        {
            Destroy(ClickedMaterial.mainTexture);            
        }
        if (reset)
        {
            ClickedMaterial.mainTexture = WhiteTexture2D;
            ClickedMaterial.name = ObjExportHandler.DefaultMatName;
        }        
    }

    public string GetClickedImagePath()
    {
        return Path.GetDirectoryName(ClickedSubMeshInfo.FilePath) + '/' + ClickedMaterial.name;
    }

    public int GetClickedSubmeshIndex()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_MainCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (9 == hit.transform.gameObject.layer) //Model layer
            {
                Mesh clickedMesh = Utills.GetMeshOfGameobject(hit.transform.gameObject);

                int[] hittedTriangle = new int[]
                {
                    clickedMesh.triangles[hit.triangleIndex * 3],
                    clickedMesh.triangles[hit.triangleIndex * 3 + 1],
                    clickedMesh.triangles[hit.triangleIndex * 3 + 2]
                };

                for (int i = 0; i < clickedMesh.subMeshCount; i++)
                {
                    int[] subMeshTris = clickedMesh.GetTriangles(i);
                    for (int j = 0; j < subMeshTris.Length; j += 3)
                    {
                        if (subMeshTris[j] == hittedTriangle[0] &&
                            subMeshTris[j + 1] == hittedTriangle[1] &&
                            subMeshTris[j + 2] == hittedTriangle[2])
                        {
                            return i;
                        }
                    }
                }
            }
        }
        return -1;
    }

    private void SwitchShader(Shader targetShader)
    {
        ClickedMaterial.shader = targetShader;
    }
}