using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AsImpL;
using System;

public class SubMeshInfo : MonoBehaviour
{
    [System.Serializable]
    public class FaceTriangleList
    {
        public List<DataSet.FaceIndices> Face { get; set; } = new List<DataSet.FaceIndices>();

        public void AddTriangle(DataSet.FaceIndices v0, DataSet.FaceIndices v1, DataSet.FaceIndices v2)
        {
            Face.Add(v0);
            Face.Add(v1);
            Face.Add(v2);
        }
    }
    public string FilePath { get; set; }
    public bool IsCompleteUvModel { get; set; }
    //各submesh的三角形列表（存储原face字符串的各index），数量等于face数
    public List<FaceTriangleList> SubMeshLists { get; set; } = new List<FaceTriangleList>();
    //存储原face字符串（正数的），数量等于face数，即submesh数
    public List<List<string>> OrigonalFacesLists { get; set; } = new List<List<string>>();
    //存储各face，每个顶点新的index，新index为去重后的index，可从mesh中直接关联获取顶点信息，数量等于face数，即submesh数
    public List<List<int>> FaceNewIndexLists { get; set; } = new List<List<int>>();
    //各face的line对列表
    public List<List<Tuple<int, int>>> LineIndexLists { get; set; } = new List<List<Tuple<int, int>>>();
    //string为原face的位置/uv索引/法线，int为去重后的index，此index作为submesh的三角形顶点索引，以三角形列表形式传入mesh中
    public Dictionary<string, int> OrigonalFaceNewIndexDictionary { get; set; } = new Dictionary<string, int>();
    //贴图路径
    public List<string> ImagePaths { get; set; } = new List<string>();
    //Y轴最小值
    public float MinY { get; set; } = float.MaxValue;

    public void AddOrigonalFace(DataSet.FaceIndices[] faces)
    {
        List<string> origonalFace = new List<string>();
        foreach (var face in faces)
        {
            origonalFace.Add(DataSet.GetFaceIndicesKey(face));
        }
        OrigonalFacesLists.Add(origonalFace);
    }

    public void ConvertFaceNewIndicies(Dictionary<string, int> vIdxCount)
    {
        OrigonalFaceNewIndexDictionary = vIdxCount;
        for (int faceIndex = 0; faceIndex < OrigonalFacesLists.Count; ++faceIndex)
        {
            List<string> origonalFacesList = OrigonalFacesLists[faceIndex];
            List<int> faceNewIndexList = new List<int>();
            foreach (var origonalFace in origonalFacesList)
            {
                faceNewIndexList.Add(vIdxCount[origonalFace]);
            }
            FaceNewIndexLists.Add(faceNewIndexList);

            List<Tuple<int, int>> lineIndexList = new List<Tuple<int, int>>();
            for (int i = 0; i < origonalFacesList.Count; ++i)
            {
                string v1 = origonalFacesList[i];
                string v2 = origonalFacesList[i + 1 < origonalFacesList.Count ? i + 1 : 0];
                int index1 = OrigonalFaceNewIndexDictionary[v1];
                int index2 = OrigonalFaceNewIndexDictionary[v2];
                Tuple<int, int> mayDeleteTuple = new Tuple<int, int> (index2, index1);
                if (lineIndexList.Contains(mayDeleteTuple))
                {
                    lineIndexList.Remove(mayDeleteTuple);
                }
                else
                {
                    lineIndexList.Add(new Tuple<int, int>(index1, index2));
                }
            }
            LineIndexLists.Add(lineIndexList);
        }
        ImagePaths = new List<string>(new string[FaceNewIndexLists.Count]);
    }
}
