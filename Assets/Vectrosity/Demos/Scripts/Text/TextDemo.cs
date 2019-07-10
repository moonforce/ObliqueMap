using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class TextDemo : MonoBehaviour {

	public string text = "Vectrosity!";
	public int textSize = 40;
	private VectorLine textLine;
	
	void Start () {
		textLine = new VectorLine("Text", new List<Vector2>(), 1.0f);
		textLine.color = Color.yellow;
		textLine.drawTransform = transform;
		textLine.MakeText (text, new Vector2(Screen.width/2 - text.Length*textSize/2, Screen.height/2 + textSize/2), textSize);
	}
	
	void Update () {
		transform.RotateAround (new Vector2(Screen.width/2, Screen.height/2), Vector3.forward, Time.deltaTime * 45.0f);
		var v3 = transform.localScale;
		v3.x = 1 + Mathf.Sin (Time.time*3)*.3f;
		v3.y = 1 + Mathf.Cos (Time.time*3)*.3f;
		transform.localScale = v3;
		textLine.Draw();
	}
}