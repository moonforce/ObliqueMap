// This script draws an ellipse using a continuous line
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class Ellipse1 : MonoBehaviour {

	public Texture lineTexture;
	public float xRadius = 120.0f;
	public float yRadius = 120.0f;
	public int segments = 60;
	public float pointRotation = 0.0f;
	
	void Start () {
		// Make Vector2 list where the size is the number of segments plus one (since the first and last points must be the same)
		var linePoints = new List<Vector2>(segments+1);
		// Make a VectorLine object using the above points, with a width of 3 pixels
		var line = new VectorLine("Line", linePoints, lineTexture, 3.0f, LineType.Continuous);
		// Create an ellipse in the VectorLine object, where the origin is the center of the screen
		// If xRadius and yRadius are the same, you can use MakeCircleInLine instead, which needs just one radius value instead of two
		line.MakeEllipse (new Vector2(Screen.width/2, Screen.height/2), xRadius, yRadius, segments, pointRotation);
		// Draw the line
		line.Draw();
	}
}