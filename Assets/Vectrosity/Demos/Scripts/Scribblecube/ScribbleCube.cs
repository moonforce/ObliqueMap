using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class ScribbleCube : MonoBehaviour {
	
	public Texture lineTexture ;
	public Material lineMaterial;
	public int lineWidth = 14;
	private Color color1 = Color.green;
	private Color color2 = Color.blue;
	private VectorLine line;
	private List<Color32> lineColors;
	private int numberOfPoints = 350;
	
	void Start () {
		line = new VectorLine("Line", new List<Vector3>(numberOfPoints), lineTexture, lineWidth, LineType.Continuous);
		line.material = lineMaterial;
		line.drawTransform = transform;
		LineSetup (false);
	}
	
	void LineSetup (bool resize) {
		if (resize) {
			lineColors = null;
			line.Resize (numberOfPoints);
		}
		for (int i = 0; i < line.points3.Count; i++) {
			line.points3[i] = new Vector3(Random.Range (-5.0f, 5.0f), Random.Range (-5.0f, 5.0f), Random.Range (-5.0f, 5.0f));
		}
		SetLineColors();
	}
	
	void SetLineColors () {
		if (lineColors == null) {
			lineColors = new List<Color32>(new Color32[numberOfPoints-1]);
		}
		for (int i = 0; i < lineColors.Count; i++) {
			lineColors[i] = Color.Lerp (color1, color2, (float)i/lineColors.Count);
		}
		line.SetColors (lineColors);
	}
	
	void LateUpdate () {
		line.Draw();
	}
	
	void OnGUI() {
		GUI.Label (new Rect(20, 10, 250, 30), "Zoom with scrollwheel or arrow keys");
		if (GUI.Button (new Rect(20, 50, 100, 30), "Change colors")) {
			// Select random R G B components, making sure they are different, so color1 and color2 will be guaranteed to be not the same color
			var component1 = Random.Range(0, 3);
			var component2 = 0;
			do {
				component2 = Random.Range(0, 3);
			} while (component2 == component1);
			color1 = RandomColor (color1, component1);
			color2 = RandomColor (color2, component2);
			SetLineColors();
		}
		GUI.Label (new Rect(20, 100, 150, 30), "Number of points: " + numberOfPoints);
		numberOfPoints = (int)GUI.HorizontalSlider (new Rect(20, 130, 120, 30), numberOfPoints, 50, 1000);
		if (GUI.Button (new Rect(160, 120, 40, 30), "Set")) {
			LineSetup (true);
		}
	}
	
	Color RandomColor (Color color, int component) {
		// The specified R G B component will be darker than the others
		for (int i = 0; i < 3; i++) {
			if (i == component) {
				color[i] = Random.value*.25f;
			}
			else {
				color[i] = Random.value*.5f + .5f;
			}
		}
		return color;
	}
}