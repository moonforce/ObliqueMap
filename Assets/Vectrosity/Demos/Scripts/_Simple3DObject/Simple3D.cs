// See Simple3D 2.js for another way of doing this that uses TextAsset.bytes instead.
// If the vector object doesn't appear, make sure the scene view isn't visible while in play mode
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class Simple3D : MonoBehaviour {
	void Start () {
		// Make a Vector3 array that contains points for a cube that's 1 unit in size
		var cubePoints = new List<Vector3>{new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f)};
		
		// Make a line using the above points, with a width of 2 pixels
		var line = new VectorLine(gameObject.name, cubePoints, 2.0f);
		
		// Make this transform have the vector line object that's defined above
		// This object is a rigidbody, so the vector object will do exactly what this object does
		VectorManager.ObjectSetup (gameObject, line, Visibility.Dynamic, Brightness.None);
	}
}