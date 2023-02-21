using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenubarButtonGroupManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private bool m_IsPointerIn = false;
    [SerializeField]
    private bool m_IsClicked = false;
    [SerializeField]
    private Color m_NormalColor = new Color(214 / 255f, 219 / 255f, 233 / 255f);
    [SerializeField]
    private Color m_ChooseColor = new Color(163 / 255f, 233 / 255f, 255 / 255f);

    private GameObject m_ActiveButton;

    public Color ChooseColor { get => m_ChooseColor; set => m_ChooseColor = value; }

    public Color NormalColor { get => m_NormalColor; set => m_NormalColor = value; }

    public bool IsClicked { get => m_IsClicked; set => m_IsClicked = value; }

    public bool IsPointerIn { get => m_IsPointerIn; set => m_IsPointerIn = value; }

    public GameObject ActiveButton
    {
        get
        {
            return m_ActiveButton;
        }

        set
        {
            m_ActiveButton = value;
        }
    }   

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (m_IsClicked && !m_IsPointerIn)
            {
                m_IsClicked = false;
                m_ActiveButton.transform.GetChild(0).gameObject.SetActive(false);
                m_ActiveButton.GetComponent<Image>().color = m_NormalColor;
            }
        }
    }

    public void MenubarButtonOnPointerClick()
    {
        if (m_IsClicked)
        {
            m_ActiveButton.transform.GetChild(0).gameObject.SetActive(false);
            m_ActiveButton.GetComponent<Image>().color = m_NormalColor;
            foreach (var mnabc in m_ActiveButton.transform.GetChild(0).GetComponentsInChildren<MenubarNoActionButtonCtrl>())
            {
                mnabc.OnPointerExit();
            }
            m_IsClicked = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_IsPointerIn = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_IsPointerIn = true;
    }

    public void ChangeActiveButtonField(MenubarFirstClassButtonCtrl ctrl)
    {
        MenubarFirstClassButtonCtrl[] m_MenubarFirstClassButtonCtrlList = gameObject.GetComponentsInChildren<MenubarFirstClassButtonCtrl>();
        foreach (var mbcbcl in m_MenubarFirstClassButtonCtrlList)
        {
            if (mbcbcl != ctrl)
            {
                mbcbcl.transform.GetChild(0).gameObject.SetActive(false);
                mbcbcl.GetComponent<Image>().color = m_NormalColor;
            }
            else
            {
                mbcbcl.transform.GetChild(0).gameObject.SetActive(true);
                m_ActiveButton = mbcbcl.gameObject;
            }
        }
    }
}
