using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CanvasCtrl : Singleton<CanvasCtrl>
{
    protected CanvasCtrl() { }
    public bool IsMainImageDragging = false;
    public bool IsGalleryDragging = false;
    public bool IsUvLineDragging = false;
    public Canvas MainCanvas { get; set; }

    void Awake()
    {
        MainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
        }
    }
}
