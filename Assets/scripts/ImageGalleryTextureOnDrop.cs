using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ImageGalleryTextureOnDrop : MonoBehaviour, IDropHandler
{
    private Image receivingImage;

    void Start()
    {
        receivingImage = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData data)
    {
        Sprite dropSprite = GetDropSprite(data);
        if (!dropSprite)
            return;
        //这里应改为加载原图，通过同名文件索引
        receivingImage.sprite = dropSprite;
        GetComponent<ImageController>().ConfigureContentView();
    }

    private Sprite GetDropSprite(PointerEventData data)
    {
        var originalObj = data.pointerDrag;
        if (originalObj == gameObject || originalObj == null || !originalObj.GetComponent<ImageGalleryTextureOnDrag>() || !originalObj.GetComponent<ImageGalleryTextureOnDrag>().IsSelf)
            return null;

        var srcImage = originalObj.GetComponent<Image>();
        if (srcImage == null)
            return null;

        return srcImage.sprite;
    }
}