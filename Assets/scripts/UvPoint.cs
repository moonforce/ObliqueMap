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
    [SerializeField]
    private Color m_PointNomalColor = Color.white;
    [SerializeField]
    private Color m_PointHighlightColor = Color.white;
    public int Index { get; set; }

    void Start()
    {
        m_RT = GetComponent<RectTransform>();
        m_Circle = GetComponent<UICircle>();
        m_Circle.color = m_PointNomalColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePosition;
        if (m_IsDragging && RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.pressEventCamera, out globalMousePosition))
        {
            m_RT.position = globalMousePosition;
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
            m_Circle.color = m_PointNomalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Circle.color = m_PointHighlightColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == -1)
            m_IsDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_IsDragging = false;
        m_Circle.color = m_PointNomalColor;
    }

    public void UpdatePointRadius(float radius)
    {
        m_RT.sizeDelta = new Vector2(radius, radius);
    }
}
