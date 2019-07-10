// *****************************************************************************
//
// This file is part of a MASA library or program.
// Refer to the included end-user license agreement for restrictions.
//
// Copyright (c) 2014 MASA Group
//
// *****************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace mlv
{
    public class Exporter : MonoBehaviour 
    {
        [Serializable]
        public class LayerData
        { 
            public string layerName = "YourObjHere";
            public GameObject[] selection;
            public GameObject[] allowedObjects;
            public bool recursive = true;
            public bool staticOnly = true;
            public bool enabled = true;
            
            public bool Contains( GameObject obj )
            {
                for( int i = 0; i< selection.Length; ++i )
                {
                    if( selection[i] == obj )
                        return true;
                }
                return false;
            }
        }

        int exportNumber = 0;

        public StreamWriter CreateMLVScene()
        {
            string template = "{\n\t\"type\" : \"root\",\n\t\"enable\" : true,\n\t\"children\" : \n\t[";
            string path = dotSceneExporterPath;
            StreamWriter tw = new StreamWriter(path);
            tw.WriteLine(template);
            return tw;
        }

        public void AddLayerMLVScene(StreamWriter tw, string name, bool end)
        {
            string template = "\t\t{\n\t\t\t\"translation\" :  [0,0,0],\n\t\t\t\"rotation\" :  [0,0,0,1],\n\t\t\t\"scale\" :  [1,1,1],\n\t\t\t\"type\" :  \"geometry\",\n\t\t\t\"label\" :  \"" + name + "\",\n\t\t\t\"color\" :  \"#cccccc\",\n\t\t\t\"source\" :  \"" + name + ".obj\"\n\t\t}";
            if (!end)
                template += ",";
            tw.WriteLine(template);
            
        }

        public void CloseMLVScene(StreamWriter tw)
        {
            tw.WriteLine(" \t]\n}");
            tw.Close();
        }

        public int ExportObj( StreamWriter tw, Mesh me, Transform transform, string name, int offset )
        {
            tw.WriteLine( "g " + name + "_" + exportNumber.ToString() );
            exportNumber++;
            tw.WriteLine();
            Vector3[] vertices = me.vertices;           // cached for amazing speedup
            // export all vertices ( transformed to world then transformed to MLV frame )
            for( int i = 0; i< me.vertexCount; ++i )
            { 
                Vector3 v = transform.TransformPoint( vertices[i] );
                tw.WriteLine( "v "+(v.x*1f).ToString()+ " " + (v.y*1f).ToString() + " " + (v.z*1f).ToString() );
            }
            tw.WriteLine();
            Vector3[] normals = me.normals;             // cached for amazing speedup
            // export all normals ( transformed to world then transformed to MLV frame )
            for( int i = 0; i< me.vertexCount; ++i )
            {
                Vector3 v = transform.TransformDirection( normals[i] );
                tw.WriteLine( "vn "+(v.x).ToString()+ " " + v.y.ToString() + " " + v.z.ToString() );
            }
            tw.WriteLine();
            Vector2[] uvs = me.uv;                      // cached for amazing speedup
            // export all uvs 
            for( int i = 0; i< me.vertexCount; ++i )
            {
                Vector2 uv = uvs[i];
                tw.WriteLine( "vt "+uv.x.ToString()+ " " + uv.y.ToString() );
            }       
            tw.WriteLine();
            int[] triangles = me.triangles;             // cached for amazing speedup
            // export all faces (taking account offset for vertex indice )
            for( int f = 0; f < triangles.Length/3; ++f )
            {
                int a = triangles[f*3]+1+offset;
                int b = triangles[f*3+1]+1+offset;
                int c = triangles[f*3+2]+1+offset;
                tw.WriteLine( "f "+a.ToString()+"/"+a.ToString()+"/"+a.ToString()+" "+b.ToString()+"/"+b.ToString()+"/"+b.ToString()+" "+c.ToString()+"/"+c.ToString()+"/"+c.ToString() );
            }
            tw.WriteLine();
            return me.vertices.Length;
        }

        public int ExportTerrain( StreamWriter tw, Terrain ter, Transform transform, string name, int offset )
        {
            TerrainData terrain = ter.terrainData;
            int w = terrain.heightmapWidth;
            int h = terrain.heightmapHeight;
            Vector3 meshScale = terrain.size;
            meshScale = new Vector3(meshScale.x / (w - 1) , meshScale.y, meshScale.z / (h - 1) );
            Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
            
            float[,] tData = terrain.GetHeights(0, 0, w, h);
            
            Vector3[] tVertices = new Vector3[w * h];
            Vector2[] tUV = new Vector2[w * h];
            
            int[] tPolys = new int[(w - 1) * (h - 1) * 6];
            
            // Build vertices and UVs
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(x, tData[y, x], y)) + transform.position;
                    tUV[y * w + x] = Vector2.Scale(new Vector2(x , y), uvScale);
                }
            }
            
            int index = 0;
            // Build triangle indices: 3 indices into vertex array for each triangle
            for (int y = 0; y < h - 1; y++)
            {
                for (int x = 0; x < w - 1; x++)
                {
                    // For each grid cell output two triangles
                    tPolys[index++] = (y * w) + x + offset;
                    tPolys[index++] = ((y + 1) * w) + x + offset;
                    tPolys[index++] = (y * w) + x + 1 + offset;
                    
                    tPolys[index++] = ((y + 1) * w) + x + offset;
                    tPolys[index++] = ((y + 1) * w) + x + 1 + offset;
                    tPolys[index++] = (y * w) + x + 1 + offset;
                }
            }
            try
            {
                // Write vertices
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                for (int i = 0; i < tVertices.Length; i++)
                {
                    StringBuilder sb = new StringBuilder("v ", 20);
                    // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
                    // Which is important when you're exporting huge terrains.
                    sb.Append(tVertices[i].x.ToString()).Append(" ").
                        Append(tVertices[i].y.ToString()).Append(" ").
                            Append(tVertices[i].z.ToString());
                    tw.WriteLine(sb);
                }
                // Write UVs
                for (int i = 0; i < tUV.Length; i++)
                {
                    StringBuilder sb = new StringBuilder("vt ", 22);
                    sb.Append(tUV[i].x.ToString()).Append(" ").
                        Append(tUV[i].y.ToString());
                    tw.WriteLine(sb);
                }
                // Write triangles
                for (int i = 0; i < tPolys.Length; i += 3)
                {
                    StringBuilder sb = new StringBuilder("f ", 43);
                    sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                        Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                            Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1);
                    tw.WriteLine(sb);
                }
            }
            catch (Exception err)
            {
                Debug.Log("Error saving file: " + err.Message);
            }
            return offset + tVertices.Length ;
            
        }

        public string dotSceneExporterPath = string.Empty;
        public List< LayerData > layers = new List< LayerData >();
        
        public int ExistingName( string name )
        {
            int count = 0;
            for( int i = 0; i < layers.Count; ++i )
            {
                if( string.Compare ( layers[i].layerName, name, true ) == 0  )
                    count ++;
            }
            return count;
        }
        
    }
}