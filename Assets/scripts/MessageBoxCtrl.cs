using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageBoxCtrl : Singleton<MessageBoxCtrl>
{
    protected MessageBoxCtrl() { }

    private TextMeshProUGUI m_Title;
    private GraphicRaycaster m_MainCanvasRaycaster;
    private OrbitCamera m_ModelViewScript;
    private Canvas m_MessageBoxCanvas;

    void Start()
    {
        m_Title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        m_MainCanvasRaycaster = GameObject.Find("MainCanvas").GetComponent<GraphicRaycaster>();
        m_ModelViewScript = Camera.main.GetComponent<OrbitCamera>();
        transform.Find("Yes").GetComponent<Button>().onClick.AddListener(Hide);
        m_MessageBoxCanvas = GetComponent<Canvas>();
        Hide();
    }

    public void Show(string title)
    {
        m_Title.text = title;
        m_MainCanvasRaycaster.enabled = false;
        m_ModelViewScript.enabled = false;
        m_MessageBoxCanvas.enabled = true;
    }

    public void Hide()
    {
        m_MainCanvasRaycaster.enabled = true;
        m_ModelViewScript.enabled = true;
        m_MessageBoxCanvas.enabled = false;
    }
}
