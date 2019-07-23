using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Crosstales.FB;

public class SettingsPanelCtrl : MonoBehaviour
{
    private TMP_InputField m_PhotoshopInputField;

    void Start()
    {
        m_PhotoshopInputField = transform.Find("Photoshop/InputField").GetComponent<TMP_InputField>();
        transform.Find("Photoshop/SetButton").GetComponent<Button>().onClick.AddListener(SetPhotoshopPath);
    }

    void SetPhotoshopPath()
    {
        string folderPath = FileBrowser.OpenSingleFile("选择Photoshop路径", null, "exe");
        if (folderPath.Length == 0)
            return;
        m_PhotoshopInputField.text = folderPath;
    }

    public void OpenSettingsPanel()
    {
        gameObject.SetActive(true);
    }
}
