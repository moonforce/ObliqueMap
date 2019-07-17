using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Vectrosity;

public class UvBox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 m_LastMousePos;    
    [SerializeField]
    private int m_MaterialExpandPixels = 3;

    //存储当前拖拽图片的RectTransform组件
    private RectTransform m_RT;

    public AABB AABB { get; set; } = new AABB();

    void Awake()
    {
        //初始化
        m_RT = gameObject.GetComponent<RectTransform>();
    }

    public void Refresh()
    {
        gameObject.SetActive(false);
        AABB.Reset();
    }

    public void SetPosition(Vector2 textureRectSize, Vector2 textureSize)
    {
        m_RT.anchoredPosition = new Vector2((AABB.MinX - m_MaterialExpandPixels / textureSize.x) * textureRectSize.x, (AABB.MinY - m_MaterialExpandPixels / textureSize.y) * textureRectSize.y);
        m_RT.sizeDelta = new Vector2((AABB.MaxX - AABB.MinX + m_MaterialExpandPixels * 2 / textureSize.x) * textureRectSize.x, (AABB.MaxY - AABB.MinY + m_MaterialExpandPixels * 2 / textureSize.y) * textureRectSize.y);
    }

    //开始拖拽触发
    //When using a mouse the pointerId returns -1, -2, or -3. These are the left, right and center mouse buttons respectively.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != -1 || CanvasCtrl.Instance.IsUvLineDragging)
            return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.pressEventCamera, out m_LastMousePos);

        SetDraggedPosition(eventData);
    }

    //拖拽过程中触发
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != -1 || CanvasCtrl.Instance.IsUvLineDragging)
            return;
        SetDraggedPosition(eventData);
    }

    //结束拖拽触发
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != -1 || CanvasCtrl.Instance.IsUvLineDragging)
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
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            //设置位置及偏移量
            Vector3 deltaMousePos = globalMousePos - m_LastMousePos;
            m_LastMousePos = globalMousePos;
            m_RT.position = deltaMousePos + m_RT.position;
            Vector2 deltaVector2 = new Vector2(deltaMousePos.x / transform.parent.lossyScale.x, deltaMousePos.y / transform.parent.lossyScale.y);
            TextureHandler.Instance.UpdateUvByBox(deltaVector2);
        }
    }
}