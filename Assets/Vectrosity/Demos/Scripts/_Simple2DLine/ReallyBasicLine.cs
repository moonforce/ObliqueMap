using UnityEngine;
using Vectrosity;

public class ReallyBasicLine : MonoBehaviour {
	void Start () {
		// Draw a line from the lower-left corner to the upper-right corner
		VectorLine.SetLine (Color.white, new Vector2(0, 0), new Vector2(Screen.width-1, Screen.height-1));
	}
}