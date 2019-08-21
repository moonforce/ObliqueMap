using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ObjLoaderLY
{
    public class ObjImportHandler
    {
        static Shader ShaderUsed = Shader.Find("Standard");
        static Material DefaultMaterial = new Material(ShaderUsed);

        GameObject m_GameObject;
        SubMeshInfo m_SubMeshInfo;
        DataSetLY m_DataSet = new DataSetLY();
        Texture2D m_LoadedTexture;
        Dictionary<int, Material> m_Materials = new Dictionary<int, Material>();
        Dictionary<int, string> m_ImagePaths = new Dictionary<int, string>();
        //string m_mtlLib;

        public IEnumerator Load(string objName, string absolutePath, Transform parentObj, bool isCompleteUvModel)
        {
            m_GameObject = new GameObject(objName);
            m_GameObject.SetActive(false);
            m_GameObject.layer = 9;
            m_GameObject.transform.SetParent(parentObj);
            m_SubMeshInfo = m_GameObject.AddComponent<SubMeshInfo>();
            m_SubMeshInfo.IsCompleteUvModel = isCompleteUvModel;
            m_SubMeshInfo.FilePath = absolutePath;

            if (!isCompleteUvModel)
                absolutePath = Path.GetDirectoryName(absolutePath) + "/CompleteUvModel/" + Path.GetFileName(absolutePath);
            ParseObjText(absolutePath);
            //if (!string.IsNullOrEmpty(m_mtlLib))
            //{
            //    LoadMaterialLibrary(absolutePath);
            //}
            yield return Build();
            ProgressbarCtrl.Instance.ProgressPlusPlus();
        }

        private void ParseObjText(string absolutePath)
        {
            string objDataText = File.ReadAllText(absolutePath);
            string[] lines = objDataText.Split("\n".ToCharArray());
            bool isFirstInGroup = true;
            bool isFaceIndexPlus = true;
            char[] separators = new char[] { ' ', '\t' };
            int faceCount = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                // comment line
                if (line.Length > 0 && line[0] == '#')
                {
                    continue;
                }
                string[] p = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                // empty line
                if (p.Length == 0)
                {
                    continue;
                }
                string parameters = null;
                if (line.Length > p[0].Length)
                {
                    parameters = line.Substring(p[0].Length + 1).Trim();
                }
                switch (p[0])
                {
                    case "v":
                        float z = ParseFloat(p[3]);
                        m_SubMeshInfo.MinY = Mathf.Min(m_SubMeshInfo.MinY, z); //yz颠倒
                        m_DataSet.AddVertex(new Vector3(ParseFloat(p[1]), z, ParseFloat(p[2])));
                        break;
                    case "vt":
                        m_DataSet.AddUV(new Vector2(ParseFloat(p[1]), ParseFloat(p[2])));
                        break;
                    case "vn":
                        m_DataSet.AddNormal(new Vector3(ParseFloat(p[1]), ParseFloat(p[3]), ParseFloat(p[2])));
                        break;
                    case "f":
                        {
                            faceCount++;
                            int numVerts = p.Length - 1;
                            FaceIndices[] face = new FaceIndices[numVerts];
                            if (isFirstInGroup)
                            {
                                isFirstInGroup = false;
                                string[] c = p[1].Trim().Split("/".ToCharArray());
                                isFaceIndexPlus = (int.Parse(c[0]) >= 0);
                            }
                            GetFaceIndicesByOneFaceLine(face, p, isFaceIndexPlus);
                            m_SubMeshInfo.AddOrigonalFace(face);
                            if (numVerts == 3)
                            {
                                m_DataSet.AddFaceIndices(face[0]);
                                m_DataSet.AddFaceIndices(face[2]);
                                m_DataSet.AddFaceIndices(face[1]);
                                SubMeshInfo.FaceTriangleList newFace = new SubMeshInfo.FaceTriangleList();
                                newFace.AddTriangle(face[0], face[2], face[1]);
                                m_SubMeshInfo.SubMeshLists.Add(newFace);
                            }
                            else
                            {
                                TriangulatorLY.Triangulate(m_DataSet, face, m_SubMeshInfo);
                            }
                        }
                        break;
                    case "mtllib":
                        if (!string.IsNullOrEmpty(parameters))
                        {
                            //m_mtlLib = parameters;
                        }
                        break;
                    case "usemtl":
                        if (!string.IsNullOrEmpty(parameters) && ObjExportHandler.DefaultMatName != parameters)
                        {
                            m_ImagePaths.Add(faceCount, parameters);
                        }
                        break;
                }
            }
        }

        protected IEnumerator Build()
        {
            foreach (var indexPathPair in m_ImagePaths)
            {
                yield return LoadMaterialTexture(Path.GetDirectoryName(m_SubMeshInfo.FilePath) + '/' + indexPathPair.Value);

                Material material = new Material(ShaderUsed);
                material.name = indexPathPair.Value;
                material.SetFloat("_Glossiness", 0);
                material.SetTexture("_MainTex", m_LoadedTexture);
                m_Materials.Add(indexPathPair.Key, material);
            }

            BuildObject();
        }

        protected void BuildObject()
        {
            //string为原face的位置/法线/uv索引，int为去重后的index，此index作为submesh的三角形顶点索引，以三角形列表形式传入mesh中
            Dictionary<string, int> vIdxCount = new Dictionary<string, int>();
            int vcount = 0;
            //allfaces为全部三角形
            foreach (FaceIndices fi in m_DataSet.Faces)
            {
                string key = DataSetLY.GetFaceIndicesKey(fi);
                int idx;
                // avoid duplicates
                if (!vIdxCount.TryGetValue(key, out idx))
                {
                    vIdxCount.Add(key, vcount);
                    vcount++;
                }
            }
            m_SubMeshInfo.ConvertFaceNewIndicies(vIdxCount);

            int arraySize = vcount;
            Vector3[] newVertices = new Vector3[arraySize];
            Vector2[] newUVs = new Vector2[arraySize];
            Vector3[] newNormals = new Vector3[arraySize];

            foreach (FaceIndices fi in m_DataSet.Faces)
            {
                string key = DataSetLY.GetFaceIndicesKey(fi);
                int k = vIdxCount[key];
                newVertices[k] = m_DataSet.VertList[fi.vertIdx];
                newUVs[k] = m_DataSet.UvList[fi.uvIdx];
                newNormals[k] = m_DataSet.NormalList[fi.normIdx];
            }

            int n = m_DataSet.Faces.Count;
            int numIndices = n;

            MeshFilter meshFilter = m_GameObject.AddComponent<MeshFilter>();
            m_GameObject.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            mesh.name = m_GameObject.name;
            meshFilter.sharedMesh = mesh;

            mesh.vertices = newVertices;
            mesh.uv = newUVs;
            mesh.normals = newNormals;

            int subMeshCount = m_SubMeshInfo.SubMeshLists.Count;
            mesh.subMeshCount = subMeshCount;
            for (int subMeshIndex = 0; subMeshIndex < subMeshCount; ++subMeshIndex)
            {
                SubMeshInfo.FaceTriangleList faceTriangleList = m_SubMeshInfo.SubMeshLists[subMeshIndex];
                int indicesCount = faceTriangleList.Face.Count;
                int[] indices = new int[indicesCount];
                for (int s = 0; s < indicesCount; ++s)
                {
                    string key = DataSetLY.GetFaceIndicesKey(faceTriangleList.Face[s]);
                    indices[s] = vIdxCount[key];
                }
                mesh.SetTriangles(indices, subMeshIndex);
            }

            Renderer renderer = m_GameObject.GetComponent<Renderer>();
            Material[] materials = new Material[subMeshCount];
            for (int i = 0; i < subMeshCount; ++i)
            {
                if (m_Materials.ContainsKey(i))
                {
                    materials[i] = m_Materials[i];
                }                   
                else
                {
                    materials[i] = new Material(DefaultMaterial);
                    materials[i].name = ObjExportHandler.DefaultMatName;
                }                    
            }
            renderer.sharedMaterials = materials;
            RendererExtensions.UpdateGIMaterials(renderer);
            mesh.RecalculateNormals();
            Solve(mesh);
            BuildMeshCollider(m_GameObject, false, false);
        }

        private IEnumerator LoadMaterialTexture(string texPath)
        {
            m_LoadedTexture = null;
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(texPath))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.LogError(uwr.error);
                }
                else
                {
                    m_LoadedTexture = DownloadHandlerTexture.GetContent(uwr);
                }
            }
        }

        public static void Solve(Mesh origMesh)
        {
            if (origMesh.uv == null || origMesh.uv.Length == 0)
            {
                Debug.LogWarning("Unable to compute tangent space vectors - texture coordinates not defined.");
                return;
            }
            if (origMesh.vertices == null || origMesh.vertices.Length == 0)
            {
                Debug.LogWarning("Unable to compute tangent space vectors - vertices not defined.");
                return;
            }
            if (origMesh.normals == null || origMesh.normals.Length == 0)
            {
                Debug.LogWarning("Unable to compute tangent space vectors - normals not defined.");
                return;
            }
            if (origMesh.triangles == null || origMesh.triangles.Length == 0)
            {
                Debug.LogWarning("Unable to compute tangent space vectors - triangles not defined.");
                return;
            }
            Vector3[] vertices = origMesh.vertices;
            Vector3[] normals = origMesh.normals;
            Vector2[] texcoords = origMesh.uv;
            int[] triangles = origMesh.triangles;
            int triVertCount = origMesh.triangles.Length;
            int maxVertIdx = -1;
            for (int i = 0; i < triangles.Length; i++)
            {
                if (maxVertIdx < triangles[i])
                {
                    maxVertIdx = triangles[i];
                }
            }
            if (vertices.Length <= maxVertIdx)
            {
                Debug.LogWarning("Unable to compute tangent space vectors - not enough vertices: " + vertices.Length.ToString());
                return;
            }
            if (normals.Length <= maxVertIdx)
            {
                Debug.LogWarning("Unable to compute tangent space vectors - not enough normals.");
                return;
            }
            if (texcoords.Length <= maxVertIdx)
            {
                Debug.LogWarning("Unable to compute tangent space vectors - not enough UVs.");
                return;
            }

            int vertexCount = origMesh.vertexCount;
            Vector4[] tangents = new Vector4[vertexCount];
            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            int triangleCount = triangles.Length / 3;
            int tri = 0;

            for (int i = 0; i < triangleCount; i++)
            {
                int i1 = triangles[tri];
                int i2 = triangles[tri + 1];
                int i3 = triangles[tri + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = texcoords[i1];
                Vector2 w2 = texcoords[i2];
                Vector2 w3 = texcoords[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;

                tri += 3;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 n = normals[i];
                Vector3 t = tan1[i];

                // Gram-Schmidt orthogonalize
                Vector3.OrthoNormalize(ref n, ref t);

                tangents[i].x = t.x;
                tangents[i].y = t.y;
                tangents[i].z = t.z;

                // Calculate handedness
                tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
            }

            origMesh.tangents = tangents;
        }


        /// <summary>
        /// Build mesh colliders for objects with a mesh filter.
        /// </summary>
        /// <param name="targetObject">Game object to process (if it hasn't a mesh filter nothing happens)</param>
        /// <param name="convex">Build a convex mesh collider.</param>
        /// <param name="isTrigger">Set collider as "trigger"</param>
        /// <param name="inflateMesh">Inflate the convex mesh</param>
        /// <param name="skinWidth">Amout to be inflated</param>
        public static void BuildMeshCollider(GameObject targetObject, bool convex = false, bool isTrigger = false, bool inflateMesh = false, float skinWidth = 0.01f)
        {
            MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Mesh objectMesh = meshFilter.sharedMesh;
                MeshCollider meshCollider = targetObject.AddComponent<MeshCollider>();

                // Note: the order of these assignments is important
                meshCollider.sharedMesh = objectMesh;
                if (convex)
                {
                    meshCollider.convex = convex;
                    meshCollider.isTrigger = isTrigger;
                }
            }
        }

        private float ParseFloat(string floatString)
        {
            return float.Parse(floatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        private void GetFaceIndicesByOneFaceLine(FaceIndices[] faces, string[] p, bool isFaceIndexPlus)
        {
            if (isFaceIndexPlus)
            {
                for (int j = 1; j < p.Length; j++)
                {
                    string[] c = p[j].Trim().Split("/".ToCharArray());
                    FaceIndices fi = new FaceIndices();
                    // vertex
                    int vi = int.Parse(c[0]);
                    fi.vertIdx = vi - 1;
                    // uv
                    if (c.Length > 1 && c[1] != "")
                    {
                        int vu = int.Parse(c[1]);
                        fi.uvIdx = vu - 1;
                    }
                    // normal
                    if (c.Length > 2 && c[2] != "")
                    {
                        int vn = int.Parse(c[2]);
                        fi.normIdx = vn - 1;
                    }
                    else
                    {
                        fi.normIdx = -1;
                    }
                    faces[j - 1] = fi;
                }
            }
            else
            { // for minus index
                int vertexCount = m_DataSet.VertList.Count;
                int uvCount = m_DataSet.UvList.Count;
                int normalCount = m_DataSet.NormalList.Count;
                for (int j = 1; j < p.Length; j++)
                {
                    string[] c = p[j].Trim().Split("/".ToCharArray());
                    FaceIndices fi = new FaceIndices();
                    // vertex
                    int vi = int.Parse(c[0]);
                    fi.vertIdx = vertexCount + vi;
                    // uv
                    if (c.Length > 1 && c[1] != "")
                    {
                        int vu = int.Parse(c[1]);
                        fi.uvIdx = uvCount + vu;
                    }
                    // normal
                    if (c.Length > 2 && c[2] != "")
                    {
                        int vn = int.Parse(c[2]);
                        fi.normIdx = normalCount + vn;
                    }
                    else
                    {
                        fi.normIdx = -1;
                    }
                    faces[j - 1] = fi;
                }
            }
        }

        //protected void LoadMaterialLibrary(string absolutePath)
        //{
        //    string mtlPath = Path.GetDirectoryName(absolutePath) + '/' + m_mtlLib;
        //    string mtlDataText = File.ReadAllText(mtlPath);
        //    string[] lines = mtlDataText.Split("\n".ToCharArray());
        //    char[] separators = new char[] { ' ', '\t' };
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        string line = lines[i].Trim();
        //        // remove comments
        //        if (line.IndexOf("#") != -1) line = line.Substring(0, line.IndexOf("#"));
        //        string[] p = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        //        if (p.Length == 0 || string.IsNullOrEmpty(p[0])) continue;
        //        string parameters = null;
        //        if (line.Length > p[0].Length)
        //        {
        //            parameters = line.Substring(p[0].Length + 1).Trim();
        //        }
        //        switch (p[0])
        //        {
        //            case "newmtl":
        //                materialData.Add(parameters);
        //                break;
        //            case "map_Kd": // newmtl和map_Kd参数名相同
        //                //current.diffuseTexPath = parameters;
        //                break;
        //        }
        //    }
        //}
    }
}
