using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Crosstales.FB;

public class SettingsPanelCtrl : Singleton<SettingsPanelCtrl>
{
    protected SettingsPanelCtrl() { }

    private TMP_InputField m_PhotoshopInputField;
    public string PhotoshopPath { get; set; }

    private CanvasGroup m_CanvasGroup;

    void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        transform.Find("Close").GetComponent<Button>().onClick.AddListener(Close);
        m_PhotoshopInputField = transform.Find("Photoshop/InputField").GetComponent<TMP_InputField>();
        PhotoshopPath = m_PhotoshopInputField.text;
        transform.Find("Photoshop/SetButton").GetComponent<Button>().onClick.AddListener(SetPhotoshopPath);
    }

    void Close()
    {
        Utills.DisableCanvasGroup(m_CanvasGroup);
    }

    void SetPhotoshopPath()
    {
        string folderPath = FileBrowser.OpenSingleFile("选择Photoshop路径", null, "exe");
        if (folderPath.Length == 0)
            return;
        m_PhotoshopInputField.text = folderPath;
        PhotoshopPath = folderPath;
    }

    public void OpenSettingsPanel()
    {
        Utills.EnableCanvasGroup(m_CanvasGroup);
    }
}
