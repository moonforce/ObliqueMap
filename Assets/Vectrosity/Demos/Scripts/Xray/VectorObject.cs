using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class VectorObject : MonoBehaviour {

	public enum Shape {Cube = 0, Sphere = 1}
	public Shape shape = Shape.Cube;
	
	void Start () {
		var line = new VectorLine ("Shape", XrayLineData.use.shapePoints[(int)shape], XrayLineData.use.lineTexture, XrayLineData.use.lineWidth);
		line.color = Color.green;
		VectorManager.ObjectSetup (gameObject, line, Visibility.Always, Brightness.None);
	}
}