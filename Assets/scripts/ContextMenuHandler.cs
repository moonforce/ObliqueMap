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
        // TextureHandler存在图片、MeshAnalizer编辑中
        MenuItemInfo[] items = menu.Items;
        foreach (var item in items)
        {
            item.Command = "DisabledCmd";
        }
        if (MeshAnaliser.Instance.Editting && ImageController.Instance.HaveImage)
        {
            items[0].Command = "Paste";
        }
        if (ImageController.Instance.HaveImage)
        {            
            items[1].Command = "FullImage";
        }        
    }

    void ModelPanelContextMenuCheckButtonState(Menu menu)
    {
        MenuItemInfo[] items = menu.Items;
        foreach (var item in items)
        {
            item.Command = "DisabledCmd";
        }            
        if (ObliqueMapTreeView.CurrentGameObject)
        {
            items[2].Command = "Replace";
            items[3].Command = "Cancel";
            items[4].Command = "Output";
        }            
        int clickedSubmeshIndex = MeshAnaliser.Instance.GetClickedSubmeshIndex();
        // 选中某个面并且此面材质含图片
        if (clickedSubmeshIndex != -1)
        {            
            string clickedImagePath = ObliqueMapTreeView.CurrentGameObject.GetComponent<SubMeshInfo>().ImagePaths[clickedSubmeshIndex];
            if (!string.IsNullOrEmpty(clickedImagePath))
            {
                items[0].Command = "PS|" + clickedImagePath;
                items[1].Command = "Refresh|" + clickedImagePath + '|' + clickedSubmeshIndex.ToString();
            }
        }
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
        else if (cmd == "FullImage")
        {
            ImageController.Instance.ViewFullImage();
        }
        else if (cmd.StartsWith("PS"))
        {
            string imagePath = cmd.Split('|')[1];
            if (!File.Exists(SettingsPanelCtrl.Instance.PhotoshopPath))
            {
                MessageBoxCtrl.Instance.Show("PS路径不正确！");
                return;
            }
            else if (!File.Exists(imagePath))
            {
                MessageBoxCtrl.Instance.Show("未找到该贴图文件！");
                return;
            }
            Process process = new Process();
            process.StartInfo.FileName = SettingsPanelCtrl.Instance.PhotoshopPath;
            process.StartInfo.Arguments = cmd.Split('|')[1];
            process.Start();
        }
        else if (cmd.StartsWith("Refresh"))
        {
            var argvs = cmd.Split('|');
            string imagePath = argvs[1];
            int index = int.Parse(argvs[2]);
            StartCoroutine(DownloadTexture(imagePath, index));
        }
        else if (cmd == "Output")
        {
            ObjExportHandler.Export(ObliqueMapTreeView.CurrentGameObject.GetComponentsInChildren<MeshFilter>(), null);
        }
        else if (cmd == "Cancel")
        {
            MeshAnaliser.Instance.ResetChoice();
            OrbitCamera.Instance.Replace();
        }
        else if (cmd == "Replace")
        {
            OrbitCamera.Instance.Replace();
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
