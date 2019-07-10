// Version 5.4
// ©2017 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using Vectrosity;

public class UIVector2D : MonoBehaviour {
	[MenuItem("GameObject/UI/VectorLine #&v")]
	static void CreateLine () {
		var points2 = new List<Vector2>(){new Vector2(Screen.width/2 - 100, Screen.height/2), new Vector2(Screen.width/2 + 100, Screen.height/2)};
		var line = new VectorLine("Line", points2, 2.0f, LineType.Continuous);
		
		Undo.RegisterCreatedObjectUndo (line.rectTransform.gameObject, "Create VectorLine");
		
		var useVectorCanvas = true;
		var selectedObj = Selection.activeObject as GameObject;
		if (selectedObj != null) {
			var canvas = selectedObj.GetComponent<Canvas>();
			if (canvas != null && canvas.renderMode != RenderMode.WorldSpace) {
				line.SetCanvas (canvas);
				useVectorCanvas = false;
			}
		}
		line.Draw();
		
		if (useVectorCanvas) {
			if (VectorLine.canvas.gameObject.GetComponent<CanvasScaler>() == null) {
				VectorLine.canvas.gameObject.AddComponent<CanvasScaler>();
			}
		}
		
		Selection.activeGameObject = line.rectTransform.gameObject;
	}
}