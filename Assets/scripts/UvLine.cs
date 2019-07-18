using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vectrosity;

public class UvLine : MonoBehaviour
{
    [SerializeField]
    bool m_CanDrag = false;
    public Tuple<int, int> IndexTuple { get; set; }
    public Tuple<Vector2, Vector2> UvTuple { get; set; }
    public VectorLine TheLine { get; set; }

    [SerializeField]
    private Color m_LineNomalColor = Color.white;
    [SerializeField]
    private Color m_LineHighlightColor = Color.white;
    [SerializeField]
    private float m_LineWidth = 0;
    [SerializeField]
    private float m_PointRadius = 2;
    private VectorObject2D m_LineVectorObject2D;
    private EdgeCollider2D m_EdgeCollider2D;

    private bool m_IsDragging = false;
    private Vector3 m_LastMousePosition;

    void Awake() // 创建时就调用成员变量，所以在Awake事初始化
    {
        m_LineVectorObject2D = GetComponent<VectorObject2D>();
        TheLine = m_LineVectorObject2D.vectorLine;
        if (m_CanDrag)
            m_EdgeCollider2D = GetComponent<EdgeCollider2D>();       
    }

    public void UpdateColliderPoints()
    {
        Vector2 lineUv1 = TheLine.points2[0];
        Vector2 lineUv2 = TheLine.points2[1];
        Vector2[] colliderPoints = new Vector2[2];
        colliderPoints[0] = lineUv1 + (lineUv2 - lineUv1).normalized * m_PointRadius;
        colliderPoints[1] = lineUv2 + (lineUv1 - lineUv2).normalized * m_PointRadius;
        m_EdgeCollider2D.points = colliderPoints;
    }

    public void UpdateUvTuple()
    {
        Vector2 uv1 = TextureHandler.Instance.ConvertToUv(TheLine.points2[0]);
        Vector2 uv2 = TextureHandler.Instance.ConvertToUv(TheLine.points2[1]);
        UvTuple = new Tuple<Vector2, Vector2>(uv1, uv2);
    }

    //gallery的line用到create和update，texture的line只用到了update
    public void CreateOrUpdateLine(Vector2 cellSize, bool create = false)
    {
        Vector2 uv1 = UvTuple.Item1;
        Vector2 uv2 = UvTuple.Item2;
        UVtoPoint(ref uv1, ref uv2, cellSize);
        TheLine.points2 = new List<Vector2>() { uv1, uv2 };
        if (m_CanDrag)
        {
            UpdateColliderPoints();
            if (!create)
                UpdateUvTuple();
        }            
        if (create)
        {
            TheLine.SetWidth(m_LineWidth);
        }
        TheLine.Draw();
        if (create)
        {
            TheLine.SetColor(m_LineNomalColor);
            TheLine.texture = Resources.Load("prefab/dashed") as Texture;
            TheLine.textureScale = 4;
        }
    }

    private void UVtoPoint(ref Vector2 uv1, ref Vector2 uv2, Vector2 cellsize)
    {
        uv1.x *= cellsize.x;
        uv1.y *= cellsize.y;
        uv2.x *= cellsize.x;
        uv2.y *= cellsize.y;
    }

    void Update()
    {
        if (m_CanDrag)
        {
            CheckForPointerIn();
            CheckForClicks();

            if (m_IsDragging)
            {
                Vector3 mousePosition = Input.mousePosition;

                Vector3 deltaMousePosition = mousePosition - m_LastMousePosition;
                Vector2 deltaVector2 = TextureHandler.Instance.ConvertToRelativeScale(deltaMousePosition);
                TextureHandler.Instance.UpdateUvByLine(IndexTuple, deltaVector2);

                m_LastMousePosition = mousePosition;
            }
        }
    }

    private void CheckForClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var colliderObject = Physics2D.OverlapPoint(Input.mousePosition);
            if (colliderObject == m_EdgeCollider2D)
            {
                m_IsDragging = true;
                CanvasCtrl.Instance.IsUvLineDragging = true;
                m_LastMousePosition = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (m_IsDragging)
            {
                m_IsDragging = false;
                CanvasCtrl.Instance.IsUvLineDragging = false;
            }
        }
    }

    private void CheckForPointerIn()
    {
        var colliderObject = Physics2D.OverlapPoint(Input.mousePosition);
        if (colliderObject == m_EdgeCollider2D || m_IsDragging)
        {
            TheLine.color = m_LineHighlightColor;
        }
        else
        {
            TheLine.color = m_LineNomalColor;
        }
    }

    public bool UpdateUvByRelativeForGivenIndex(int index, Vector2 delta) //某个索引发生相对移动，自身两个顶点移动，其他一个顶点移动
    {
        bool changed = false;
        if (IndexTuple.Item1 == index)
        {
            TheLine.points2[0] += delta;
            changed = true;
        }
        if (IndexTuple.Item2 == index)
        {
            TheLine.points2[1] += delta;
            changed = true;
        }
        return changed;
    }

    public void UpdateUvByRelative(Vector2 delta) //某个索引发生相对移动，自身两个顶点移动，其他一个顶点移动
    {
        TheLine.points2[0] += delta;
        TheLine.points2[1] += delta;
    }

    public bool UpdateUvByAbsolute(int index, Vector2 pos) //某个索引发生绝对移动，只可能移动一个顶点
    {
        if (IndexTuple.Item1 == index)
        {
            TheLine.points2[0] = pos;
            return true;
        }
        else if (IndexTuple.Item2 == index)
        {
            TheLine.points2[1] = pos;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateLineEverything()
    {
        TheLine.Draw();
        UpdateColliderPoints();
        UpdateUvTuple();
    }
}
