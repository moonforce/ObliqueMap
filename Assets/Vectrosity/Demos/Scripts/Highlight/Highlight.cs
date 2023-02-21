using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class Highlight : MonoBehaviour {

	public int lineWidth = 5;
	public int energyLineWidth = 4;
	public float selectionSize = .5f;
	public float force = 20.0f;
	public int pointsInEnergyLine = 100;
	
	private VectorLine line;
	private VectorLine energyLine;
	private RaycastHit hit;
	private int selectIndex = 0;
	private float energyLevel = 0.0f;
	private bool canClick;
	private GameObject[] spheres;
	private double timer = 0.0;
	private int ignoreLayer;
	private int defaultLayer;
	private bool fading = false;
	
	void Start () {
		Time.fixedDeltaTime = .01f;
		spheres = new GameObject[GetComponent<MakeSpheres>().numberOfSpheres];
		ignoreLayer = LayerMask.NameToLayer ("Ignore Raycast");
		defaultLayer = LayerMask.NameToLayer ("Default");
	
		// Set up the two lines
		line = new VectorLine("Line", new List<Vector2>(), lineWidth);
		line.color = Color.green;
		line.capLength = lineWidth*.5f;
		energyLine = new VectorLine("Energy", new List<Vector2>(pointsInEnergyLine), null, energyLineWidth, LineType.Continuous);
		SetEnergyLinePoints();
	}
	
	void SetEnergyLinePoints () {
		for (int i = 0; i < energyLine.points2.Count; i++) {
			var xPoint = Mathf.Lerp(70, Screen.width-20, (i+0.0f)/energyLine.points2.Count);
			energyLine.points2[i] = new Vector2(xPoint, Screen.height*.1f);
		}
	}
	
	void Update () {
		// Don't allow clicking in the left-most 50 pixels (where the slider is), or if the spheres are currently fading
		if (Input.GetMouseButtonDown (0) && Input.mousePosition.x > 50 && !fading) {
			// If neither shift key is down, reset selection
			if (!(Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && selectIndex > 0) {
				ResetSelection (true);
			}
			// See if we clicked on an object (the room is set to the IgnoreRaycast layer, so we can't select it)
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
				spheres[selectIndex] = hit.collider.gameObject;
				spheres[selectIndex].layer = ignoreLayer;	// So it can't be clicked again (unless reset)
				spheres[selectIndex].GetComponent<Renderer>().material.EnableKeyword ("_EMISSION");	// So changing emission color will work at runtime
				selectIndex++;
				line.Resize (selectIndex * 10);
			}
		}
		
		// Draw a square for each selected object
		for (int i = 0; i < selectIndex; i++) {
			// Make the size of the square larger or smaller depending on the object's Z distance from the camera
			var squareSize = (Screen.height * selectionSize) / Camera.main.transform.InverseTransformPoint(spheres[i].transform.position).z;
			var screenPoint = Camera.main.WorldToScreenPoint (spheres[i].transform.position);
			var thisSquare = new Rect(screenPoint.x-squareSize, screenPoint.y-squareSize, squareSize*2, squareSize*2);
			line.MakeRect (thisSquare, i*10);
			// Make a line connecting from the midpoint of the square's left edge to the energyLevel slider position
			line.points2[i*10 + 8] = new Vector2(thisSquare.x - lineWidth*.25f, thisSquare.y + squareSize);
			line.points2[i*10 + 9] = new Vector2(35, Mathf.Lerp (65, Screen.height-25, energyLevel));
			// Change color of selected objects
			spheres[i].GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(energyLevel, energyLevel, energyLevel));
		}
	}
	
	void FixedUpdate () {
		// Move y position of all points to the left by one
		int i;
		for (i = 0; i < energyLine.points2.Count-1; i++) {
			energyLine.points2[i] = new Vector2(energyLine.points2[i].x, energyLine.points2[i+1].y);
		}
		// Calculate new point based on the energy level and time
		timer += Time.deltaTime * Mathf.Lerp (5.0f, 20.0f, energyLevel);
		energyLine.points2[i] = new Vector2(energyLine.points2[i].x, Screen.height * (.1f + Mathf.Sin ((float)timer) * .08f * energyLevel));
	}
	
	void LateUpdate () {
		line.Draw();
		energyLine.Draw();
	}
	
	void ResetSelection (bool instantFade) {
		// Fade sphere colors back to normal
		if (energyLevel > 0.0f) {
			StartCoroutine (FadeColor (instantFade));
		}
		// Reset the selection index and erase all squares and lines that might have been made
		selectIndex = 0;
		energyLevel = 0.0f;
		line.points2.Clear();
		line.Draw();
		// Reset sphere layers so they can be clicked again
		foreach (var sphere in spheres) {
			if (sphere) sphere.layer = defaultLayer;
		}
	}
	
	IEnumerator FadeColor (bool instantFade) {
		if (instantFade) {
			// Set all spheres to normal color instantly
			for (int i = 0; i < selectIndex; i++) {
				spheres[i].GetComponent<Renderer>().material.SetColor ("_EmissionColor", Color.black);
			}
		}
		else {
			// Do a gradual fade
			fading = true;
			var startColor = new Color(energyLevel, energyLevel, energyLevel, 0.0f);
			var thisIndex = selectIndex;	// Since selectIndex is set back to 0 this frame
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime) {
				for (int i = 0; i < thisIndex; i++) {
					spheres[i].GetComponent<Renderer>().material.SetColor ("_EmissionColor", Color.Lerp (startColor, Color.black, t));
				}
				yield return null;
			}
			fading = false;
		}
	}
	
	void OnGUI () {
		GUI.Label (new Rect(60, 20, 600, 40), "Click to select sphere, shift-click to select multiple spheres\nThen change energy level slider and click Go");
		energyLevel = GUI.VerticalSlider (new Rect(30, 20, 10, Screen.height-80), energyLevel, 1.0f, 0.0f);
		// Prevent energy slider from working if nothing is selected
		if (selectIndex == 0) {
			energyLevel = 0.0f;
		}
		if (GUI.Button (new Rect(20, Screen.height-40, 32, 20), "Go")) {
			for (int i = 0; i < selectIndex; i++) {
				spheres[i].GetComponent<Rigidbody>().AddRelativeForce (Vector3.forward * force * energyLevel, ForceMode.VelocityChange);
			}
			ResetSelection (false);
		}
	}
}