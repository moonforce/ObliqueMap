using UnityEngine;

public class CameraZoom : MonoBehaviour {

	public float zoomSpeed = 10.0f;
	public float keyZoomSpeed = 20.0f;
	
	void Update () {
		transform.Translate (Vector3.forward * zoomSpeed * Input.GetAxis ("Mouse ScrollWheel"));
		transform.Translate (Vector3.forward * keyZoomSpeed * Time.deltaTime * Input.GetAxis ("Vertical"));
	}
}