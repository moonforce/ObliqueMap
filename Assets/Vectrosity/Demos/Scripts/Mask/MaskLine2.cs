using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class MaskLine2 : MonoBehaviour {
	
	public int numberOfPoints = 100;
	public Color lineColor = Color.yellow;
	public GameObject mask;
	public float lineWidth = 9.0f;
	public float lineHeight = 17.0f;
	private VectorLine spikeLine;
	private float t = 0.0f;
	private Vector3 startPos;
	
	void Start () {
		spikeLine = new VectorLine("SpikeLine", new List<Vector3>(numberOfPoints), 2.0f, LineType.Continuous);
		float y = lineHeight / 2;
		for (int i = 0; i < numberOfPoints; i++) {
			spikeLine.points3[i] = new Vector2(Random.Range(-lineWidth/2, lineWidth/2), y);
			y -= lineHeight / numberOfPoints;
		}
		spikeLine.color = lineColor;
		spikeLine.drawTransform = transform;
		spikeLine.SetMask (mask);
		
		startPos = transform.position;
	}
	
	void Update () {
		// Move this transform around in a circle, and the line uses the same movement since it's using this transform with .drawTransform
		t = Mathf.Repeat (t + Time.deltaTime, 360.0f);
		transform.position = new Vector2(startPos.x, startPos.y + Mathf.Cos (t) * 4);
		spikeLine.Draw();
	}
}