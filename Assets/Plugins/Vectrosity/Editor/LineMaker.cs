// Version 5.5
// ©2017 Starscene Software. All rights reserved. Redistribution without permission not allowed.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Vectrosity;

public class LineMaker : ScriptableWizard {
	Transform objectTransform;
	Transform[] pointObjects;
	List<GameObject> lines;
	List<Vector3Pair> linePoints;
	static float pointScale = .2f;
	static float lineScale = .1f;
	static float oldPointScale = .2f;
	static float oldLineScale = .1f;
	string message = "";
	List<Vector3> linePositions;
	List<Quaternion> lineRotations;
	Material objectMaterial;
	Material originalMaterial;
	Material pointMaterial;
	Material lineMaterial;
	Mesh objectMesh;
	static Vector3 objectScale;
	bool initialized = false;
	int idx;
	int endianDiff1;
	int endianDiff2;
	bool canUseVector2 = true;
	bool useVector2 = false;
	List<int> selectedSegments;
	string selectedSegmentsString;
	byte[] byteBlock;
	Vector3[] vectorArray;
	bool loadedFile;
	static LineMaker window;
	
	[MenuItem ("Assets/Line Maker... %l")]
	static void CreateWizard () {
		var go = (Selection.activeObject as GameObject);
		if (!go) {
			EditorUtility.DisplayDialog ("No object selected", "Please select an object in the scene hierarchy", "Cancel");
			return;
		}
		var mf = go.GetComponentInChildren<MeshFilter>();
		if (!mf) {
			EditorUtility.DisplayDialog ("No MeshFilter present", "Object must have a MeshFilter component", "Cancel");
			return;
		}
		var objectMesh = mf.sharedMesh as Mesh;
		if (!objectMesh) {
			EditorUtility.DisplayDialog ("No mesh present", "Object must have a mesh", "Cancel");
			return;
		}
		if (objectMesh.vertexCount > 2000) {
			EditorUtility.DisplayDialog ("Too many vertices", "Please select a low-poly object", "Cancel");
			return;
		}
		objectScale = go.transform.localScale;
		objectScale = new Vector3(1.0f/objectScale.x, 1.0f/objectScale.y, 1.0f/objectScale.z);
		
		window = ScriptableWizard.DisplayWizard<LineMaker>("Line Maker");
		window.minSize = new Vector2(360, 245);
	}
	
	// Initialize this way so we don't have to use static vars, so multiple wizards can be used at once with different objects
	void OnWizardUpdate () {
		if (!initialized) {
			Initialize();
		}
	}
	
	void Initialize () {
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
		var go = (Selection.activeObject as GameObject);
		objectMesh = go.GetComponentInChildren<MeshFilter>().sharedMesh;	
		originalMaterial = go.GetComponentInChildren<Renderer>().sharedMaterial;
		objectMaterial = new Material(Shader.Find ("Transparent/Diffuse"));
		var col = objectMaterial.color;
		col.a = .8f;
		objectMaterial.color = col;
		go.GetComponentInChildren<Renderer>().material = objectMaterial;
		pointMaterial = new Material(Shader.Find ("VertexLit"));
		pointMaterial.color = Color.blue;
		pointMaterial.SetColor ("_Emission", Color.blue);
		pointMaterial.SetColor ("_SpecColor", Color.blue);
		lineMaterial = new Material(Shader.Find ("VertexLit"));
		lineMaterial.color = Color.green;
		lineMaterial.SetColor ("_Emission", Color.green);
		lineMaterial.SetColor ("_SpecColor", Color.green);
		
		var meshVertices = objectMesh.vertices;
		// Remove duplicate vertices
		var meshVerticesList = new List<Vector3>(meshVertices);
		for (int i = 0; i < meshVerticesList.Count-1; i++) {
			int j = i+1;
			while (j < meshVerticesList.Count) {
				if (meshVerticesList[i] == meshVerticesList[j]) {
					meshVerticesList.RemoveAt(j);
				}
				else {
					j++;
				}
			}
		}
		meshVertices = meshVerticesList.ToArray();
		
		// See if z is substantially different on any points, in which case the option to generate a Vector2 array will not be available
		float zCoord = (float)System.Math.Round(meshVertices[0].z, 3);
		for (int i = 1; i < meshVertices.Length; i++) {
			if (!Mathf.Approximately ((float)System.Math.Round(meshVertices[i].z, 3), zCoord)) {
				canUseVector2 = false;
				break;
			}
		}

		// Create the blue point sphere widgets
		objectTransform = go.transform;
		var objectMatrix = objectTransform.localToWorldMatrix;
		pointObjects = new Transform[meshVertices.Length];
		for (int i = 0; i < pointObjects.Length; i++) {
			pointObjects[i] = GameObject.CreatePrimitive (PrimitiveType.Sphere).transform;
			pointObjects[i].position = objectMatrix.MultiplyPoint3x4 (meshVertices[i]);
			pointObjects[i].parent = objectTransform;
			pointObjects[i].localScale = objectScale * pointScale;
			pointObjects[i].GetComponent<Renderer>().sharedMaterial = pointMaterial;
		}
		
		lines = new List<GameObject>();
		linePoints = new List<Vector3Pair>();
		linePositions = new List<Vector3>();
		lineRotations = new List<Quaternion>();
		endianDiff1 = System.BitConverter.IsLittleEndian? 0 : 3;
		endianDiff2 = System.BitConverter.IsLittleEndian? 0 : 1;
		Selection.objects = new UnityEngine.Object[0];	// Deselect object so it's not in the way as much
		selectedSegmentsString = " ";
		byteBlock = new byte[4];
		initialized = true;
	}
	
	void OnFocus () {
		for (int i = 0; i < lines.Count; i++) {
			GameObject line = lines[i];
			// Make sure line segment is where it's supposed to be in case it was moved
			if (line) {
				line.transform.position = linePositions[i];
				line.transform.rotation = lineRotations[i];
			}
			// But if it's null, then the user must have trashed it, so delete the line
			else {
				DeleteLine(i);
			}
		}
		
		// See which line segments are selected, and make a string that lists the line segment indices
		var selectionIDs = Selection.instanceIDs;
		selectedSegments = new List<int>();
		for (int i = 0; i < lines.Count; i++) {
			for (int j = 0; j < selectionIDs.Length; j++) {
				if ((lines[i] as GameObject).GetInstanceID() == selectionIDs[j]) {
					selectedSegments.Add(i);
				}
			}
		}
		if (selectedSegments.Count > 0) {
			selectedSegmentsString = "Selected line segment " + ((selectedSegments.Count > 1)? "indices: " : "index: ");
			for (int i = 0; i < selectedSegments.Count; i++) {
				selectedSegmentsString += selectedSegments[i];
				if (i < selectedSegments.Count-1) {
					selectedSegmentsString += ", ";
				}
			}
		}
		else {
			selectedSegmentsString = " ";
		}
	}
	
	private void OnLostFocus () {
		selectedSegmentsString = " ";
	}
	
	private void Update () {
		if (EditorApplication.isCompiling || EditorApplication.isPlaying) {
			OnDestroy();	// Otherwise, compiling scripts makes created objects not get destroyed properly
		}
	}

	private void OnGUI () {
		GUILayout.BeginHorizontal();
			GUILayout.Label ("Object: \"" + objectTransform.name + "\"");
			if (GUILayout.Button ("Load Line File")) {
				LoadLineFile();
			}
		GUILayout.EndHorizontal();
		GUILayout.Space (5);
	
		GUILayout.BeginHorizontal();
			GUILayout.Label ("Point size: ", GUILayout.Width(60));
			pointScale = GUILayout.HorizontalSlider (pointScale, .025f, 1.0f);
			if (pointScale != oldPointScale) {
				SetPointScale();
			}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
			GUILayout.Label ("Line size: ", GUILayout.Width(60));
			lineScale = GUILayout.HorizontalSlider (lineScale, .0125f, .5f);
			if (lineScale != oldLineScale) {
				SetLineScale();
			}
		GUILayout.EndHorizontal();
		GUILayout.Space (10);
	
		GUILayout.BeginHorizontal();
		int buttonWidth = (int)window.position.width/2 - 6;
		if (GUILayout.Button ("Make Line Segment", GUILayout.Width(buttonWidth))) {
			message = "";
			var selectionIDs = Selection.instanceIDs;
			if (selectionIDs.Length == 2) {
				if (CheckPointID (selectionIDs[0]) && CheckPointID (selectionIDs[1])) {
					CreateLine();
				}
				else {
					message = "Must select vertex points from this object";
				}
			}
			else {
				message = "Must have two points selected";
			}
		}
		if (GUILayout.Button ("Delete Line Segments", GUILayout.Width(buttonWidth))) {
			message = "";
			var selectionIDs = Selection.instanceIDs;
			if (selectionIDs.Length == 0) {
				message = "Must select line segment(s) to delete";
			}
			for (int i = 0; i < selectionIDs.Length; i++) {
				var lineNumber = CheckLineID(selectionIDs[i]);
				if (lineNumber != -1) {
					DeleteLine (lineNumber);
				}
				else {
					message = "Only lines from this object can be deleted";
				}
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space (5);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button (loadedFile? "Restore Loaded Lines" : "Connect All Points", GUILayout.Width(buttonWidth))) {
			message = "";
			if (objectMesh.triangles.Length == 0) {
				message = "No triangles exist...must connect points manually";
			}
			else {
				DeleteAllLines();
				ConnectAllPoints();
			}
		}
		if (GUILayout.Button ("Delete All Line Segments", GUILayout.Width(buttonWidth))) {
			message = "";
			DeleteAllLines();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space (5);
		
		GUILayout.Label (selectedSegmentsString);
		GUILayout.Space (5);

		GUILayout.Label (message);
		GUILayout.Space (5);
		
		GUI.enabled = canUseVector2;
		useVector2 = GUILayout.Toggle (useVector2, "Use Vector2 points");
		
		GUI.enabled = (lines.Count > 0);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Generate Line (Unityscript)", GUILayout.Width(buttonWidth))) {
			message = "";
			ComputeLine (false);
		}
		if (GUILayout.Button ("Generate Line (C#)", GUILayout.Width(buttonWidth))) {
			message = "";
			ComputeLine (true);
		}
		GUILayout.EndHorizontal();
		GUILayout.Space (2);
		
		if (GUILayout.Button ("Write Complete Line to File")) {
			WriteLineToFile();
		}
	}
	
	private bool CheckPointID (int thisID) {
		for (int i = 0; i < pointObjects.Length; i++) {
			if (pointObjects[i].gameObject.GetInstanceID() == thisID) {
				return true;
			}
		}
		return false;
	}

	private int CheckLineID (int thisID) {
		for (int i = 0; i < lines.Count; i++) {
			if ((lines[i] as GameObject).GetInstanceID() == thisID) {
				return i;
			}
		}
		return -1;
	}
	
	private void CreateLine () {
		var selectedObjects = Selection.gameObjects;
		Line (selectedObjects[0].transform.position, selectedObjects[1].transform.position);
	}
	
	private void Line (Vector3 p1, Vector3 p2) {
		// Make a cube midway between the two points, scaled and rotated so it connects them
		var line = GameObject.CreatePrimitive (PrimitiveType.Cube);
		line.transform.position = new Vector3( (p1.x + p2.x)/2, (p1.y + p2.y)/2, (p1.z + p2.z)/2 );
		line.transform.localScale = new Vector3(lineScale, lineScale, Vector3.Distance(p1, p2));
		line.transform.LookAt(p1);
		line.transform.parent = objectTransform;
		line.GetComponent<Renderer>().sharedMaterial = lineMaterial;
		lines.Add (line);
		linePoints.Add (new Vector3Pair(p1, p2));
		linePositions.Add (line.transform.position);
		lineRotations.Add (line.transform.rotation);
	}
	
	private void DeleteLine (int lineID) {
		message = "";
		var thisLine = lines[lineID];
		lines.RemoveAt (lineID);
		linePoints.RemoveAt (lineID);
		linePositions.RemoveAt (lineID);
		lineRotations.RemoveAt (lineID);
		if (thisLine) {
			DestroyImmediate (thisLine);
		}
		selectedSegmentsString = " ";
	}
	
	private void ConnectAllPoints () {
		if (!loadedFile) {
			var meshVertices = objectMesh.vertices;
			var objectMatrix = objectTransform.localToWorldMatrix;
			var pairs = new Dictionary<Vector3Pair, bool>();
			
			for (int i = 0; i < objectMesh.subMeshCount; i++) {
				var meshIndices = objectMesh.GetIndices (i);
				var polyCount = (objectMesh.GetTopology (i) == MeshTopology.Triangles)? 3 : 4;
				
				for (int j = 0; j < meshIndices.Length; j += polyCount) {
					for (int k = 0; k < polyCount; k++) {
						var p1 = meshVertices[meshIndices[j + k]];
						var p2 = meshVertices[meshIndices[j + (k+1)%polyCount]];
						CheckPoints (pairs, p1, p2, objectMatrix);
					}
				}
			}
		}
		else {
			ConnectLoadedLines();
		}
	}
	
	private void CheckPoints (Dictionary<Vector3Pair, bool> pairs, Vector3 p1, Vector3 p2, Matrix4x4 objectMatrix) {
		// Only add a line if the two points haven't been connected yet, so there are no duplicate lines
		var pair1 = new Vector3Pair(p1, p2);
		var pair2 = new Vector3Pair(p2, p1);
		if (!pairs.ContainsKey (pair1) && !pairs.ContainsKey (pair2)) {
			pairs[pair1] = true;
			pairs[pair2] = true;
			Line (objectMatrix.MultiplyPoint3x4 (p1), objectMatrix.MultiplyPoint3x4 (p2));
		}
	}
	
	private void DeleteAllLines () {
		if (lines == null) return;
		
		foreach (GameObject line in lines) {
			DestroyImmediate (line);
		}
		lines = new List<GameObject>();
		linePoints = new List<Vector3Pair>();
		linePositions = new List<Vector3>();
		lineRotations = new List<Quaternion>();
		selectedSegmentsString = " ";
	}
	
	private void ComputeLine (bool useCsharp) {
		var floatSuffix = useCsharp? "f" : "";
		var newPrefix = useCsharp? "new " : "";
		var startChar = useCsharp? "" : "[";
		var endChar = useCsharp? "" : "]";
		var output = startChar;
		for (int i = 0; i < linePoints.Count; i++) {
			var p1 = Vector3Round(linePoints[i].p1 - objectTransform.position);
			var p2 = Vector3Round(linePoints[i].p2 - objectTransform.position);
			if (!useVector2) {
				output += newPrefix + "Vector3(" + p1.x + floatSuffix + ", " + p1.y + floatSuffix + ", " + p1.z + floatSuffix + "), "
						+ newPrefix + "Vector3(" + p2.x + floatSuffix + ", " + p2.y + floatSuffix + ", " + p2.z + floatSuffix + ")";
			}
			else {
				output += newPrefix + "Vector2(" + p1.x + floatSuffix + ", " + p1.y + floatSuffix + "), "
						+ newPrefix + "Vector2(" + p2.x + floatSuffix + ", " + p2.y + floatSuffix + ")";
			}
			if (i < linePoints.Count-1) {
				output += ", ";
			}
		}
		output += endChar;
		EditorGUIUtility.systemCopyBuffer = output;
		message = "Vector line sent to copy buffer. Please paste into a script now.";
	}
	
	Vector3 Vector3Round (Vector3 p) {
		// Round to 3 digits after the decimal so we don't get annoying and unparseable floating point values like -5E-05
		return new Vector3((float)System.Math.Round (p.x, 3), (float)System.Math.Round (p.y, 3), (float)System.Math.Round (p.z, 3));
	}
	
	private void WriteLineToFile () {
		var path = EditorUtility.SaveFilePanelInProject("Save " + (useVector2? "Vector2" : "Vector3") + " Line", objectTransform.name+"Vector.bytes", "bytes", 
														"Please enter a file name for the line data");
		if (path == "") return;
		
		var fileBytes = new byte[(useVector2? linePoints.Count*16 : linePoints.Count*24)];
		idx = 0;
		for (int i = 0; i < linePoints.Count; i++) {
			Vector3Pair v = linePoints[i];
			Vector3 p = v.p1 - objectTransform.position;
			ConvertFloatToBytes (p.x, fileBytes);
			ConvertFloatToBytes (p.y, fileBytes);
			if (!useVector2) {
				ConvertFloatToBytes (p.z, fileBytes);
			}
			p = v.p2 - objectTransform.position;
			ConvertFloatToBytes (p.x, fileBytes);
			ConvertFloatToBytes (p.y, fileBytes);
			if (!useVector2) {
				ConvertFloatToBytes (p.z, fileBytes);
			}
		}
		
		try {
			File.WriteAllBytes (path, fileBytes);
			AssetDatabase.Refresh();
		}
		catch (System.Exception err) {
			message = err.Message;
			return;
		}
		message = "File written successfully";
	}
	
	private void ConvertFloatToBytes (float f, byte[] bytes) {
		var floatBytes = System.BitConverter.GetBytes (f);
		bytes[idx++] = floatBytes[    endianDiff1];
		bytes[idx++] = floatBytes[1 + endianDiff2];
		bytes[idx++] = floatBytes[2 - endianDiff2];
		bytes[idx++] = floatBytes[3 - endianDiff1];
	}
	
	private float ConvertBytesToFloat (byte[] bytes, int i) {
		byteBlock[    endianDiff1] = bytes[i  ];
		byteBlock[1 + endianDiff2] = bytes[i+1];
		byteBlock[2 - endianDiff2] = bytes[i+2];
		byteBlock[3 - endianDiff1] = bytes[i+3];
		return System.BitConverter.ToSingle (byteBlock, 0);
	}
	
	private void SetPointScale () {
		oldPointScale = pointScale;
		foreach (Transform po in pointObjects) {
			po.localScale = objectScale * pointScale;
		}
	}

	private void SetLineScale () {
		oldLineScale = lineScale;
		foreach (GameObject line in lines) {
			line.transform.localScale = new Vector3(lineScale*objectScale.x, lineScale*objectScale.y, line.transform.localScale.z);
		}
	}
	
	private void LoadLineFile () {
		var path = EditorUtility.OpenFilePanel ("Load line", Application.dataPath, "bytes");
		if (path == "") return;
		
		var bytes = File.ReadAllBytes(path);
		if ((canUseVector2 && bytes.Length % 16 != 0) || (!canUseVector2 && bytes.Length % 24 != 0)) {
			EditorUtility.DisplayDialog ("Incorrect file length", "File does not seem to be a " + (canUseVector2? "2" : "3") + "D line file", "Cancel");
			return;
		}
		loadedFile = true;
		
		// Convert bytes to Vector3 array
		vectorArray = new Vector3[bytes.Length / (canUseVector2? 8 : 12)];
		int vectorType = canUseVector2? 2 : 3;
		int count = 0;
		for (int i = 0; i < bytes.Length; i += 4 * vectorType) {
			vectorArray[count++] = new Vector3(ConvertBytesToFloat (bytes, i), ConvertBytesToFloat (bytes, i+4), (canUseVector2? 0 : ConvertBytesToFloat (bytes, i+8)));
		}
		
		// Remove duplicate points
		var points = new List<Vector3>(vectorArray);
		for (int i = 0; i < points.Count-1; i++) {
			for (int j = i+1; j < points.Count; j++) {
				if (points[i] == points[j]) {
					points.RemoveAt (j--);
				}
			}
		}
		
		// Create the blue point sphere widgets
		foreach (Transform po in pointObjects) {
			if (po != null) DestroyImmediate (po.gameObject);
		}
		pointObjects = new Transform[points.Count];
		for (int i = 0; i < points.Count; i++) {
			pointObjects[i] = GameObject.CreatePrimitive (PrimitiveType.Sphere).transform;
			pointObjects[i].position = points[i] + objectTransform.position;
			pointObjects[i].parent = objectTransform;
			pointObjects[i].localScale = objectScale * pointScale;
			pointObjects[i].GetComponent<Renderer>().sharedMaterial = pointMaterial;
		}
		
		ConnectLoadedLines();
	}
	
	private void ConnectLoadedLines () {
		for (int i = 0; i < vectorArray.Length; i += 2) {
			Line (vectorArray[i] + objectTransform.position, vectorArray[i+1] + objectTransform.position);
		}
	}

	private void OnDestroy () {
		foreach (Transform po in pointObjects) {
			if (po != null) DestroyImmediate (po.gameObject);
		}
		DeleteAllLines();
		objectTransform.GetComponentInChildren<Renderer>().material = originalMaterial;
		DestroyImmediate (objectMaterial);
		DestroyImmediate (pointMaterial);
		DestroyImmediate (lineMaterial);
	}
}