using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helps : MonoBehaviour
{
    private string m_HelpDocumentPath = "UserManual.docx";

    void Start()
    {
        
    }

    public void OpenUserManual()
    {
        Application.OpenURL(Application.streamingAssetsPath + "\\" + m_HelpDocumentPath);
    }
}
