using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class DrawCurve : MonoBehaviour {

	public Texture lineTexture;
	public Color lineColor = Color.white;
	public Texture dottedLineTexture;
	public Color dottedLineColor = Color.yellow;
	public int segments = 60;
	
	public GameObject anchorPoint;
	public GameObject controlPoint;
	
	private int numberOfCurves = 1;
	
	private VectorLine line;
	private VectorLine controlLine;
	
	private int pointIndex = 0;
	private GameObject anchorObject;
	private int oldWidth;
	private bool useDottedLine = false;
	private bool oldDottedLineSetting = false;
	private int oldSegments;
	private bool listPoints = false;
	
	public static DrawCurve use;
	public static Camera cam;
	
	void Start () {
		use = this;	// Reference to this script, so FindObjectOfType etc. are not needed
		cam = Camera.main;
		oldWidth = Screen.width;
		oldSegments = segments;
	
		// Set up initial curve points (also used for drawing the green lines that connect control points to anchor points)
		var curvePoints = new List<Vector2>();
		curvePoints.Add (new Vector2(Screen.width*.25f, Screen.height*.25f));
		curvePoints.Add (new Vector2(Screen.width*.125f, Screen.height*.5f));
		curvePoints.Add (new Vector2(Screen.width-Screen.width*.25f, Screen.height-Screen.height*.25f));
		curvePoints.Add (new Vector2(Screen.width-Screen.width*.125f, Screen.height*.5f));
		
		// Make the control lines
		controlLine = new VectorLine("Control Line", curvePoints, 2.0f);
		controlLine.color = new Color(0.0f, .75f, .1f, .6f);
		controlLine.Draw();
		
		// Make the line object for the curve
		line = new VectorLine("Curve", new List<Vector2>(segments+1), lineTexture, 5.0f, LineType.Continuous, Joins.Weld);
		
		// Create a curve in the VectorLine object
		line.MakeCurve (curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments);
		line.Draw();
		
		// Make the GUITexture objects for anchor and control points (two anchor points and two control points)
		AddControlObjects();
		AddControlObjects();
	}
	
	void SetLine () {
		if (useDottedLine) {
			line.texture = dottedLineTexture;
			line.color = dottedLineColor;
			line.lineWidth = 8.0f;
			line.textureScale = 1.0f;
		}
		else {
			line.texture = lineTexture;
			line.color = lineColor;
			line.lineWidth = 5.0f;
			line.textureScale = 0.0f;		
		}
	}
	
	void AddControlObjects () {
		anchorObject = Instantiate(anchorPoint, cam.ScreenToViewportPoint(controlLine.points2[pointIndex]), Quaternion.identity) as GameObject;
		anchorObject.GetComponent<CurvePointControl>().objectNumber = pointIndex++;
		var controlObject = Instantiate(controlPoint, cam.ScreenToViewportPoint(controlLine.points2[pointIndex]), Quaternion.identity) as GameObject;
		controlObject.GetComponent<CurvePointControl>().objectNumber = pointIndex++;
		// Make the anchor object have a reference to the control object, so they can move together
		// Having control objects be children of anchor objects would be easier, but parent/child doesn't really work with GUITextures
		anchorObject.GetComponent<CurvePointControl>().controlObject = controlObject;
	}
	
	public void UpdateLine (int objectNumber, Vector2 pos, GameObject go) {
		var oldPos = controlLine.points2[objectNumber];	// Get previous position, so we can make the control point move with the anchor point
		controlLine.points2[objectNumber] = pos;
		int curveNumber = objectNumber / 4;
		int curveIndex = curveNumber * 4;
		line.MakeCurve (controlLine.points2[curveIndex], controlLine.points2[curveIndex+1], controlLine.points2[curveIndex+2], controlLine.points2[curveIndex+3],
						segments, curveNumber * (segments+1));
			
		// If it's an anchor point...
		if (objectNumber % 2 == 0) {
			// Move control point also
			controlLine.points2[objectNumber+1] += pos-oldPos;
			go.GetComponent<CurvePointControl>().controlObject.transform.position = cam.ScreenToViewportPoint(controlLine.points2[objectNumber+1]);
			// If it's not an end anchor point, move the next anchor/control points as well, and update the next curve
		 	if (objectNumber > 0 && objectNumber < controlLine.points2.Count-2) {
				controlLine.points2[objectNumber+2] = pos;
				controlLine.points2[objectNumber+3] += pos-oldPos;
				go.GetComponent<CurvePointControl>().controlObject2.transform.position = cam.ScreenToViewportPoint(controlLine.points2[objectNumber+3]);
				line.MakeCurve (controlLine.points2[curveIndex+4], controlLine.points2[curveIndex+5], controlLine.points2[curveIndex+6], controlLine.points2[curveIndex+7],
								segments, (curveNumber+1) * (segments+1));
			}
		}
		
		line.Draw();
		controlLine.Draw();	
	}
	
	void OnGUI () {
		if (GUI.Button (new Rect(20, 20, 100, 30), "Add Point")) {
			AddPoint();
		}
		
		GUI.Label(new Rect(20, 59, 200, 30), "Curve resolution: " + segments);
		segments = (int)GUI.HorizontalSlider (new Rect(20, 80, 150, 30), segments, 3, 60);
		if (oldSegments != segments) {
			oldSegments = segments;
			ChangeSegments();
		}
		
		useDottedLine = GUI.Toggle (new Rect(20, 105, 80, 20), useDottedLine, " Dotted line");
		if (oldDottedLineSetting != useDottedLine) {
			oldDottedLineSetting = useDottedLine;
			SetLine();
			line.Draw();
		}
		
		GUILayout.BeginArea (new Rect(20, 150, 150, 800));
		if (GUILayout.Button (listPoints? "Hide points" : "List points", GUILayout.Width(100)) ) {
			listPoints = !listPoints;
		}
		if (listPoints) {
			int idx = 0;
			for (int i = 0; i < controlLine.points2.Count; i += 2) {
				GUILayout.Label ("Anchor " + idx + ": (" + (int)(controlLine.points2[i].x) + ", " + (int)(controlLine.points2[i].y) + ")");
				GUILayout.Label ("Control " + idx++ + ": (" + (int)(controlLine.points2[i+1].x) + ", " + (int)(controlLine.points2[i+1].y) + ")");
			}
		}
		GUILayout.EndArea();
	}
	
	void AddPoint () {
		// Don't do anything if adding a new point would exceed the max number of vertices per mesh
		if (line.points2.Count + controlLine.points2.Count + segments + 4 > 16383) return;
		
		// Make the first anchor and control points of the new curve be the same as the second anchor/control points of the previous curve
		controlLine.points2.Add (controlLine.points2[pointIndex-2]);
		controlLine.points2.Add (controlLine.points2[pointIndex-1]);
		// Make the second anchor/control points of the new curve be offset a little ways from the first
		var offset = (controlLine.points2[pointIndex-2] - controlLine.points2[pointIndex-4]) * .25f;
		controlLine.points2.Add (controlLine.points2[pointIndex-2] + offset);
		controlLine.points2.Add (controlLine.points2[pointIndex-1] + offset);
		// If that made the new anchor point go off the screen, offset them the opposite way
		if (controlLine.points2[pointIndex+2].x > Screen.width || controlLine.points2[pointIndex+2].y > Screen.height ||
				controlLine.points2[pointIndex+2].x < 0 || controlLine.points2[pointIndex+2].y < 0) {
			controlLine.points2[pointIndex+2] = controlLine.points2[pointIndex-2] - offset;
			controlLine.points2[pointIndex+3] = controlLine.points2[pointIndex-1] - offset;
		}
		// For the next control point, make the initial position offset from the anchor point the opposite way as the second control point in the curve
		var controlPointPos = controlLine.points2[pointIndex-1] + (controlLine.points2[pointIndex] - controlLine.points2[pointIndex-1])*2;
		pointIndex++;	// Skip the next anchor point, since we want the second anchor point of one curve and the first anchor point of the next curve
						// to move together (this is handled in UpdateLine)
		controlLine.points2[pointIndex] = controlPointPos;
		// Make another control point
		var controlObject = Instantiate (controlPoint, cam.ScreenToViewportPoint (controlPointPos), Quaternion.identity) as GameObject;
		controlObject.GetComponent<CurvePointControl>().objectNumber = pointIndex++;
		// For the last anchor object that was made, make a reference to this control point so they can move together
		anchorObject.GetComponent<CurvePointControl>().controlObject2 = controlObject;
		// Then make another anchor/control point group
		AddControlObjects();
		
		// Update the control lines
		controlLine.Draw();
		
		// Update the curve with the new points
		line.Resize ((segments+1) * ++numberOfCurves);
		line.MakeCurve (controlLine.points2[pointIndex-4], controlLine.points2[pointIndex-3], controlLine.points2[pointIndex-2], controlLine.points2[pointIndex-1],
						segments, (segments+1) * (numberOfCurves-1));
		line.Draw();
	}
	
	void ChangeSegments () {
		// Don't do anything if the requested segments would make the curve exceed the max number of vertices per mesh
		if (segments*4*numberOfCurves > 65534) return;
		
		line.Resize ((segments+1) * numberOfCurves);
		for (int i = 0; i < numberOfCurves; i++) {
			line.MakeCurve (controlLine.points2[i*4], controlLine.points2[i*4+1], controlLine.points2[i*4+2], controlLine.points2[i*4+3], segments, (segments+1)*i);
		}
		line.Draw();
	}
	
	void Update () {
		if (Screen.width != oldWidth) {
			oldWidth = Screen.width;
			ChangeResolution();
		}
	}
	
	void ChangeResolution () {
		var controlPointObjects = GameObject.FindGameObjectsWithTag ("GameController");
		foreach (var obj in controlPointObjects) {
			obj.transform.position = cam.ScreenToViewportPoint(controlLine.points2[obj.GetComponent<CurvePointControl>().objectNumber]);
		}
	}
}