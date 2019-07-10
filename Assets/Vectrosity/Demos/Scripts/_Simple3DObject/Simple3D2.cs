// See Simple3D.js for another way of doing this that doesn't use TextAsset.bytes
// If the vector object doesn't appear, make sure the scene view isn't visible while in play mode
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class Simple3D2 : MonoBehaviour {

	public TextAsset vectorCube;

	void Start () {
		// Make a Vector3 array from the data stored in the vectorCube text asset
		// Try using different assets from the Vectors folder for different shapes (the collider will still be a cube though!)
		var cubePoints = VectorLine.BytesToVector3List (vectorCube.bytes);
		
		// Make a line using the above points, with a width of 2 pixels
		var line = new VectorLine(gameObject.name, cubePoints, 2.0f);
		
		// Make this transform have the vector line object that's defined above
		// This object is a rigidbody, so the vector object will do exactly what this object does
		VectorManager.ObjectSetup (gameObject, line, Visibility.Dynamic, Brightness.None);
	}
}