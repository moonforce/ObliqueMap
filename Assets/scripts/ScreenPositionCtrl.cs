using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ScreenPositionCtrl : MonoBehaviour
{
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll", EntryPoint = "ShowWindow")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    const int SW_SHOWMINIMIZED = 2; //｛最小化, 激活｝  
    const int SW_SHOWMAXIMIZED = 3;//最大化  
    const int SW_SHOWRESTORE = 1;//还原  

    void Start()
    {
        int srceenWidth = PlayerPrefs.GetInt("Screenmanager Resolution Width", 0);
        if (srceenWidth == Screen.currentResolution.width)
            ShowWindow(FindWindow(null, "ObliqueMap"), SW_SHOWMAXIMIZED);
    }
}
