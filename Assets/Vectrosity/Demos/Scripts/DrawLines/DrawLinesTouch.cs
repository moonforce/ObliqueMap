// For touchscreen devices -- draw a line with your finger
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class DrawLinesTouch : MonoBehaviour {

	public Texture2D lineTex;
	public int maxPoints = 5000;
	public float lineWidth = 4.0f;
	public int minPixelMove = 5;	// Must move at least this many pixels per sample for a new segment to be recorded
	public bool useEndCap = false;
	public Texture2D capLineTex;
	public Texture2D capTex;
	public float capLineWidth = 20.0f;
	
	private VectorLine line;
	private Vector2 previousPosition;
	private int sqrMinPixelMove;
	private bool canDraw = false;
	private Touch touch;
	
	void Start () {
		float useLineWidth;
		Texture2D tex;
		if (useEndCap) {
			VectorLine.SetEndCap ("RoundCap", EndCap.Mirror, capLineTex, capTex);
			tex = capLineTex;
			useLineWidth = capLineWidth;
		}
		else {
			tex = lineTex;
			useLineWidth = lineWidth;
		}
		
		line = new VectorLine("DrawnLine", new List<Vector2>(), tex, useLineWidth, LineType.Continuous, Joins.Weld);
		line.endPointsUpdate = 2;	// Optimization for updating only the last couple points of the line, and the rest is not re-computed
		if (useEndCap) {
			line.endCap = "RoundCap";
		}
		// Used for .sqrMagnitude, which is faster than .magnitude
		sqrMinPixelMove = minPixelMove*minPixelMove;
	}
	
	void Update () {
		if (Input.touchCount > 0) {
			touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began) {
				line.points2.Clear();
				line.Draw();
				previousPosition = touch.position;
				line.points2.Add (touch.position);
				canDraw = true;
			}
			else if (touch.phase == TouchPhase.Moved && (touch.position - previousPosition).sqrMagnitude > sqrMinPixelMove && canDraw) {
				previousPosition = touch.position;
				line.points2.Add (touch.position);
				if (line.points2.Count >= maxPoints) {
					canDraw = false;
				}
				line.Draw();
			}
		}
	}
}