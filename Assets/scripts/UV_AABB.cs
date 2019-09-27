using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class UV_AABB
{
    static int MaterialExpandPixels = 3;

    public float MinX = 1f;
    public float MaxX = 0f;
    public float MinY = 1f;
    public float MaxY = 0f;

    public Vector2 Center
    {
        get
        {
            return new Vector2((MinX + MaxX) / 2f, (MinY + MaxY) / 2f);
        }
    }

    public Vector2 Spacing
    {
        get
        {
            return new Vector2(MaxX - MinX, MaxY - MinY);
        }
    }

    public void Reset()
    {
        MinX = 1f;
        MaxX = 0f;
        MinY = 1f;
        MaxY = 0f;
    }

    public void UpdateAABB(Vector2 uv)
    {
        MinX = Mathf.Min(MinX, uv.x);
        MaxX = Mathf.Max(MaxX, uv.x);
        MinY = Mathf.Min(MinY, uv.y);
        MaxY = Mathf.Max(MaxY, uv.y);
    }

    public void ExpandAABB(Vector2 textureSize)
    {
        MinX -= MaterialExpandPixels / textureSize.x;
        MinY -= MaterialExpandPixels / textureSize.y;
        MaxX += MaterialExpandPixels / textureSize.x;
        MaxY += MaterialExpandPixels / textureSize.y;
    }
}
