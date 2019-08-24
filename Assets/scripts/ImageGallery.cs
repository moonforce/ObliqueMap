using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;

public class ImageGallery : Singleton<ImageGallery>
{
    protected ImageGallery() { }

    public Transform ImagesParent { get; set; }

    public Scrollbar HorizontalScrollbar { get; set; }

    public int CurrentTextureSibling { get; set; } = -1;

    void Start()
    {
        ImagesParent = transform.Find("ScrollView/Viewport/Content");
        HorizontalScrollbar = transform.Find("ScrollView/Scrollbar Horizontal").GetComponent<Scrollbar>();
    }

    private void ResetScrollbar()
    {
        HorizontalScrollbar.value = 0;
    }

    public void ClearContents()
    {
        for (int i = 0; i < ImagesParent.childCount; ++i)
        {
            RawImage Texture = ImagesParent.GetChild(i).GetComponent<RawImage>();
            Destroy(Texture.texture);
            Texture.texture = null;
            Destroy(ImagesParent.GetChild(i).gameObject);
        }
        ResetScrollbar();
        Resources.UnloadUnusedAssets();
    }

    public void AddImage(ImageInfo imageInfo, List<Tuple<int, int>> lineIndexList, int siblingIndex)
    {
        GameObject newImage = Instantiate(Resources.Load<GameObject>("prefab/ImageGalleryTexture"));
        FileInfo fileInfo = imageInfo.File;
        newImage.name = fileInfo.FullName;
        newImage.transform.SetParent(ImagesParent);
        newImage.transform.localPosition = new Vector3(newImage.transform.localPosition.x, newImage.transform.localPosition.y, 0);
        newImage.transform.localScale = Vector3.one;
        string imageUrl = fileInfo.DirectoryName + "/thumb/" + fileInfo.Name;
        newImage.GetComponent<ImageGalleryTexture>().InitContent(imageUrl, lineIndexList, imageInfo.Index_UVs, siblingIndex);
    }

    public void SwitchToNextImage()
    {
        if (++CurrentTextureSibling >= ImagesParent.childCount)
            CurrentTextureSibling = 0;
        ImagesParent.GetChild(CurrentTextureSibling).GetComponent<ImageGalleryTexture>().SendTextureToTextureHandler();
        //以下语句不应CurrentTextureSibling + 1，应为CurrentTextureSibling，但CurrentTextureSibling + 1能弥补误差
        HorizontalScrollbar.value = (float)CurrentTextureSibling / ImagesParent.childCount + ImagesParent.GetComponent<GridLayoutGroup>().spacing.x * (CurrentTextureSibling + 1) / ImagesParent.GetComponent<RectTransform>().sizeDelta.x;
    }
}