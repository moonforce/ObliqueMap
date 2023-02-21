using UnityEngine;

public class CurvePointControl : MonoBehaviour {

	public int objectNumber;
	public GameObject controlObject;
	public GameObject controlObject2;
	
	void OnMouseDrag () {
		transform.position = DrawCurve.cam.ScreenToViewportPoint (Input.mousePosition);
		DrawCurve.use.UpdateLine (objectNumber, Input.mousePosition, gameObject);
	}
}