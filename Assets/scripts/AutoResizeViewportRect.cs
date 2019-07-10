using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoResizeViewportRect : MonoBehaviour
{
    private Camera m_MainCamera;
    private RectTransform m_MainCanvasRectTransform;
    private Vector2 m_Resolution;

    private void Start()
    {
        m_MainCamera = Camera.main;
        m_MainCanvasRectTransform = GameObject.Find("MainCanvas").GetComponent<RectTransform>();
        Invoke("OnRectTransformDimensionsChange", 0.1f);
    }

    private void FixedUpdate()
    {
        if (m_Resolution.x != Screen.width || m_Resolution.y != Screen.height)
        {
            OnRectTransformDimensionsChange();
        }
    }

    void OnRectTransformDimensionsChange()
    {
        if (!m_MainCamera)
            return;
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        //corners[0]为左下角
        corners[0] = m_MainCanvasRectTransform.InverseTransformPoint(corners[0]);        
        float totalWidth = m_MainCanvasRectTransform.rect.width;
        float totalHeight = m_MainCanvasRectTransform.rect.height;
        float x = 0.5f + corners[0].x / totalWidth;
        float y = 0.5f + corners[0].y / totalHeight;
        m_MainCamera.rect = new Rect(x, y, 1, 1);
    }
}
