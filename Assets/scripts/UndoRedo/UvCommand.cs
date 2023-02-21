using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UvCommand : MonoBehaviour, Command
{
    //这里的Execute和UnExecute都执行贴图，所以函数保持相同
    private UvBox m_UvBox;
    private Dictionary<int, Vector2> m_UniqueIndexUv = new Dictionary<int, Vector2>();
    private Texture2D m_Texture;
    private string m_TileTexturePath;
    private bool m_FirstCall = true;

    public UvCommand(Dictionary<int, Vector2> uniqueIndexUv, UvBox uvBox)
    {
        m_UniqueIndexUv = new Dictionary<int, Vector2>(uniqueIndexUv);
        m_UvBox = uvBox;
    }

    public void Execute()
    {
        UndoRedo();
    }

    public void UnExecute()
    {
        UndoRedo();
    }

    private void UndoRedo()
    {        
        if (m_FirstCall)
        {
            m_FirstCall = false;
            
            m_TileTexturePath = Path.GetDirectoryName(MeshAnaliser.Instance.ClickedSubMeshInfo.FilePath) + '/' + Path.GetFileNameWithoutExtension(MeshAnaliser.Instance.ClickedSubMeshInfo.FilePath) + '_' + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            Utills.TextureTile2ImageFile(TextureHandler.Instance.TextureDownloaded,
                out m_Texture,
                (int)(m_UvBox.AABB.MinX * TextureHandler.Instance.TextureDownloaded.width + 0.5f),
                (int)((1 - m_UvBox.AABB.MaxY) * TextureHandler.Instance.TextureDownloaded.height + 0.5f),
                (int)(m_UvBox.AABB.Spacing.x * TextureHandler.Instance.TextureDownloaded.width + 0.5f),
                (int)(m_UvBox.AABB.Spacing.y * TextureHandler.Instance.TextureDownloaded.height + 0.5f),
                m_TileTexturePath
                );
            SetTexture(m_Texture);
        }
        else
        {
            //更新uv点线框的位置
            TextureHandler.Instance.UpdateAllUvElementsByUniqueIndexUv(m_UniqueIndexUv);
            SetTexture(m_Texture);
        }        
    }

    private void SetTexture(Texture2D texture)
    {
        //MeshAnaliser.Instance.DestroyClickedMainTexture();
        MeshAnaliser.Instance.ClickedMaterial.name = Path.GetFileName(m_TileTexturePath);
        MeshAnaliser.Instance.ClickedMaterial.mainTexture = texture;
        MeshAnaliser.Instance.ClickedMaterial.SetColor("_Color", new Color(1, 1, 1, 1));
        Vector2[] uvCopy = MeshAnaliser.Instance.ClickedMesh.uv;
        foreach (var pair in m_UniqueIndexUv)
        {
            uvCopy[pair.Key] = Utills.ConvertGlobalUvToLocalUv(pair.Value, m_UvBox.AABB);
        }
        MeshAnaliser.Instance.ClickedMesh.uv = uvCopy;
    }

    public void ReleaseTexture()
    {
        Destroy(m_Texture);
    }
}
