using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class UV_AABB
{
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

    public void Reset()
    {
        MinX = 1f;
        MaxX = 0f;
        MinY = 1f;
        MaxY = 0f;
    }
}
