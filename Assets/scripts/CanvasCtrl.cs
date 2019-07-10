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
    [SerializeField]
    private bool m_IsMainImageDragging = false;
    [SerializeField]
    private bool m_IsGalleryDragging = false;
    public bool IsMainImageDragging { get; set; }
    public bool IsGalleryDragging { get; set; }
    public Canvas MainCanvas { get; set; }

    void Awake()
    {
        MainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
    }

    void Start()
    {

    }
}
