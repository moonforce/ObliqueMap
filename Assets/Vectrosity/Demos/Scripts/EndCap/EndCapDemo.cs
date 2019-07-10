using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class EndCapDemo : MonoBehaviour {
	public Texture2D lineTex;
	public Texture2D lineTex2;
	public Texture2D lineTex3;
	public Texture2D frontTex;
	public Texture2D backTex;
	public Texture2D capTex;
	
	void Start () {
		VectorLine.SetEndCap ("arrow", EndCap.Front, lineTex, frontTex);
		VectorLine.SetEndCap ("arrow2", EndCap.Both, lineTex2, frontTex, backTex);
		VectorLine.SetEndCap ("rounded", EndCap.Mirror, lineTex3, capTex);
	
		var line1 = new VectorLine("Arrow", new List<Vector2>(50), 30.0f, LineType.Continuous, Joins.Weld);
		line1.useViewportCoords = true;
		var splinePoints = new Vector2[] {new Vector2(.1f, .15f), new Vector2(.3f, .5f), new Vector2(.5f, .6f), new Vector2(.7f, .5f), new Vector2(.9f, .15f)};
		line1.MakeSpline (splinePoints);
		line1.endCap = "arrow";
		line1.Draw();
	
		var line2 = new VectorLine("Arrow2", new List<Vector2>(50), 40.0f, LineType.Continuous, Joins.Weld);
		line2.useViewportCoords = true;
		splinePoints = new Vector2[] {new Vector2(.1f, .85f), new Vector2(.3f, .5f), new Vector2(.5f, .4f), new Vector2(.7f, .5f), new Vector2(.9f, .85f)};
		line2.MakeSpline (splinePoints);
		line2.endCap = "arrow2";
		line2.continuousTexture = true;
		line2.Draw();
		
		var line3 = new VectorLine("Rounded", new List<Vector2>{new Vector2(.1f, .5f), new Vector2(.9f, .5f)}, 20.0f);
		line3.useViewportCoords = true;
		line3.endCap = "rounded";
		line3.Draw();
	}
}