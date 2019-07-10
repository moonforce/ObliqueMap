using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MenubarFirstClassButtonCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool m_IsPointerIn = false;
    private Color m_OrigonalColor;
    private MenubarButtonGroupManager m_MenubarButtonGroupManager;

    void Start()
    {        
        m_MenubarButtonGroupManager = transform.parent.GetComponent<MenubarButtonGroupManager>();
        m_OrigonalColor = m_MenubarButtonGroupManager.NormalColor;
        GetComponent<Image>().color = m_OrigonalColor;
        transform.GetChild(0).gameObject.SetActive(false);
        foreach (var button in gameObject.GetComponentsInChildren<Button>(true))
        {
            if (button.gameObject != gameObject && !button.GetComponent<MenubarNoActionButtonCtrl>())
            {
                button.onClick.AddListener(m_MenubarButtonGroupManager.MenubarButtonOnPointerClick);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_IsPointerIn = false;
        if (!m_MenubarButtonGroupManager.IsClicked)
        {
            GetComponent<Image>().color = m_OrigonalColor;
        }            
    }

    IEnumerator CheckParentIsPointerIn()
    {
        yield return new WaitForEndOfFrame();
        if (!m_MenubarButtonGroupManager.IsPointerIn)
        {
            m_MenubarButtonGroupManager.IsClicked = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_IsPointerIn = true;
        GetComponent<Image>().color = m_MenubarButtonGroupManager.ChooseColor;
        if (m_MenubarButtonGroupManager.IsClicked)
        {
            m_MenubarButtonGroupManager.ChangeActiveButtonField(this);
        }        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_MenubarButtonGroupManager.IsClicked = !m_MenubarButtonGroupManager.IsClicked;
        if (m_MenubarButtonGroupManager.IsClicked)
        {
            m_MenubarButtonGroupManager.ActiveButton = gameObject;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            m_MenubarButtonGroupManager.ActiveButton = null;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
