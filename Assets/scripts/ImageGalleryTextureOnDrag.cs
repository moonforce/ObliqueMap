using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageGalleryTextureOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void DragEvent();
    public event DragEvent OnBeginDragCallBack = null;
    public event DragEvent OnEndDragCallBack = null;

    private GameObject m_DraggingTextureGameObject;
    private RectTransform m_DraggingPlane;
    [SerializeField]
    private bool m_IsVertical = false; 
    public ScrollRect ScrollRect { get; set; }
    public bool IsSelf { get; set; } = false;

    void Start()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 touchDeltaPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (m_IsVertical)
        {
            if (Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
            {
                IsSelf = true;
                m_DraggingTextureGameObject = CreateTexture();
                m_DraggingTextureGameObject.transform.SetAsLastSibling();
                m_DraggingTextureGameObject.AddComponent<IgnoreRayCast>();
                m_DraggingPlane = transform as RectTransform;

                SetDraggedPosition(eventData);
                if (OnBeginDragCallBack != null)
                {
                    OnBeginDragCallBack();
                }
            }
            else
            {
                IsSelf = false;
                if (ScrollRect != null)
                    ScrollRect.OnBeginDrag(eventData);
            }
        }
        else
        {
            if (Mathf.Abs(touchDeltaPosition.x) < Mathf.Abs(touchDeltaPosition.y))
            {
                IsSelf = true;
                m_DraggingTextureGameObject = CreateTexture();
                m_DraggingTextureGameObject.transform.SetAsLastSibling();
                m_DraggingTextureGameObject.AddComponent<IgnoreRayCast>();
                m_DraggingPlane = transform as RectTransform;

                SetDraggedPosition(eventData);
                if (OnBeginDragCallBack != null)
                {
                    OnBeginDragCallBack();
                }
            }
            else
            {
                IsSelf = false;
                if (ScrollRect != null)
                    ScrollRect.OnBeginDrag(eventData);
            }
        }
        ProjectStage.Instance.IsGalleryDragging = true;
    }

    public void OnDrag(PointerEventData data)
    {
        if (IsSelf)
        {
            if (m_DraggingTextureGameObject != null)
            {
                SetDraggedPosition(data);
            }
        }
        else
        {
            if (ScrollRect != null)
                ScrollRect.OnDrag(data);
        }
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingTextureGameObject.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    private GameObject CreateTexture()
    {
        m_DraggingTextureGameObject = Instantiate(Resources.Load<GameObject>("prefab/ImageGalleryTexture"));
        ImageGalleryTexture imageGalleryTexture = m_DraggingTextureGameObject.GetComponent<ImageGalleryTexture>();
        m_DraggingTextureGameObject.transform.parent = ProjectStage.Instance.transform;
        m_DraggingTextureGameObject.transform.localScale = Vector3.one;
        imageGalleryTexture.Texture.texture = GetComponent<RawImage>().texture;

        return m_DraggingTextureGameObject;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsSelf && m_DraggingTextureGameObject)
        {
            Destroy(m_DraggingTextureGameObject);
            if (OnEndDragCallBack != null)
            {
                OnEndDragCallBack();
            }
        }
        else
        {
            if (ScrollRect != null)
            {
                ScrollRect.OnEndDrag(eventData);
                ScrollRect.StopMovement();
            }
        }
        ProjectStage.Instance.IsGalleryDragging = false;
    }
}