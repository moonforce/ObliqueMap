using UnityEngine;

public class RotateViewpoint : MonoBehaviour {

	public float rotateSpeed = 5.0f;
	
	void Update () {
		transform.RotateAround (Vector3.zero, Vector3.right, rotateSpeed * Time.deltaTime);
	}
}