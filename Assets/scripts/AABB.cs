using System.Collections;
using System.Collections.Generic;

public class AABB
{
    public float MinX { get; set; } = 1f;
    public float MaxX { get; set; } = 0f;
    public float MinY { get; set; } = 1f;
    public float MaxY { get; set; } = 0f;

    public void Reset()
    {
        MinX = 1f;
        MaxX = 0f;
        MinY = 1f;
        MaxY = 0f;
    }
}
