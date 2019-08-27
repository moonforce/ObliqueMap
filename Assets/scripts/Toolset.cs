using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Toolset : MonoBehaviour
{
    void Start()
    {
        
    }

    public void ClearUselessTextures()
    {
        foreach (MeshRenderer meshRenderer in ProjectCtrl.Instance.ModelContainer.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (meshRenderer.name == "ModelGridPlane")
                continue;
            SubMeshInfo subMeshInfo = meshRenderer.GetComponent<SubMeshInfo>();
            string prefix = Path.GetFileNameWithoutExtension(subMeshInfo.name);
            string postfix = "*.jpg";
            List<string> imageFiles = Directory.GetFiles(Path.GetDirectoryName(subMeshInfo.FilePath), prefix + postfix).ToList();
            // 如果引用的是materials，会给所有的材质名增加(Instanced)后缀
            foreach (Material material in meshRenderer.sharedMaterials)
            {
                string matImagePath = Path.GetDirectoryName(subMeshInfo.FilePath) + '\\' + material.name;
                imageFiles.Remove(matImagePath);
            }
            foreach (string imageFile in imageFiles)
            {
                File.Delete(imageFile);
            }
        }
    }

    public void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }

    public void ClearCameraHandlers()
    {
        ProjectCtrl.Instance.CameraHandlers.Clear();
        ProjectCtrl.Instance.ModifyProjectPath();
    }
}
