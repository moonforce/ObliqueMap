// This is the same thing as the Simple3D script, except it draws the line in "real" 3D space, so it can be occluded by other 3D objects and so on.
// If the vector object doesn't appear, make sure the scene view isn't visible while in play mode
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class Simple3D3 : MonoBehaviour {	
	void Start () {
		// Make a Vector3 array that contains points for a cube that's 1 unit in size
		var cubePoints = new List<Vector3>{new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f)};
		
		// Make a line using the above points, with a width of 3 pixels
		var line = new VectorLine(gameObject.name, cubePoints, 3.0f);
		
		// Make VectorManager lines be drawn in the scene instead of as an overlay
		VectorManager.useDraw3D = true;
		
		// Make this transform have the vector line object that's defined above
		// This object is a rigidbody, so the vector object will do exactly what this object does
		// "false" is added at the end, so that the cube mesh is not replaced by an invisible bounds mesh
		VectorManager.ObjectSetup (gameObject, line, Visibility.Dynamic, Brightness.None, false);
	}
}