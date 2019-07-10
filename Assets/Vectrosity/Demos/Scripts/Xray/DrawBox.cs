// This script is put on a plane using a depthmask shader. There are two cameras: the main camera on top that sees all layers except the UI layer,
// and a vector camera underneath that sees only the UI layer. By moving and resizing this depthmask plane, a "window" into
// the vector cam can be made. Since the vector objects are synced to the normal objects and the two cams are in the same position,
// an x-ray like effect is created.
using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class DrawBox : MonoBehaviour {

	public float moveSpeed = 1.0f;
	public float explodePower = 20.0f;
	public Camera vectorCam;
	private bool mouseDown = false;
	private Rigidbody[] rigidbodies;
	private bool canClick = true;
	private bool boxDrawn = false;
	
	IEnumerator Start () {
		GetComponent<Renderer>().enabled = false;
		rigidbodies = FindObjectsOfType (typeof(Rigidbody)) as Rigidbody[];
		VectorLine.canvas.planeDistance = .5f;
		
		// When using SetCanvasCamera, lines should be drawn first or else they don't appear correctly, so wait a frame for that to happen
		yield return null;
		VectorLine.SetCanvasCamera (vectorCam);
	}
	
	void Update () {
		var mousePos = Input.mousePosition;
		mousePos.z = 1.0f;
		var worldPos = Camera.main.ScreenToWorldPoint (mousePos);
		
		if (Input.GetMouseButtonDown (0) && canClick) {
			GetComponent<Renderer>().enabled = true;
			transform.position = worldPos;
			mouseDown = true;
		}
		
		if (mouseDown) {
			transform.localScale = new Vector3(worldPos.x - transform.position.x, worldPos.y - transform.position.y, 1.0f);
		}
		
		if (Input.GetMouseButtonUp (0)) {
			mouseDown = false;
			boxDrawn = true;
		}
		
		transform.Translate (-Vector3.up * Time.deltaTime * moveSpeed * Input.GetAxis ("Vertical"));
		transform.Translate (Vector3.right * Time.deltaTime * moveSpeed * Input.GetAxis ("Horizontal"));
	}
	
	void OnGUI () {
		GUI.Box (new Rect(20, 20, 320, 38), "Draw a box by clicking and dragging with the mouse\nMove the drawn box with the arrow keys");
		var buttonRect = new Rect(20, 62, 60, 30);
		// Prevent mouse button click in Update from working if mouse is over the "boom" button
		canClick = (buttonRect.Contains (Event.current.mousePosition)? false : true);
		// The "boom" button is only drawn after a box is made, so users don't get distracted by the physics first ;)
		if (boxDrawn && GUI.Button (buttonRect, "Boom!")) {
			foreach (var body in rigidbodies) {
				body.AddExplosionForce (explodePower, new Vector3(0.0f, -6.5f, -1.5f), 20.0f, 0.0f, ForceMode.VelocityChange);
			}
		}
	}
}