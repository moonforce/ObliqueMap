using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class SelectLine : MonoBehaviour {
	
	public float lineThickness = 10.0f;
	public int extraThickness = 2;
	public int numberOfLines = 2;
	private VectorLine[] lines;
	private bool[] wasSelected;
	
	void Start () {
		lines = new VectorLine[numberOfLines];
		wasSelected = new bool[numberOfLines];
		for (int i = 0; i < numberOfLines; i++) {
			lines[i] = new VectorLine("SelectLine", new List<Vector2>(5), lineThickness, LineType.Continuous, Joins.Fill);
			SetPoints (i);
		}
	}
	
	void SetPoints (int i) {
		for (int j = 0; j < lines[i].points2.Count; j++) {
			lines[i].points2[j] = new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height-20));
		}
		lines[i].Draw();	
	}
	
	void Update () {
		for (int i = 0; i < numberOfLines; i++) {
			int index;
			if (lines[i].Selected (Input.mousePosition, extraThickness, out index)) {
				if (!wasSelected[i]) {	// We use wasSelected to update the line color only when needed, instead of every frame
					lines[i].SetColor (Color.green);
					wasSelected[i] = true;
				}
				if (Input.GetMouseButtonDown (0)) {
					SetPoints (i);
				}
			}
			else {
				if (wasSelected[i]) {
					wasSelected[i] = false;
					lines[i].SetColor (Color.white);
				}
			}
		}
	}
	
	void OnGUI () {
		GUI.Label (new Rect(10, 10, 800, 30), "Click a line to make a new line");
	}
}