using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class CreateHills : MonoBehaviour {

	public Texture hillTexture;
	public PhysicsMaterial2D hillPhysicsMaterial ;
	public int numberOfPoints = 100;
	public int numberOfHills = 4;
	public GameObject ball;
	private Vector3 storedPosition;
	private VectorLine hills;
	private Vector2[] splinePoints;
	
	void Start () {
		storedPosition = ball.transform.position;
		splinePoints = new Vector2[numberOfHills*2 + 1];
		
		hills = new VectorLine("Hills", new List<Vector2>(numberOfPoints), hillTexture, 12.0f, LineType.Continuous, Joins.Weld);
		hills.useViewportCoords = true;
		hills.collider = true;
		hills.physicsMaterial = hillPhysicsMaterial;
		
#if UNITY_5_2 || UNITY_5_3
		Random.seed = 95;
#else
		Random.InitState (95);
#endif
		CreateHillLine();
	}
	
	void OnGUI () {
		if (GUI.Button (new Rect(10, 10, 150, 40), "Make new hills")) {
			CreateHillLine();
			ball.transform.position = storedPosition;
			ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			ball.GetComponent<Rigidbody2D>().WakeUp();
		}
	}
	
	void CreateHillLine () {
		splinePoints[0] = new Vector2(-0.02f, Random.Range (0.1f, 0.6f));
		var x = 0.0f;
		var distance = 1.0f / (numberOfHills * 2);
		int i;
		for (i = 1; i < splinePoints.Length; i += 2) {
			x += distance;
			splinePoints[i  ] = new Vector2(x, Random.Range (0.3f, 0.7f));
			x += distance;
			splinePoints[i+1] = new Vector2(x, Random.Range (0.1f, 0.6f));
		}
		splinePoints[i-1] = new Vector2(1.02f, Random.Range (0.1f, 0.6f));
		
		hills.MakeSpline (splinePoints);
		hills.Draw();
	}
}