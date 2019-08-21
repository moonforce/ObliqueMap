using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using Battlehub.UIControls.MenuControl;
using System.Diagnostics;
using System.Collections;
using UnityEngine.Networking;

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
        m_MainCanvas = ProjectStage.Instance.MainCanvas;
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
        // TextureHandler存在图片、MeshAnalizer编辑中
        MenuItemInfo[] items = menu.Items;
        foreach (var item in items)
        {
            item.Command = "DisabledCmd";
        }
        if (ProjectStage.Instance.FaceChosed && !ProjectStage.Instance.FaceEditting)
        {
            items[0].Command = "StartEditting";
        }
        if (ProjectStage.Instance.FaceEditting)
        {
            items[1].Command = "TexturePaste";
            items[3].Command = "SwitchImage";
            items[4].Command = "Cancel";
        }
        if (ImageController.Instance.HaveImage)
        {
            items[2].Command = "FullImage";
        }
    }

    void ModelPanelContextMenuCheckButtonState(Menu menu)
    {
        MenuItemInfo[] items = menu.Items;
        foreach (var item in items)
        {
            item.Command = "DisabledCmd";
        }
        if (ProjectStage.Instance.FaceChosed && !ProjectStage.Instance.FaceEditting)
        {
            items[0].Command = "StartEditting";
        }
        if (ProjectStage.Instance.FaceEditting)
        {
            string clickedImagePath = MeshAnaliser.Instance.GetClickedImagePath();
            if (!string.IsNullOrEmpty(clickedImagePath) && MeshAnaliser.Instance.ClickedMaterial.name != ObjExportHandler.DefaultMatName)
            {
                items[1].Command = "PS";
                items[2].Command = "RefreshTexture";
                items[3].Command = "DeleteTexture";
            }                
        }
        if (ObliqueMapTreeView.CurrentGameObject)
        {
            items[4].Command = "ReplaceModel";
        }
        if (ProjectStage.Instance.FaceEditting)
        {
            items[5].Command = "OutputModel";
            items[6].Command = "Cancel";
        }
        // 可以PS和刷新非选中面的方法
        //int clickedSubmeshIndex = MeshAnaliser.Instance.GetClickedSubmeshIndex();
        //// 选中某个面并且此面材质含图片
        //if (clickedSubmeshIndex != -1)
        //{
        //    string clickedImagePath = MeshAnaliser.Instance.GetClickedImagePath();
        //    if (!string.IsNullOrEmpty(clickedImagePath) && clickedImagePath != ObjExportHandler.DefaultMatName)
        //    {
        //        items[0].Command = "PS|" + clickedImagePath;
        //        items[1].Command = "Refresh|" + clickedImagePath + '|' + clickedSubmeshIndex.ToString();
        //    }
        //}
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
        if (cmd == "StartEditting")
        {
            Shortcuts.Instance.StartEditting();
        }
        else if (cmd == "TexturePaste")
        {
            Shortcuts.Instance.TexturePaste();
        }
        else if (cmd == "FullImage")
        {
            Shortcuts.Instance.FullImage();
        }
        else if (cmd == "SwitchImage")
        {
            Shortcuts.Instance.SwitchImage();
        }        
        else if (cmd.StartsWith("PS"))
        {
            Shortcuts.Instance.PS();
        }
        else if (cmd.StartsWith("RefreshTexture"))
        {
            Shortcuts.Instance.RefreshTexture();
        }
        else if (cmd.StartsWith("DeleteTexture"))
        {
            Shortcuts.Instance.DeleteTexture();
        }
        else if (cmd == "ReplaceModel")
        {
            Shortcuts.Instance.ReplaceModel();
        }
        else if (cmd == "OutputModel")
        {
            Shortcuts.Instance.OutputModel();
        }
        else if (cmd == "Cancel")
        {
            Shortcuts.Instance.Cancel();
        }       
    }

    public static IEnumerator DownloadTexture(string imagePath, int index)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imagePath))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                UnityEngine.Debug.Log(www.error);
            }
            else
            {
                ObliqueMapTreeView.CurrentGameObject.GetComponentInChildren<MeshRenderer>().sharedMaterials[index].mainTexture = DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}
