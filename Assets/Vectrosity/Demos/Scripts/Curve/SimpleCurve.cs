// This script draws a curve using a continuous line
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class SimpleCurve : MonoBehaviour {

	public Vector2[] curvePoints;	// The points for the curve are defined in the inspector
	public int segments = 50;
	
	void Start () {
		if (curvePoints.Length != 4) {
			Debug.Log ("Curve points array must have 4 elements only");
			return;
		}
	
		// Make Vector2 list where the size is the number of segments plus one, since it's for a continuous line
		// (A discrete line would need the size to be segments*2)
		var linePoints = new List<Vector2>(segments+1);
		
		// Make a VectorLine object using the above points and the default material,
		// with a width of 2 pixels, an end cap of 0 pixels, and depth 0
		var line = new VectorLine("Curve", linePoints, 2.0f, LineType.Continuous, Joins.Weld);
		// Create a curve in the VectorLine object using the curvePoints array as defined in the inspector
		line.MakeCurve (curvePoints, segments);
		
		// Draw the line
		line.Draw();
	}
}