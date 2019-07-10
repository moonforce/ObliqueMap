using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class DrawLines : MonoBehaviour {
	
	public float rotateSpeed = 90.0f;
	public float maxPoints = 500;
	private VectorLine line;
	private bool endReached;
	private bool continuous = true;
	private bool oldContinuous = true;
	private bool fillJoins = false;
	private bool oldFillJoins = false;
	private bool weldJoins = false;
	private bool oldWeldJoins = false;
	private bool thickLine = false;
	private bool canClick = true;
	
	void Start () {
		SetLine();
	}
	
	void SetLine () {
		VectorLine.Destroy (ref line);
		
		if (!continuous) {
			fillJoins = false;
		}
		var lineType = (continuous? LineType.Continuous : LineType.Discrete);
		var joins = (fillJoins? Joins.Fill : Joins.None);
		var lineWidth = (thickLine? 24 : 2);
		
		line = new VectorLine("Line", new List<Vector2>(), lineWidth, lineType, joins);
		line.drawTransform = transform;
			
		endReached = false;
	}
	
	void Update () {
		// Since we can rotate the transform, get the local space for the current point, so the mouse position won't be rotated with the line
		var mousePos = transform.InverseTransformPoint (Input.mousePosition);
		// Add a line point when the mouse is clicked
		if (Input.GetMouseButtonDown (0) && canClick && !endReached) {
			line.points2.Add (mousePos);
			
			// Start off with 2 points
			if (line.points2.Count == 1) {
				line.points2.Add (Vector2.zero);
			}
			
			if (line.points2.Count == maxPoints) {
				endReached = true;
			}
		}
		
		// The last line point should always be where the mouse is; only draw when there are enough points
		if (line.points2.Count >= 2) {
			line.points2[line.points2.Count-1] = mousePos;
			line.Draw();
		}
		
		// Rotate around midpoint of screen
		transform.RotateAround (new Vector2(Screen.width/2, Screen.height/2), Vector3.forward, Time.deltaTime * rotateSpeed * Input.GetAxis ("Horizontal"));
	}
	
	void OnGUI () {
		var rect = new Rect(20, 20, 265, 220);
		canClick = (!rect.Contains (Event.current.mousePosition));
		GUILayout.BeginArea (rect);
		GUI.contentColor = Color.black;
		GUILayout.Label("Click to add points to the line\nRotate with the right/left arrow keys");
		GUILayout.Space (5);
		continuous = GUILayout.Toggle (continuous, "Continuous line");
		thickLine = GUILayout.Toggle (thickLine, "Thick line");
		line.lineWidth = (thickLine? 24 : 2);
		fillJoins = GUILayout.Toggle (fillJoins, "Fill joins (only works with continuous line)");
		if (line.lineType != LineType.Continuous) {
			fillJoins = false;
		}
		weldJoins = GUILayout.Toggle (weldJoins, "Weld joins");
		if (oldContinuous != continuous) {
			oldContinuous = continuous;
			line.lineType = continuous? LineType.Continuous : LineType.Discrete;
		}
		if (oldFillJoins != fillJoins) {
			if (fillJoins) {
				weldJoins = false;
			}
			oldFillJoins = fillJoins;
		}
		else if (oldWeldJoins != weldJoins) {
			if (weldJoins) {
				fillJoins = false;
			}
			oldWeldJoins = weldJoins;
		}
		if (fillJoins) {
			line.joins = Joins.Fill;
		}
		else if (weldJoins) {
			line.joins = Joins.Weld;
		}
		else {
			line.joins = Joins.None;
		}
		GUILayout.Space (10);
		GUI.contentColor = Color.white;
		if (GUILayout.Button ("Randomize Color", GUILayout.Width(150))) {
			RandomizeColor();
		}
		if (GUILayout.Button ("Randomize All Colors", GUILayout.Width(150))) {
			RandomizeAllColors();
		}
		if (GUILayout.Button("Reset line", GUILayout.Width(150))) {
			SetLine();
		}
		
		if (endReached) {
			GUI.contentColor = Color.black;
			GUILayout.Label ("No more points available. You must be bored!");
		}
		GUILayout.EndArea();
	}
	
	void RandomizeColor () {
		line.color = new Color(Random.value, Random.value, Random.value);
	}
	
	void RandomizeAllColors () {
		int maxSegment = line.GetSegmentNumber();
		for (int i = 0; i < maxSegment; i++) {
			line.SetColor (new Color(Random.value, Random.value, Random.value), i);
		}
	}
}