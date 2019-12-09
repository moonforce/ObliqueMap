using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class Utills
{
    //https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public static void setMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2000;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    public static Mesh GetMeshOfGameobject(GameObject go)
    {
        if (go)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf)
            {
                Mesh m = mf.sharedMesh;
                if (!m) { m = mf.mesh; }
                if (m)
                {
                    return m;
                }
            }
        }
        return null;
    }

    public static Mesh GetMeshOfMeshFilter(MeshFilter mf)
    {
        if (mf)
        {
            Mesh m = mf.sharedMesh;
            if (!m) { m = mf.mesh; }
            if (m)
            {
                return m;
            }
        }
        return null;
    }

    public static void DestroyAllChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
        }
    }

    public delegate void SetTexture(Texture2D texture);
    public static IEnumerator DownloadTexture(string imageUrl, SetTexture setTexture)
    {
        imageUrl = UnityWebRequest.EscapeURL(imageUrl);//转义特殊字符，UrlEncode函数可达到同样效果
        //imageUrl = UrlEncode(imageUrl);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + imageUrl))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                setTexture(DownloadHandlerTexture.GetContent(www));
            }
        }
    }

    private readonly static string reservedCharacters = "!*'();:@&=+$,/?%#[]";
    public static string UrlEncode(string value)
    {
        if (String.IsNullOrEmpty(value))
            return String.Empty;

        var sb = new StringBuilder();

        foreach (char @char in value)
        {
            if (reservedCharacters.IndexOf(@char) == -1)
                sb.Append(@char);
            else
                sb.AppendFormat("%{0:X2}", (int)@char);
        }
        return sb.ToString();
    }

    #region texture2image 
    public static System.Drawing.Image Texture2Image(Texture2D texture)
    {
        if (texture == null)
        {
            return null;
        }
        //Save the texture to the stream.
        byte[] bytes = texture.EncodeToPNG();

        //Memory stream to store the bitmap data.
        MemoryStream ms = new MemoryStream(bytes);

        //Seek the beginning of the stream.
        ms.Seek(0, SeekOrigin.Begin);

        //Create an image from a stream.
        System.Drawing.Image bmp2 = System.Drawing.Bitmap.FromStream(ms);

        //Close the stream, we nolonger need it.
        ms.Close();
        ms = null;

        return bmp2;
    }
    #endregion

    #region image2texture 
    public static Texture2D Image2Texture(System.Drawing.Image im)
    {
        if (im == null)
        {
            return new Texture2D(4, 4);
        }

        //Memory stream to store the bitmap data.
        MemoryStream ms = new MemoryStream();

        //Save to that memory stream.
        im.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        //Go to the beginning of the memory stream.
        ms.Seek(0, SeekOrigin.Begin);
        //make a new Texture2D
        Texture2D tex = new Texture2D(im.Width, im.Height);

        tex.LoadImage(ms.ToArray());

        //Close the stream.
        ms.Close();
        ms = null;

        return tex;
    }
    #endregion

    public static void TextureTile2ImageFile(Texture2D texture, out Texture2D tileTexture, int x, int y, int blockWidth, int blockHeight, string path, int quality = 80)
    {
        var colors = texture.GetPixels(x, y, blockWidth, blockHeight);
        tileTexture = new Texture2D(blockWidth, blockHeight);
        tileTexture.SetPixels(colors);
        tileTexture = FlipTexture(tileTexture);
        tileTexture.Apply();
        File.WriteAllBytes(path, tileTexture.EncodeToJPG(quality));
    }

    public static Texture2D FlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);
        int xN = original.width;
        int yN = original.height;
        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                flipped.SetPixel(i, yN - j - 1, original.GetPixel(i, j));
            }
        }
        flipped.Apply();
        return flipped;
    }

    public static void EnableCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void DisableCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public static string ChangeExtensionToDDS(string originName)
    {
        return Path.GetDirectoryName(originName) + "\\" + Path.GetFileNameWithoutExtension(originName) + ".dds";
    }

    public static string DoubleToStringSignificantDigits(double a, int SignificantDigits)
    {
        string formaterG = 'G' + SignificantDigits.ToString("N0");
        string strResult = a.ToString(formaterG);
        int resultLength = SignificantDigits;
        if (strResult.IndexOf('-') >= 0) resultLength++;
        if (strResult.IndexOf('.') >= 0) resultLength++;
        if (Math.Abs(a) < 1) resultLength++; //绝对值小于1，有一个整数0不算有效位
        if (strResult.Length < resultLength)
        {
            if (strResult.IndexOf('.') < 0)
            {
                strResult += '.';
                resultLength++;
            }
            strResult = strResult.PadRight(resultLength, '0');
        }
        return (strResult);
    }

    public static Vector2 ConvertGlobalUvToLocalUv(Vector2 global, UV_AABB AABB)
    {
        Vector2 res = new Vector2((global.x - AABB.MinX) / AABB.Spacing.x, (global.y - AABB.MinY) / AABB.Spacing.y);
        return res;
    }
}
