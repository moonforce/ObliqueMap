using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasCtrl : Singleton<CanvasCtrl>
{
    protected CanvasCtrl() { }
    public bool IsMainImageDragging { get; set; } = false;
    public bool IsGalleryDragging { get; set; } = false;
    public bool IsUvLineDragging { get; set; } = false;
    public Canvas MainCanvas { get; set; }

    void Awake()
    {
        MainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
    }

    void Start()
    {

    }
}
