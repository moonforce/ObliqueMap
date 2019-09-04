using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helps : MonoBehaviour
{
    private string m_HelpDocumentPath = Application.streamingAssetsPath + "/UserManual.docx";

    void Start()
    {
        
    }

    public void OpenUserManual()
    {
        Application.OpenURL(m_HelpDocumentPath);
    }
}
