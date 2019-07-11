using System.Collections;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class AutoResizeImageGallerySize : MonoBehaviour
{
    private Transform m_ImagesParent;
    private GridLayoutGroup m_GridLayoutGroup;
    private Splitter m_Splitter;
    private Scrollbar m_Scrollbar;
    private float m_StartScrollbarValue;
    [SerializeField]
    private float m_Margin = 5f;
    [SerializeField]
    private float m_WidthHeightRatio = 4f / 3f;

    private float m_LastScreenWidth = 0;
    private float m_LastScreenHeight = 0;

    void Start()
    {
        m_ImagesParent = transform.Find("ScrollView/Viewport/Content");
        m_GridLayoutGroup = m_ImagesParent.GetComponent<GridLayoutGroup>();
        m_Splitter = transform.parent.Find("HorizontalSplitter").GetComponent<Splitter>();
        m_Scrollbar = transform.Find("ScrollView/Scrollbar Horizontal").GetComponent<Scrollbar>();

        m_Splitter.OnStartResize.AddListener(StartResizeSplitter);
        m_Splitter.OnEndResize.AddListener(EndResizeSpliter);
    }

    private void StartResizeSplitter(Splitter splitter)
    {
        m_StartScrollbarValue = m_Scrollbar.value;
    }

    private void EndResizeSpliter(Splitter splitter)
    {
        UpdateLayout();
        UpdateUvLine();
        StartCoroutine(SetScrollbarValue());
    }

    public IEnumerator SetScrollbarValue()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        m_Scrollbar.value = m_StartScrollbarValue;
    }

    public void UpdateLayout()
    {
        float height = transform.GetComponent<RectTransform>().rect.height - m_Margin * 2;
        m_GridLayoutGroup.cellSize = new Vector2(height * m_WidthHeightRatio, height - 20);//20是scrollbar的高度
    }

    public void UpdateUvLine()
    {
        if (m_ImagesParent.childCount > 0)
        {
            for (int i = 0; i < m_ImagesParent.childCount; ++i)
            {
                m_ImagesParent.GetChild(i).GetComponent<ImageGalleryTexture>().UpdateUvLine(m_GridLayoutGroup.cellSize);
            }
        }
    }

    private void LateUpdate()
    {
        if (Mathf.Abs((m_LastScreenWidth - Screen.width)) > 1 || Mathf.Abs(m_LastScreenHeight - Screen.height) > 1)
        {
            m_LastScreenWidth = Screen.width;
            m_LastScreenHeight = Screen.height;
            StartCoroutine(UpdateLayoutAndUvLine());
        }
    }

    public IEnumerator UpdateLayoutAndUvLine()
    {
        yield return new WaitForEndOfFrame();
        UpdateLayout();
        UpdateUvLine();
    }
}
