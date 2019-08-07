using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Crosstales.FB;

public class SettingsPanelCtrl : Singleton<SettingsPanelCtrl>
{
    protected SettingsPanelCtrl() { }

    public string PhotoshopPath { get; set; }
    public float DeltaX = -490000.0f;
    public float DeltaY = -3800000.0f;
    public float PointRadius = 1f;
    public float LineWidth = 0.125f;

    private TMP_InputField m_PhotoshopInputField;
    private TMP_InputField m_DeltaX_InputField;
    private TMP_InputField m_DeltaY_InputField;
    private TMP_InputField m_PointRadius_InputField;
    private TMP_InputField m_LineWidth_InputField;
    private CanvasGroup m_CanvasGroup;

    void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        transform.Find("Close").GetComponent<Button>().onClick.AddListener(Close);
        m_PhotoshopInputField = transform.Find("Photoshop/InputField").GetComponent<TMP_InputField>();
        m_PhotoshopInputField.onEndEdit.AddListener(PhotoshopInputFieldEndEidt);
        PhotoshopPath = m_PhotoshopInputField.text;
        transform.Find("Photoshop/SetButton").GetComponent<Button>().onClick.AddListener(SetPhotoshopPath);
        m_DeltaX_InputField = transform.Find("Delta_PointLine/InputField_X").GetComponent<TMP_InputField>();
        m_DeltaX_InputField.onEndEdit.AddListener(DeltaXInputFieldEndEidt);
        m_DeltaY_InputField = transform.Find("Delta_PointLine/InputField_Y").GetComponent<TMP_InputField>();
        m_DeltaY_InputField.onEndEdit.AddListener(DeltaYInputFieldEndEidt);
        m_PointRadius_InputField = transform.Find("Delta_PointLine/InputField_PointRadius").GetComponent<TMP_InputField>();
        m_PointRadius_InputField.onEndEdit.AddListener(PointRadiusInputFieldEndEidt);
        m_LineWidth_InputField = transform.Find("Delta_PointLine/InputField_LineWidth").GetComponent<TMP_InputField>();
        m_LineWidth_InputField.onEndEdit.AddListener(LineWidthInputFieldEndEidt);

        CheckPlayerPrefs();
    }

    void CheckPlayerPrefs()
    {
        PhotoshopPath = PlayerPrefs.GetString("PhotoshopPath", string.Empty);
        m_PhotoshopInputField.text = PhotoshopPath;
        DeltaX = PlayerPrefs.GetFloat("DeltaX", 0);
        m_DeltaX_InputField.text = DeltaX.ToString();
        DeltaY = PlayerPrefs.GetFloat("DeltaY", 0);
        m_DeltaY_InputField.text = DeltaY.ToString();
        PointRadius = PlayerPrefs.GetFloat("PointRadius", 0);
        m_PointRadius_InputField.text = PointRadius.ToString();
        LineWidth = PlayerPrefs.GetFloat("LineWidth", 0);
        m_LineWidth_InputField.text = LineWidth.ToString();
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
        if (!string.IsNullOrEmpty(PhotoshopPath))
        {
            PlayerPrefs.SetString("PhotoshopPath", PhotoshopPath);
        }
    }

    public void PhotoshopInputFieldEndEidt(string input)
    {
        PhotoshopPath = input;
        if (!string.IsNullOrEmpty(PhotoshopPath))
        {
            PlayerPrefs.SetString("PhotoshopPath", PhotoshopPath);
        }
    }

    public void DeltaXInputFieldEndEidt(string input)
    {
        if (float.TryParse(input, out DeltaX))
        {
            PlayerPrefs.SetFloat("DeltaX", DeltaX);
        }
    }

    public void DeltaYInputFieldEndEidt(string input)
    {
        if (float.TryParse(input, out DeltaY))
        {
            PlayerPrefs.SetFloat("DeltaY", DeltaY);
        }
    }

    public void PointRadiusInputFieldEndEidt(string input)
    {
        if (float.TryParse(input, out PointRadius))
        {
            PlayerPrefs.SetFloat("PointRadius", PointRadius);
        }
        //更新已存在的UvPoint
        foreach (var point in TextureHandler.Instance.UvPoints)
        {
            point.UpdatePointRadius(PointRadius);
        }
    }

    public void LineWidthInputFieldEndEidt(string input)
    {
        if (float.TryParse(input, out LineWidth))
        {
            PlayerPrefs.SetFloat("LineWidth", LineWidth);
        }
        //更新已存在的UvLine
        foreach (var line in TextureHandler.Instance.UvLines)
        {
            line.UpdateLineWidth(LineWidth);
        }
    }

    public void OpenSettingsPanel()
    {
        Utills.EnableCanvasGroup(m_CanvasGroup);
    }
}
