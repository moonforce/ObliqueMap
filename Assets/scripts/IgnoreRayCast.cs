using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IgnoreRayCast : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return false;
    }
}