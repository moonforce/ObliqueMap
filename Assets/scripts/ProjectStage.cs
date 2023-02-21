using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProjectStage : Singleton<ProjectStage>
{
    protected ProjectStage() { }

    public bool FaceChosed = false;
    public bool FaceEditting = false;
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
}
