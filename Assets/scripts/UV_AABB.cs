using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class UV_AABB
{
    public float MinX = 1f;
    public float MaxX = 0f;
    public float MinY = 1f;
    public float MaxY = 0f;

    public void Reset()
    {
        MinX = 1f;
        MaxX = 0f;
        MinY = 1f;
        MaxY = 0f;
    }
}
