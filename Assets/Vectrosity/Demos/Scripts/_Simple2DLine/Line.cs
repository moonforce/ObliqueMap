// Use this method if you need more control than you get with Vector.SetLine
using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

namespace Vectrosity
{
    public class Line : MonoBehaviour
    {
        void Start()
        {
            // Make a Vector2 list; in this case we just use 2 elements...
            var linePoints = new List<Vector2>();
            linePoints.Add(new Vector2(0, Random.Range(0, Screen.height)));             // ...one on the left side of the screen somewhere
            linePoints.Add(new Vector2(Screen.width - 1, Random.Range(0, Screen.height)));  // ...and one on the right

            // Make a VectorLine object using the above points, with a width of 2 pixels
            var line = new VectorLine("Line", linePoints, 2.0f);

            // Draw the line
            line.Draw();
        }
    }
}