using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class SelectionBox : MonoBehaviour {
	
	private VectorLine selectionLine;
	private Vector2 originalPos;
	private List<Color32> lineColors;
	
	void Start () {
		lineColors = new List<Color32>(new Color32[4]);
		selectionLine = new VectorLine("Selection", new List<Vector2>(5), 3.0f, LineType.Continuous);
		selectionLine.capLength = 1.5f;
	}
	
	void OnGUI () {
		GUI.Label (new Rect(10, 10, 300, 25), "Click & drag to make a selection box");
	}
	
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			StopCoroutine ("CycleColor");
			selectionLine.SetColor (Color.white);
			originalPos = Input.mousePosition;
		}
		if (Input.GetMouseButton (0)) {
			selectionLine.MakeRect (originalPos, Input.mousePosition);
			selectionLine.Draw();
		}
		if (Input.GetMouseButtonUp (0)) {
			StartCoroutine ("CycleColor");
		}
	}
	
	IEnumerator CycleColor () {
		while (true) {
			for (int i = 0; i < 4; i++) {
				lineColors[i] = Color.Lerp (Color.yellow, Color.red, Mathf.PingPong((Time.time+i*.25f)*3.0f, 1.0f));
			}
			selectionLine.SetColors (lineColors);
			yield return null;
		}
	}
}