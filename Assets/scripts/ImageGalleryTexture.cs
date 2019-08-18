using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Vectrosity;
using System.Linq;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Networking;

public class ImageGalleryTexture : MonoBehaviour, IPointerClickHandler
{
    public RawImage Texture { get; set; }
    public int SiblingIndexInGallery { get; set; }
    private List<UvLine> m_UvLines = new List<UvLine>();
    private Dictionary<int, Vector2> m_UniqueIndexUv = new Dictionary<int, Vector2>();

    void Start()
    {
        Texture = GetComponent<RawImage>();
    }

    public void InitContent(string imageUrl, List<Tuple<int, int>> lineIndexList, Dictionary<int, Vector2> index_UVs, int siblingIndex)
    {
        SiblingIndexInGallery = siblingIndex;
        foreach (Tuple<int, int> lineIndex in lineIndexList)
        {
            int index1 = lineIndex.Item1;
            int index2 = lineIndex.Item2;
            Vector2 uv1 = index_UVs[index1];
            Vector2 uv2 = index_UVs[index2];
            if (!m_UniqueIndexUv.ContainsKey(index1))
            {
                m_UniqueIndexUv.Add(index1, uv1);
            }
            if (!m_UniqueIndexUv.ContainsKey(index2))
            {
                m_UniqueIndexUv.Add(index2, uv2);
            }
            GameObject line = Instantiate(Resources.Load<GameObject>("prefab/UvLineGallery"));
            UvLine uvLine = line.GetComponent<UvLine>();
            uvLine.TheLine.SetCanvas(ProjectStage.Instance.MainCanvas);
            line.transform.SetParent(transform);
            //line.transform.SetSiblingIndex(0);
            line.transform.localPosition = Vector3.zero;
            line.transform.localScale = Vector3.one;
            line.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            uvLine.IndexTuple = lineIndex;
            uvLine.UvTuple = new Tuple<Vector2, Vector2>(uv1, uv2);
            m_UvLines.Add(uvLine);
            uvLine.gameObject.SetActive(false);
        }
        StartCoroutine(Utills.DownloadTexture(imageUrl, SetTexture));
    }

    private void SetTexture(Texture2D texture)
    {
        Texture.texture = texture;
        CreateUv();
        if (SiblingIndexInGallery == 0)
        {
            SendTexture();
        }
    }

    private void CreateUv()
    {
        foreach (UvLine uvLine in m_UvLines)
        {
            uvLine.gameObject.SetActive(true);
            uvLine.CreateOrUpdateLine(new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height), true);
        }
    }

    public void UpdateUvLine(Vector2 cellsize)
    {
        foreach (UvLine uvLine in m_UvLines)
        {
            uvLine.CreateOrUpdateLine(cellsize);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            SendTexture();
        }
    }

    private void SendTexture()
    {
        TextureHandler.Instance.ReceiveTexture(gameObject.name, m_UvLines, m_UniqueIndexUv);
    }
}