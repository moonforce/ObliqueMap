using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class DrawPoints : MonoBehaviour {
	
	public float dotSize = 2.0f;
	public int numberOfDots = 100;
	public int numberOfRings = 8;
	public Color dotColor = Color.cyan;
	
	void Start () {
		var totalDots = numberOfDots * numberOfRings;
		var dotPoints = new Vector2[totalDots];
		var dotColors = new Color32[totalDots];
		
		var reduceAmount = 1.0f - .75f/totalDots;
		for (int i = 0; i < dotColors.Length; i++) {
			dotColors[i] = dotColor;
			dotColor *= reduceAmount;
		}
		
		var dots = new VectorLine("Dots", new List<Vector2>(dotPoints), dotSize, LineType.Points);
		dots.SetColors (new List<Color32>(dotColors));
		for (int i = 0; i < numberOfRings; i++) {
			dots.MakeCircle (new Vector2(Screen.width/2, Screen.height/2), Screen.height/(i+2), numberOfDots, numberOfDots*i);	
		}
		dots.Draw();
	}
}