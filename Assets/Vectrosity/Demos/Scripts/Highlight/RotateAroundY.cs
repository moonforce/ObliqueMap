using UnityEngine;

public class RotateAroundY : MonoBehaviour {

	public float rotateSpeed = 10.0f;
	
	void Update () {
		transform.Rotate (Vector3.up * Time.deltaTime * rotateSpeed);
	}
}