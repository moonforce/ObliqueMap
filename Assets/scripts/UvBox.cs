using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Vectrosity;

public class UvBox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 m_LastMousePosiotion;
    [SerializeField]
    private int m_MaterialExpandPixels = 3;

    //存储当前拖拽图片的RectTransform组件
    private RectTransform m_RT;
    
    public UV_AABB AABB = new UV_AABB();

    void Awake()
    {
        //初始化
        m_RT = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {

    }

    public void Refresh()
    {
        AABB.Reset();
    }

    public void UpdateAABB(Vector2 uv)
    {
        AABB.MinX = Mathf.Min(AABB.MinX, uv.x);
        AABB.MaxX = Mathf.Max(AABB.MaxX, uv.x);
        AABB.MinY = Mathf.Min(AABB.MinY, uv.y);
        AABB.MaxY = Mathf.Max(AABB.MaxY, uv.y);
    }

    public void SetPosition()
    {
        Vector2 textureRectSize = ImageController.Instance.Texture.rectTransform.sizeDelta;
        Vector2 textureSize = new Vector2(TextureHandler.Instance.TextureDownloaded.width, TextureHandler.Instance.TextureDownloaded.height);
        AABB.MinX -= m_MaterialExpandPixels / textureSize.x;
        AABB.MinY -= m_MaterialExpandPixels / textureSize.y;
        AABB.MaxX += m_MaterialExpandPixels / textureSize.x;
        AABB.MaxY += m_MaterialExpandPixels / textureSize.y;
        m_RT.anchoredPosition = new Vector2(AABB.MinX * textureRectSize.x, AABB.MinY * textureRectSize.y);
        m_RT.sizeDelta = new Vector2((AABB.MaxX - AABB.MinX) * textureRectSize.x, (AABB.MaxY - AABB.MinY) * textureRectSize.y);
    }

    //开始拖拽触发
    //When using a mouse the pointerId returns -1, -2, or -3. These are the left, right and center mouse buttons respectively.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == -3)
        {
            ImageController.Instance.OnBeginDrag(eventData);
            return;
        }
        if (eventData.pointerId != -1 || ProjectStage.Instance.IsUvLineDragging)
            return;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.pressEventCamera, out m_LastMousePosiotion);
        SetDraggedPosition(eventData);
    }

    //拖拽过程中触发
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == -3)
        {
            ImageController.Instance.OnDrag(eventData);
            return;
        }
        if (eventData.pointerId != -1 || ProjectStage.Instance.IsUvLineDragging)
            return;
        SetDraggedPosition(eventData);
    }

    //结束拖拽触发
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == -3)
        {
            ImageController.Instance.OnEndDrag(eventData);
            return;
        }
        if (eventData.pointerId != -1 || ProjectStage.Instance.IsUvLineDragging)
            return;
        SetDraggedPosition(eventData);
    }

    /// <summary>
    /// 设置图片位置方法
    /// </summary>
    /// <param name="eventData"></param>
    private void SetDraggedPosition(PointerEventData eventData)
    {
        //UI屏幕坐标转换为世界坐标
        Vector3 globalMousePosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.pressEventCamera, out globalMousePosition))
        {
            //设置位置及偏移量
            Vector3 deltaMousePosition = globalMousePosition - m_LastMousePosiotion;
            m_LastMousePosiotion = globalMousePosition;
            m_RT.position = deltaMousePosition + m_RT.position;
            Vector2 deltaVector2 = TextureHandler.Instance.ConvertToRelativeScale(deltaMousePosition);
            TextureHandler.Instance.UpdateUvByBox(deltaVector2);
        }
    }
}