using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class SelectionBox2 : MonoBehaviour {
	
	public Texture lineTexture;
	public float textureScale = 4.0f;
	private VectorLine selectionLine;
	private Vector2 originalPos;
	
	void Start () {
		selectionLine = new VectorLine("Selection", new List<Vector2>(5), lineTexture, 4.0f, LineType.Continuous);
		selectionLine.textureScale = textureScale;
		// Prevent line from getting blurred by anti-aliasing (the line width is 4 but the texture has transparency that makes it effectively 1)
		selectionLine.alignOddWidthToPixels = true;
	}
	
	void OnGUI () {
		GUI.Label (new Rect(10, 10, 300, 25), "Click & drag to make a selection box");
	}
	
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			originalPos = Input.mousePosition;
		}
		if (Input.GetMouseButton (0)) {
			selectionLine.MakeRect (originalPos, Input.mousePosition);
			selectionLine.Draw();
		}
		selectionLine.textureOffset = -Time.time*2.0f % 1;
	}
}