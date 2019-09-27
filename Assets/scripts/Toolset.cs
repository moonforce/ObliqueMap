﻿using System;
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

    public void AutoMap()
    {
        MeshAnaliser.Instance.ResetChoice();
        List<MeshFilter> meshFilters = ProjectCtrl.Instance.ModelContainer.GetComponentsInChildren<MeshFilter>(true).ToList();
        meshFilters.Remove(meshFilters.Find(meshFilter => meshFilter.name == "ModelGridPlane"));
        if (meshFilters.Count <= 0)
            return;
        ProgressbarCtrl.Instance.Show("正在自动贴图……");
        ProgressbarCtrl.Instance.ResetMaxCount(meshFilters.Count);
        StartCoroutine(AutoMapMeshs(meshFilters));
    }

    IEnumerator AutoMapMeshs(List<MeshFilter> meshFilters)
    {
        yield return null; //为了显示进度条而延迟一帧
        foreach (MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = Utills.GetMeshOfMeshFilter(meshFilter);
            SubMeshInfo subMeshInfo = meshFilter.GetComponent<SubMeshInfo>();
            yield return StartCoroutine(AutoMapMesh(mesh, subMeshInfo));
            ObjExportHandler.Export(meshFilter, null);
            ProgressbarCtrl.Instance.ProgressPlusPlus();
        }
    }

    IEnumerator AutoMapMesh(Mesh mesh, SubMeshInfo subMeshInfo)
    {
        for (int i = 0; i < mesh.subMeshCount; ++i)
        {
            List<int> indecies = subMeshInfo.FaceNewIndexLists[i];
            Vector3[] allVertices = mesh.vertices;
            Dictionary<int, Vector3> subMeshVertices = new Dictionary<int, Vector3>();
            foreach (int index in indecies)
            {
                if (!subMeshVertices.ContainsKey(index))
                {
                    //注意y/z的互换
                    subMeshVertices.Add(index, new Vector3(allVertices[index].x + 0, allVertices[index].z + 0, allVertices[index].y));
                }
            }
            Vector3 faceNormal = new Vector3(mesh.normals[indecies[0]].x, mesh.normals[indecies[0]].y, mesh.normals[indecies[0]].z);
            List<ImageInfo> imageInfos = ProjectCtrl.Instance.ProjectPoints(subMeshVertices, faceNormal);
            imageInfos = imageInfos.OrderBy(it => it.DirectionDot).ToList();

            for (int j = imageInfos.Count - 1; j >= 0; j--)
            {
                //路径不同
                string path = ProjectCtrl.Instance.GetObliqueImageList().Find(o => Path.GetFileName(o) == imageInfos[j].File.Name);
                if (path == null)
                    imageInfos.Remove(imageInfos[j]);
                else
                    imageInfos[j].File = new FileInfo(path);
                //路径相同
                //if (!ProjectCtrl.Instance.ObliqueImages.Contains(imageInfos[j].File.FullName))
                //{
                //    imageInfos.Remove(imageInfos[j]);
                //}
            }
            Dictionary<int, Vector2> m_UniqueIndexUv = new Dictionary<int, Vector2>();
            foreach (Tuple<int, int> lineIndex in subMeshInfo.LineIndexLists[i])
            {
                int index1 = lineIndex.Item1;
                int index2 = lineIndex.Item2;
                Vector2 uv1 = imageInfos[0].Index_UVs[index1];
                Vector2 uv2 = imageInfos[0].Index_UVs[index2];
                if (!m_UniqueIndexUv.ContainsKey(index1))
                {
                    m_UniqueIndexUv.Add(index1, uv1);
                }
                if (!m_UniqueIndexUv.ContainsKey(index2))
                {
                    m_UniqueIndexUv.Add(index2, uv2);
                }
            }
            UV_AABB m_UV_AABB = new UV_AABB();
            foreach (var indexUv in m_UniqueIndexUv)
            {
                m_UV_AABB.UpdateAABB(indexUv.Value);
            }
            m_UV_AABB.ExpandAABB(new Vector2(imageInfos[0].Width, imageInfos[0].Height));

            Texture2D TextureDownloaded = null;
            yield return StartCoroutine(DatabaseLoaderTexture_DDS.LoadAndInvoke(Utills.ChangeExtensionToDDS(imageInfos[0].File.FullName), (texture) => { TextureDownloaded = texture; }));
            if (TextureDownloaded == null)
                continue;
            Texture2D tileTexture;
            string tileTexturePath = Path.GetDirectoryName(subMeshInfo.FilePath) + '/' + Path.GetFileNameWithoutExtension(subMeshInfo.FilePath) + '_' + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            Utills.TextureTile2ImageFile(TextureDownloaded,
                out tileTexture,
                (int)(m_UV_AABB.MinX * TextureDownloaded.width + 0.5f),
                (int)((1 - m_UV_AABB.MaxY) * TextureDownloaded.height + 0.5f),
                (int)(m_UV_AABB.Spacing.x * TextureDownloaded.width + 0.5f),
                (int)(m_UV_AABB.Spacing.y * TextureDownloaded.height + 0.5f),
                tileTexturePath
                );
            Material material = subMeshInfo.GetComponent<MeshRenderer>().sharedMaterials[i];
            if (material.mainTexture != null && material.mainTexture != MeshAnaliser.Instance.WhiteTexture2D)
            {
                Destroy(material.mainTexture);
            }
            material.name = Path.GetFileName(tileTexturePath);
            material.mainTexture = tileTexture;
            Vector2[] uvCopy = mesh.uv;
            foreach (var pair in m_UniqueIndexUv)
            {
                uvCopy[pair.Key] = new Vector2((pair.Value.x - m_UV_AABB.MinX) / m_UV_AABB.Spacing.x, (pair.Value.y - m_UV_AABB.MinY) / m_UV_AABB.Spacing.y); ;
            }
            mesh.uv = uvCopy;
            Destroy(TextureDownloaded);
            Resources.UnloadUnusedAssets();
        }
    }
}