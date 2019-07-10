using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class ImageGallery : Singleton<ImageGallery>
{
    protected ImageGallery() { }

    public Transform ImagesParent { get; set; }

    void Start()
    {
        ImagesParent = transform.Find("ScrollView/Viewport/Content");
    }

    public void AddImage(ImageInfo imageInfo, List<Tuple<int, int>> lineIndexList)
    {
        GameObject newImage = Instantiate(Resources.Load<GameObject>("prefab/ImageGalleryTexture"));
        FileInfo fileInfo = imageInfo.File;
        newImage.name = fileInfo.FullName;
        newImage.transform.SetParent(ImagesParent);
        newImage.transform.localPosition = new Vector3(newImage.transform.localPosition.x, newImage.transform.localPosition.y, 0);
        newImage.transform.localScale = Vector3.one;
        string imageUrl = fileInfo.DirectoryName + "/thumb/" + fileInfo.Name;
        newImage.GetComponent<ImageGalleryTexture>().InitContent(imageUrl, lineIndexList, imageInfo.Index_UVs);
    }
}