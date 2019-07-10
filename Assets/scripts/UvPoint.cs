using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Vectrosity;

public class UvPoint : MonoBehaviour, IDragHandler, IPointerExitHandler, IPointerEnterHandler, IBeginDragHandler, IEndDragHandler
{
    RectTransform m_RT;
    UICircle m_Circle;
    bool m_IsDragging = false;
    public int Index { get; set; }

    void Start()
    {
        m_RT = GetComponent<RectTransform>();
        m_Circle = GetComponent<UICircle>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_RT.position = globalMousePos;
            TextureHandler.Instance.UpdateUvByPoint(Index, m_RT.anchoredPosition);
        }     
    }

    public void UpdatePositionByRelative(Vector2 deltaMousePos)
    {
        m_RT.anchoredPosition += deltaMousePos;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!m_IsDragging)
            m_Circle.color = Color.yellow;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Circle.color = Color.red;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_IsDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_IsDragging = false;
        m_Circle.color = Color.yellow;
    }
}
