using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class SplineFollow2D : MonoBehaviour {
	
	public int segments = 250;
	public bool loop = true;
	public Transform cube;
	public float speed = .05f;
	
	IEnumerator Start () {
		var splinePoints = new List<Vector2>();
		var i = 1;
		var obj = GameObject.Find ("Sphere"+(i++));
		while (obj != null) {
			splinePoints.Add (Camera.main.WorldToScreenPoint (obj.transform.position));
			obj = GameObject.Find ("Sphere"+(i++));
		}
	
		var line = new VectorLine("Spline", new List<Vector2>(segments+1), 2.0f, LineType.Continuous);
		line.MakeSpline (splinePoints.ToArray(), segments, loop);
		line.Draw();
		
		// Make the cube "ride" the spline at a constant speed
		do {
			for (var dist = 0.0f; dist < 1.0f; dist += Time.deltaTime*speed) {
				var splinePoint = line.GetPoint01 (dist);
				cube.position = Camera.main.ScreenToWorldPoint (new Vector3(splinePoint.x, splinePoint.y, 10.0f));
				yield return null;
			}
		} while (loop);
	}
}