using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjLoaderLY
{
    public struct FaceIndices
    {
        public int vertIdx;
        public int uvIdx;
        public int normIdx;
    }

    public class DataSetLY
    {
        public List<FaceIndices> Faces { get; set; } = new List<FaceIndices>();
        public List<Vector3> VertList { get; set; } = new List<Vector3>();
        public List<Vector2> UvList { get; set; } = new List<Vector2>();
        public List<Vector3> NormalList { get; set; } = new List<Vector3>();

        public static string GetFaceIndicesKey(FaceIndices fi)
        {
            return fi.vertIdx.ToString() + "/" + fi.uvIdx.ToString() + "/" + fi.normIdx.ToString();
        }

        public void AddVertex(Vector3 vertex)
        {
            VertList.Add(vertex);
        }

        public void AddUV(Vector2 uv)
        {
            UvList.Add(uv);
        }

        public void AddNormal(Vector3 normal)
        {
            NormalList.Add(normal);
        }

        public void AddFaceIndices(FaceIndices faceIdx)
        {
            Faces.Add(faceIdx);
        }
    }
}