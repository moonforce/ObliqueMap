using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureHandler : Singleton<TextureHandler>
{
    protected TextureHandler() { }

    public Texture2D TextureDownloaded { get; set; } = null;

    [SerializeField]
    private float m_PointRadius = 5f;

    private List<UvLine> m_UvLines = new List<UvLine>();
    private List<UvPoint> m_UvPoints = new List<UvPoint>();
    private UvBox m_UvBox;
    private Dictionary<int, Vector2> m_UniqueIndexUv = new Dictionary<int, Vector2>();

    void Start()
    {
        
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
            //    Vector2 textureSize = m_ImageController.Instance.Texture.rectTransform.sizeDelta;
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
        ImageController.Instance.ResetContent();
    }

    public void SendTexture(string imageUrl, List<UvLine> uvLines, Dictionary<int, Vector2> uniqueIndexUv)
    {
        ResetContent();
        foreach (UvLine galleryUvLine in uvLines)
        {
            GameObject line = Instantiate(Resources.Load<GameObject>("prefab/UvLine"));
            UvLine uvLine = line.GetComponent<UvLine>();
            uvLine.TheLine.SetCanvas(CanvasCtrl.Instance.MainCanvas);
            line.transform.SetParent(ImageController.Instance.Texture.transform);
            //line.transform.SetSiblingIndex(0);
            line.transform.localPosition = Vector3.zero;
            line.transform.localScale = Vector3.one;
            line.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            uvLine.IndexTuple = galleryUvLine.IndexTuple;
            //因为uv可能在编辑过程中更改，所以在此要使用uv字典的uv值，此时UvTuple的值已经不准确了
            uvLine.UvTuple = new Tuple<Vector2, Vector2>(uniqueIndexUv[galleryUvLine.IndexTuple.Item1], uniqueIndexUv[galleryUvLine.IndexTuple.Item2]);
            m_UvLines.Add(uvLine);
            uvLine.gameObject.SetActive(false);
        }
        m_UniqueIndexUv = uniqueIndexUv;
        StartCoroutine(Utills.DownloadTexture(imageUrl, SetTexture));
    }

    private void SetTexture(Texture2D texture)
    {
        TextureDownloaded = texture;
        ImageController.Instance.setImageTexture(TextureDownloaded);
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
        Vector2 textureSize = ImageController.Instance.Texture.rectTransform.sizeDelta;
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
        m_UvBox.transform.SetParent(ImageController.Instance.transform);
        m_UvBox.transform.SetSiblingIndex(0);        
        m_UvBox.transform.localPosition = Vector3.zero;
        m_UvBox.transform.localScale = Vector3.one;

        Vector2 textureRectSize = ImageController.Instance.Texture.rectTransform.sizeDelta;
        foreach (var indexUv in m_UniqueIndexUv)
        {
            GameObject point = Instantiate(Resources.Load<GameObject>("prefab/UvPoint"));
            RectTransform point_RT = point.GetComponent<RectTransform>();
            UvPoint uvPoint = point.GetComponent<UvPoint>();
            point_RT.SetParent(ImageController.Instance.transform);
            point_RT.localPosition = Vector3.zero;
            point_RT.localScale = Vector3.one * m_PointRadius;
            point_RT.anchoredPosition = new Vector2(indexUv.Value.x * textureRectSize.x, (indexUv.Value.y * textureRectSize.y));
            uvPoint.Index = indexUv.Key;
            m_UvPoints.Add(uvPoint);

            m_UvBox.UpdateAABB(indexUv.Value);
        }
        m_UvBox.SetPosition();
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
        Vector2 uv = ConvertToUv(mousePos);
        m_UniqueIndexUv[index] = uv;
        UpdateUvBox();
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
        Vector2 uvDelta = ConvertToUv(deltaMousePos);
        m_UniqueIndexUv[indexTuple.Item1] = m_UniqueIndexUv[indexTuple.Item1] + uvDelta;
        m_UniqueIndexUv[indexTuple.Item2] = m_UniqueIndexUv[indexTuple.Item2] + uvDelta;
        UpdateUvBox();
    }

    public void UpdateUvBox()
    {
        m_UvBox.Refresh();
        foreach (var indexUv in m_UniqueIndexUv)
        {
            m_UvBox.UpdateAABB(indexUv.Value);
        }
        m_UvBox.SetPosition();
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
        Vector2 uvDelta = ConvertToUv(deltaMousePos);
        foreach (var key in keys)
        {
            m_UniqueIndexUv[key] = m_UniqueIndexUv[key] + uvDelta;
        }
    }

    public Vector2 ConvertToUv(Vector2 rectPos)
    {
        return new Vector2(rectPos.x / ImageController.Instance.Texture.rectTransform.sizeDelta.x, rectPos.y / ImageController.Instance.Texture.rectTransform.sizeDelta.y);
    }

    public Vector2 ConvertToRelativeScale(Vector2 deltaVector2)
    {
        return new Vector2(deltaVector2.x / transform.lossyScale.x, deltaVector2.y / transform.lossyScale.y);
    }
}
