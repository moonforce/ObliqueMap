using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class MakeSpline : MonoBehaviour {
	
	public int segments = 250;
	public bool loop = true;
	public bool usePoints = false;
	
	void Start () {
		var splinePoints = new List<Vector3>();
		var i = 1;
		var obj = GameObject.Find ("Sphere"+(i++));
		while (obj != null) {
			splinePoints.Add (obj.transform.position);
			obj = GameObject.Find ("Sphere"+(i++));
		}
	
		if (usePoints) {
			var dotLine = new VectorLine("Spline", new List<Vector3>(segments+1), 2.0f, LineType.Points);
			dotLine.MakeSpline (splinePoints.ToArray(), segments, loop);
			dotLine.Draw();
		}
		else {
			var spline = new VectorLine("Spline", new List<Vector3>(segments+1), 2.0f, LineType.Continuous);
			spline.MakeSpline (splinePoints.ToArray(), segments, loop);
			spline.Draw3D();
		}
	}
}