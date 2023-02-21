// This script draws a number of ellipses using a single discrete line
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class Ellipse2 : MonoBehaviour {

	public Texture lineTexture;
	public int segments = 60;
	public int numberOfEllipses = 10;
	
	void Start () {
		// Make Vector2 list where the size is twice the number of segments (since it's a discrete line where each segment needs two points),
		// multiplied by the total number to be drawn
		var linePoints = new List<Vector2>((segments*2) * numberOfEllipses);
		// Make a VectorLine object using the above points, with a width of 3 pixels
		var line = new VectorLine("Line", linePoints, lineTexture, 3.0f);
		// Create the ellipses in the VectorLine object, where the origin is random, and the radii are random
		for (int i = 0; i < numberOfEllipses; i++) {
			var origin = new Vector2(Random.Range (0, Screen.width), Random.Range (0, Screen.height));
			line.MakeEllipse (origin, Random.Range (10, Screen.width/2), Random.Range (10, Screen.height/2), segments, i*(segments*2));
		}
		// Draw the line
		line.Draw();
	}
}