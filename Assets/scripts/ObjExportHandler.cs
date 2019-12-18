using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

public class ObjExportHandler
{
    public static string DefaultMatName = "default185617";
    public static string ExportSuffix = "";

    public static void Export(MeshFilter mf, bool AddOffset)
    {
        SubMeshInfo subMeshInfo = mf.transform.GetComponent<SubMeshInfo>();
        string baseFileName = Path.GetFileNameWithoutExtension(subMeshInfo.FilePath);
        string baseDirectoryName = Path.GetDirectoryName(subMeshInfo.FilePath);
        string exportDirectoryName = baseDirectoryName;
        if (AddOffset)
            exportDirectoryName += @"\Output";
        if (!Directory.Exists(exportDirectoryName))
            Directory.CreateDirectory(exportDirectoryName);

        HashSet<string> materialCache = new HashSet<string>();     
        StringBuilder sb = new StringBuilder();
        StringBuilder sbMaterials = new StringBuilder();
        sb.AppendLine(ProjectCtrl.Instance.CompleteUvCommentLine);
        sb.AppendLine("mtllib " + baseFileName + ExportSuffix + ".mtl");

        MeshRenderer mr = mf.GetComponent<MeshRenderer>();
        Material[] mats = mr.sharedMaterials;
        for (int j = 0; j < mats.Length; j++)
        {
            Material m = mats[j];
            if (DefaultMatName == m.name)
                continue;
            if (!materialCache.Contains(m.name))
            {
                if (AddOffset)
                {
                    File.Copy(baseDirectoryName + @"\" + m.name, exportDirectoryName + @"\" + m.name);
                }
                materialCache.Add(m.name);
                sbMaterials.Append(MaterialToString(m));
                sbMaterials.AppendLine();
            }
        }

        Mesh mesh = mf.sharedMesh;
        string loadedText;
        if (!subMeshInfo.IsCompleteUvModel)
        {
            loadedText = File.ReadAllText(baseDirectoryName + @"\CompleteUvModel\" + baseFileName + ".obj");
        }
        else
        {
            loadedText = File.ReadAllText(subMeshInfo.FilePath);
        }
        string[] lines = loadedText.Split("\n".ToCharArray());
        char[] separators = new char[] { ' ', '\t' };
        for (int j = 0; j < lines.Length; j++)
        {
            string line = lines[j].Trim();
            if (line.Length > 0 && line[0] == '#')
            {
                // comment line
                continue;
            }
            string[] p = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length == 0)
            {
                // empty line
                continue;
            }

            switch (p[0])
            {
                case "v":                    
                    if (!AddOffset)
                    {
                        sb.AppendLine(line);
                    }
                    else
                    {
                        string vLine = "v " + (float.Parse(p[1]) - SettingsPanelCtrl.Instance.DeltaX).ToString() + ' ' + (float.Parse(p[2]) - SettingsPanelCtrl.Instance.DeltaY).ToString() + ' ' + p[3];
                        sb.AppendLine(vLine);
                    }
                    break;
                case "vn":
                    sb.AppendLine(line);
                    break;
            }
        }

        foreach (Vector2 v in mesh.uv)
        {
            sb.AppendLine("vt " + v.x + " " + v.y);
        }

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            string matName = mr.sharedMaterials[i].name;
            sb.AppendLine("usemtl " + matName);

            List<string> OrigonalFacesList = subMeshInfo.OrigonalFacesLists[i];
            Dictionary<string, int> OrigonalFaceNewIndexDictionary = subMeshInfo.OrigonalFaceNewIndexDictionary;
            string newFace = string.Empty;
            foreach (string OrigonalFace in OrigonalFacesList)
            {
                int newIndex = OrigonalFaceNewIndexDictionary[OrigonalFace];
                string[] element = OrigonalFace.Split('/');
                int v = int.Parse(element[0]) + 1;
                int vt = newIndex + 1;
                int vn = int.Parse(element[2]) + 1;
                newFace += v.ToString() + '/' + vt.ToString() + '/' + vn.ToString() + ' ';
            }
            sb.AppendLine("f " + newFace);
        }
        File.WriteAllText(exportDirectoryName + @"\" + baseFileName + ExportSuffix + ".obj", sb.ToString());
        File.WriteAllText(exportDirectoryName + @"\" + baseFileName + ExportSuffix + ".mtl", sbMaterials.ToString());
    }

    private static string MaterialToString(Material m)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("newmtl " + m.name);
        sb.AppendLine("map_Kd " + m.name);
        return sb.ToString();
    }
}