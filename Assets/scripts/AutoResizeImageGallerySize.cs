using UnityEngine;
using UnityEngine.UI;

public class AutoResizeImageGallerySize : MonoBehaviour
{
    private Transform m_ImagesParent;
    private GridLayoutGroup m_GridLayoutGroup;
    [SerializeField]
    private float m_Margin = 5f;
    [SerializeField]
    private float m_WidthHeightRatio = 4f / 3f;

    void Start()
    {
        m_ImagesParent = transform.Find("ScrollView/Viewport/Content");
        m_GridLayoutGroup = m_ImagesParent.GetComponent<GridLayoutGroup>();
        Invoke("OnRectTransformDimensionsChange", 0.1f);
    }

    void OnRectTransformDimensionsChange()
    {
        if (m_ImagesParent == null || m_GridLayoutGroup == null)
            return;
        float height = transform.GetComponent<RectTransform>().rect.height - m_Margin * 2;
        m_GridLayoutGroup.cellSize = new Vector2(height * m_WidthHeightRatio, height - 20);//20是scrollbar的高度

        if (m_ImagesParent.childCount > 0)
        {
            for (int i = 0; i< m_ImagesParent.childCount; ++i)
            {
                m_ImagesParent.GetChild(i).GetComponent<ImageGalleryTexture>().UpdateUvLine(m_GridLayoutGroup.cellSize);
            }
        }
    }
}
