using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class CreateStars : MonoBehaviour {
	
	public int numberOfStars = 2000;
	private VectorLine stars;
	
	void Start () {
		// Make a bunch of points in a spherical distribution
		var starPoints = new Vector3[numberOfStars];
		for (int i = 0; i < numberOfStars; i++) {
			starPoints[i] = Random.onUnitSphere * 100.0f;
		}
		// Make each star have a size ranging from 1.5 to 2.5
		var starSizes = new float[numberOfStars];
		for (int i = 0; i < numberOfStars; i++) {
			starSizes[i] = Random.Range (1.5f, 2.5f);
		}
		// Make each star have a random shade of grey
		var starColors = new Color32[numberOfStars];
		for (int i = 0; i < numberOfStars; i++) {
			var greyValue = Random.value * .75f + .25f;
			starColors[i] = new Color(greyValue, greyValue, greyValue);
		}
		
		stars = new VectorLine("Stars", new List<Vector3>(starPoints), 1.0f, LineType.Points);
		stars.SetColors (new List<Color32>(starColors));
		stars.SetWidths (new List<float>(starSizes));
	
		stars.Draw();
		// We want the stars to be drawn behind 3D objects, like a skybox. So we use SetCanvasCamera,
		// which makes the canvas draw with RenderMode.OverlayCamera using Camera.main.
		// Note that SetCanvasCamera should be called after the VectorLine is drawn,
		// or else the VectorLine won't be drawn correctly.
		VectorLine.SetCanvasCamera (Camera.main);
		VectorLine.canvas.planeDistance = Camera.main.farClipPlane-1;
	}
	
	void LateUpdate () {
		stars.Draw();
	}
}