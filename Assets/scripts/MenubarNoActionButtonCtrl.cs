using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenubarNoActionButtonCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool m_IsPointerIn = false;

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OnPointerExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
