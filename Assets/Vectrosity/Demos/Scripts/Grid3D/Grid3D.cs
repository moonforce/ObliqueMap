using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class Grid3D : MonoBehaviour {

	public int numberOfLines = 20;
	public float distanceBetweenLines = 2.0f;
	public float moveSpeed = 8.0f;
	public float rotateSpeed = 70.0f;
	public float lineWidth = 2.0f;
	
	void Start () {
		numberOfLines = Mathf.Clamp (numberOfLines, 2, 8190);
		var points = new List<Vector3>();
		// Lines down X axis
		for (int i = 0; i < numberOfLines; i++) {
			points.Add (new Vector3(i * distanceBetweenLines, 0, 0));
			points.Add (new Vector3(i * distanceBetweenLines, 0, (numberOfLines-1) * distanceBetweenLines));
		}
		// Lines down Z axis
		for (int i = 0; i < numberOfLines; i++) {
			points.Add (new Vector3(0, 0, i * distanceBetweenLines));
			points.Add (new Vector3((numberOfLines-1) * distanceBetweenLines, 0, i * distanceBetweenLines));
		}
		var line = new VectorLine("Grid", points, lineWidth);
		line.Draw3DAuto();
		
		// Move camera X position to middle of grid
		var pos = transform.position;
		pos.x = ((numberOfLines - 1) * distanceBetweenLines) / 2;
		transform.position = pos;
	}
	
	void Update () {
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			transform.Rotate (Vector3.up * Input.GetAxis ("Horizontal") * Time.deltaTime * rotateSpeed);
			transform.Translate (Vector3.up * Input.GetAxis ("Vertical") * Time.deltaTime * moveSpeed);
		}
		else {
			transform.Translate (new Vector3(Input.GetAxis ("Horizontal") * Time.deltaTime * moveSpeed, 0, Input.GetAxis ("Vertical") * Time.deltaTime * moveSpeed));
		}
	}
	
	void OnGUI () {
		GUILayout.Label (" Use arrow keys to move camera. Hold Shift + arrow up/down to move vertically. Hold Shift + arrow left/right to rotate.");
	}
}