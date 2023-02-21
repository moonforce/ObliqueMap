using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressbarCtrl : Singleton<ProgressbarCtrl>
{
    protected ProgressbarCtrl() { }

    private int m_MaxCount = 100;
    private int m_CurrentCount = 100;
    public ProgressbarDeterminate Progressbar { get; set; }
    private TextMeshProUGUI m_Title;
    private GraphicRaycaster m_MainCanvasRaycaster;
    private OrbitCamera m_ModelViewScript;
    private Canvas m_ProgressbarCanvas;

    static object m_LockObjStatic = new object();

    void Start()
    {
        Progressbar = GetComponentInChildren<ProgressbarDeterminate>();
        m_Title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        m_MainCanvasRaycaster = GameObject.Find("MainCanvas").GetComponent<GraphicRaycaster>();
        m_ModelViewScript = Camera.main.GetComponent<OrbitCamera>();
        m_ProgressbarCanvas = GetComponent<Canvas>();
        Hide();
    }

    public void SetProgressbar(int value)
    {
        Progressbar.Value = value;
    }

    public void ResetMaxCount(int maxCount)
    {
        m_MaxCount = maxCount;
        m_CurrentCount = 0;
    }

    public bool isFinished()
    {
        return m_CurrentCount == m_MaxCount;        
    }

    public void ProgressPlusPlus()
    {
        lock (m_LockObjStatic)
        {
            ++m_CurrentCount;
            SetProgressbar((int)((m_CurrentCount) * 100f / m_MaxCount + 0.5f));
            if (m_CurrentCount == m_MaxCount)
            {
                Invoke("Hide", 0.5f);
            }
        }        
    }

    public void Show(string title)
    {
        CancelInvoke();
        Progressbar.Value = 0;
        m_Title.text = title;
        m_MainCanvasRaycaster.enabled = false;
        m_ModelViewScript.enabled = false;
        m_ProgressbarCanvas.enabled = true;
    }

    public void Hide()
    {
        m_MainCanvasRaycaster.enabled = true;
        m_ModelViewScript.enabled = true;
        m_ProgressbarCanvas.enabled = false;
    }
}
