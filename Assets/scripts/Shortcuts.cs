using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Shortcuts : Singleton<Shortcuts>
{
    protected Shortcuts() { }

    public KeyCode StartEdittingKey = KeyCode.E;
    public KeyCode TexturePasteKey = KeyCode.T;
    public KeyCode CancelKey = KeyCode.C;
    public KeyCode PsOpenKey = KeyCode.F;
    public KeyCode RefreshTextureKey = KeyCode.R;
    public KeyCode OutputModelKey = KeyCode.S;
    public KeyCode SwitchImageKey = KeyCode.Space;
    public KeyCode DeleteTextureKey = KeyCode.D;
    public KeyCode UndoKey = KeyCode.Z;
    public KeyCode RedoKey = KeyCode.Y;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(StartEdittingKey))
        {
            if (ProjectStage.Instance.FaceChosed && !ProjectStage.Instance.FaceEditting)
            {
                StartEditting();
            }
        }
        else if (Input.GetKeyDown(TexturePasteKey))
        {
            if (ProjectStage.Instance.FaceEditting)
            {
                TexturePaste();
            }
        }
        else if (Input.GetKeyDown(CancelKey))
        {
            if (ProjectStage.Instance.FaceEditting)
            {
                Cancel();
            }
        }
        else if (Input.GetKeyDown(PsOpenKey))
        {
            if (ProjectStage.Instance.FaceEditting)
            {
                PS();
            }
        }
        else if (Input.GetKeyDown(RefreshTextureKey))
        {
            if (ProjectStage.Instance.FaceEditting)
            {
                RefreshTexture();
            }
        }
        else if (Input.GetKeyDown(OutputModelKey))
        {
            if (ProjectStage.Instance.FaceEditting)
            {
                OutputModel();
            }
        }
        else if (Input.GetKeyDown(SwitchImageKey))
        {
            if (ProjectStage.Instance.FaceEditting)
            {
                SwitchImage();
            }
        }
        else if (Input.GetKeyDown(DeleteTextureKey))
        {
            if (ProjectStage.Instance.FaceEditting)
            {
                DeleteTexture();
            }
        }
    }
    
    public void StartEditting()
    {
        ProjectStage.Instance.FaceEditting = true;
        MeshAnaliser.Instance.StartEditting();
    }

    public void TexturePaste()
    {
        TextureHandler.Instance.PasteTextureToModelFace();
    }

    public void FullImage()
    {
        ImageController.Instance.ViewFullImage();
    }

    public void SwitchImage()
    {
        ImageGallery.Instance.SwitchToNextImage();
    }

    public void PS()
    {
        string clickedImagePath = MeshAnaliser.Instance.GetClickedImagePath();
        if (!string.IsNullOrEmpty(clickedImagePath) && MeshAnaliser.Instance.ClickedMaterial.name != ObjExportHandler.DefaultMatName)
        {
            if (!File.Exists(SettingsPanelCtrl.Instance.PhotoshopPath))
            {
                MessageBoxCtrl.Instance.Show("PS路径不正确！");
                return;
            }
            else if (!File.Exists(clickedImagePath))
            {
                MessageBoxCtrl.Instance.Show("未找到该贴图文件！");
                return;
            }
            Process process = new Process();
            process.StartInfo.FileName = SettingsPanelCtrl.Instance.PhotoshopPath;
            process.StartInfo.Arguments = clickedImagePath;
            process.Start();
        }
    }

    public void RefreshTexture()
    {
        string clickedImagePath = MeshAnaliser.Instance.GetClickedImagePath();
        if (!string.IsNullOrEmpty(clickedImagePath) && MeshAnaliser.Instance.ClickedMaterial.name != ObjExportHandler.DefaultMatName)
        {
            StartCoroutine(DownloadTexture(clickedImagePath, MeshAnaliser.Instance.ClickedSubMeshIndex));
        }
    }

    public void DeleteTexture()
    {
        string clickedImagePath = MeshAnaliser.Instance.GetClickedImagePath();
        if (!string.IsNullOrEmpty(clickedImagePath) && MeshAnaliser.Instance.ClickedMaterial.name != ObjExportHandler.DefaultMatName)
        {
            MeshAnaliser.Instance.DestroyClickedMainTexture(true);
        }
    }

    public void ReplaceModel()
    {
        OrbitCamera.Instance.ReplaceModel();
    }

    public void OutputModel()
    {
        ObjExportHandler.Export(ObliqueMapTreeView.CurrentGameObject.GetComponentsInChildren<MeshFilter>(), null);
    }

    public void Cancel()
    {
        MeshAnaliser.Instance.ResetChoice();
        OrbitCamera.Instance.ReplaceModel();
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
