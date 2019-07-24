using UnityEngine;
using UnityEngine.EventSystems;
using Battlehub.UIControls.MenuControl;

public class ContextMenuHandler : MonoBehaviour
{
    [SerializeField]
    private Menu m_ImagePanelContextMenu = null;
    [SerializeField]
    private Menu m_ModelPanelContextMenu = null;

    private Canvas m_MainCanvas;

    private delegate void CheckeButtonState(Menu menu);

    private void Start()
    {
        m_MainCanvas = CanvasCtrl.Instance.MainCanvas;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            // ImagePanelContextMenu的判断
            if (CheckContextMenu(m_ImagePanelContextMenu, ImagePanelContextMenuCheckButtonState))
            {
            }
            // ModelPanelContextMenu的判断
            else if (CheckContextMenu(m_ModelPanelContextMenu, ModelPanelContextMenuCheckButtonState))
            {
            }
        }
    }

    private bool CheckContextMenu(Menu menu, CheckeButtonState checkeButtonState)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(menu.transform.parent.GetComponent<RectTransform>(), Input.mousePosition))
        {
            Vector3 worldPosition;
            Vector2 mousePosition = Input.mousePosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)transform, mousePosition, m_MainCanvas.worldCamera, out worldPosition))
            {
                checkeButtonState(menu);
                menu.transform.position = worldPosition;
                menu.Open();
                return true;
            }
        }
        return false;
    }

    void ImagePanelContextMenuCheckButtonState(Menu menu)
    {
        // 0:PS中打开，TextureHandler存在图片、MeshAnalizer编辑中
        MenuItemInfo[] items = menu.Items;
        if (!(ImageController.Instance.HaveImage && MeshAnaliser.Instance.Editting))
            items[0].Command = "DisabledCmd";
        else
            items[0].Command = "Paste";
    }

    void ModelPanelContextMenuCheckButtonState(Menu menu)
    {
        // 0:PS中打开，TextureHandler存在图片、MeshAnalizer编辑中
        MenuItemInfo[] items = menu.Items;
        items[0].Command = "DisabledCmd";
        items[1].Command = "DisabledCmd";
    }

    public void OnValidateCmd(MenuItemValidationArgs args)
    {
        if (args.Command == "DisabledCmd")
        {
            args.IsValid = false;
        }
    }

    public void OnCmd(string cmd)
    {
        if (cmd == "Paste")
        {
            TextureHandler.Instance.PasteTexture();
        }
    }
}
