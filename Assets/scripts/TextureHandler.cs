using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureHandler : Singleton<TextureHandler>
{
    protected TextureHandler() { }

    public ImageController MainImageController { get; set; }
    private Texture2D m_TextureDownloaded = null;   

    [SerializeField]
    private float m_PointRadius = 5f;

    private List<UvLine> m_UvLines = new List<UvLine>();
    private List<UvPoint> m_UvPoints = new List<UvPoint>();
    private UvBox m_UvBox;    
    private Dictionary<int, Vector2> m_UniqueIndexUv = new Dictionary<int, Vector2>();

    void Start()
    {
        MainImageController = GetComponent<ImageController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //if (MeshAnaliser.Instance.Editting && m_TextureDownloaded != null)
            //{
            //    SetMaterialTexture(m_TextureDownloaded);
            //    Vector2[] uvCopy = MeshAnaliser.Instance.ClickedMesh.uv;
            //    List<Vector2> uvs = m_UvLine.points2;
            //    Vector2 textureSize = m_MainImageController.Texture.rectTransform.sizeDelta;
            //    for (int i = 0; i < m_UvPoints.Count; ++i)
            //    {
            //        uvCopy[m_UvPoints[i].GetComponent<UvPoint>().UvIndex] = new Vector2(uvs[i].x / textureSize.x, uvs[i].y / textureSize.y);
            //    }
            //    MeshAnaliser.Instance.ClickedMesh.uv = uvCopy;
            //}
        }
    }
    
    public void ResetContent()
    {
        Utills.DestroyAllChildren(transform);
        m_UvLines.Clear();
        m_UvPoints.Clear();
        MainImageController.ResetContent();
    }

    public void SendTexture(string imageUrl, List<UvLine> uvLines, Dictionary<int, Vector2> uniqueIndexUv)
    {
        ResetContent();
        foreach (UvLine galleryUvLine in uvLines)
        {
            GameObject line = Instantiate(Resources.Load<GameObject>("prefab/UvLine"));
            UvLine uvLine = line.GetComponent<UvLine>();
            uvLine.TheLine.SetCanvas(CanvasCtrl.Instance.MainCanvas);
            line.transform.SetParent(MainImageController.Texture.transform);
            //line.transform.SetSiblingIndex(0);
            line.transform.localPosition = Vector3.zero;
            line.transform.localScale = Vector3.one;
            line.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            uvLine.IndexTuple = galleryUvLine.IndexTuple;
            uvLine.UvTuple = galleryUvLine.UvTuple;
            m_UvLines.Add(uvLine);
            uvLine.gameObject.SetActive(false);
        }
        m_UniqueIndexUv = uniqueIndexUv;
        StartCoroutine(Utills.DownloadTexture(imageUrl, SetTexture));
    }

    private void SetTexture(Texture2D texture)
    {
        m_TextureDownloaded = texture;
        MainImageController.setImageTexture(m_TextureDownloaded);
        CreateLines();
        CreatePointsAndAABB();
    }

    private void SetMaterialTexture(Texture2D texture)
    {
        MeshAnaliser.Instance.ClickedMaterial.mainTexture = texture;
        MeshAnaliser.Instance.ClickedMaterial.SetColor("_Color", new Color(1, 1, 1, 1));
    }

    private void CreateLines()
    {
        Vector2 textureSize = MainImageController.Texture.rectTransform.sizeDelta;
        foreach (UvLine uvLine in m_UvLines)
        {
            uvLine.gameObject.SetActive(true);
            uvLine.CreateOrUpdateLine(textureSize, true);
        }
    }

    private void CreatePointsAndAABB()
    {
        GameObject uvBox = Instantiate(Resources.Load<GameObject>("prefab/UvBox"));
        m_UvBox = uvBox.GetComponent<UvBox>();
        m_UvBox.gameObject.SetActive(false);
        m_UvBox.transform.SetSiblingIndex(0);
        m_UvBox.transform.SetParent(MainImageController.transform);
        m_UvBox.transform.localPosition = Vector3.zero;
        m_UvBox.transform.localScale = Vector3.one;
        
        Vector2 textureRectSize = MainImageController.Texture.rectTransform.sizeDelta;
        foreach (var indexUv in m_UniqueIndexUv)
        {
            GameObject point = Instantiate(Resources.Load<GameObject>("prefab/UvPoint"));
            RectTransform point_RT = point.GetComponent<RectTransform>();
            UvPoint uvPoint = point.GetComponent<UvPoint>();
            point_RT.SetParent(MainImageController.transform);
            point_RT.localPosition = Vector3.zero;
            point_RT.localScale = Vector3.one * m_PointRadius;
            point_RT.anchoredPosition = new Vector2(indexUv.Value.x * textureRectSize.x, (indexUv.Value.y * textureRectSize.y));
            uvPoint.Index = indexUv.Key;
            m_UvPoints.Add(uvPoint);

            m_UvBox.AABB.MinX = Mathf.Min(m_UvBox.AABB.MinX, indexUv.Value.x);
            m_UvBox.AABB.MaxX = Mathf.Max(m_UvBox.AABB.MaxX, indexUv.Value.x);
            m_UvBox.AABB.MinY = Mathf.Min(m_UvBox.AABB.MinY, indexUv.Value.y);
            m_UvBox.AABB.MaxY = Mathf.Max(m_UvBox.AABB.MaxY, indexUv.Value.y);
        }
        m_UvBox.SetPosition(textureRectSize, new Vector2(m_TextureDownloaded.width, m_TextureDownloaded.height));
        m_UvBox.gameObject.SetActive(true);
    }

    public void UpdateUvByPoint(int index, Vector2 mousePos)
    {
        foreach (UvLine uvLine in m_UvLines)
        {
            if (uvLine.UpdateUvByAbsolute(index, mousePos))
            {
                uvLine.UpdateLineEverything();
            }
        }
        m_UniqueIndexUv[index] = mousePos;
    }

    public void UpdateUvByLine(Tuple<int, int> indexTuple, Vector2 deltaMousePos)
    {
        foreach (UvLine uvLine in m_UvLines)
        {
            bool update1 = uvLine.UpdateUvByRelativeForGivenIndex(indexTuple.Item1, deltaMousePos);
            bool update2 = uvLine.UpdateUvByRelativeForGivenIndex(indexTuple.Item2, deltaMousePos);
            if (update1 || update2)
            {
                uvLine.UpdateLineEverything();
            }
        }
        foreach (UvPoint uvPoint in m_UvPoints)
        {
            if (uvPoint.Index == indexTuple.Item1 || uvPoint.Index == indexTuple.Item2)
            {
                uvPoint.UpdatePositionByRelative(deltaMousePos);
            }
        }
        m_UniqueIndexUv[indexTuple.Item1] = m_UniqueIndexUv[indexTuple.Item1] + deltaMousePos;
        m_UniqueIndexUv[indexTuple.Item2] = m_UniqueIndexUv[indexTuple.Item2] + deltaMousePos;
    }

    public void UpdateUvByBox(Vector2 deltaMousePos)
    {
        foreach (UvLine uvLine in m_UvLines)
        {
            uvLine.UpdateUvByRelative(deltaMousePos);
            uvLine.UpdateLineEverything();
        }
        foreach (UvPoint uvPoint in m_UvPoints)
        {
            uvPoint.UpdatePositionByRelative(deltaMousePos);
        }
        List<int> keys = new List<int>(m_UniqueIndexUv.Keys);
        foreach (var key in keys)
        {
            m_UniqueIndexUv[key] = m_UniqueIndexUv[key] + deltaMousePos;
        }            
    }
}
