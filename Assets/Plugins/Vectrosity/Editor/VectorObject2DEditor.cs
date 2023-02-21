// Version 5.5
// ©2017 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

#if UNITY_5_4 || UNITY_5_5
#define OLDUNITY
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using Vectrosity;

[CustomEditor(typeof(VectorObject2D))]
public class VectorObject2DEditor : Editor {
	
	class Segment {
		public Color32 color;
		public float width;
		
		public Segment (Color32 color, float width) {
			this.color = color;
			this.width = width;
		}
	}
	
	class Point {	// Since Vector2 can't directly modify x or y
		public float x;
		public float y;
		
		public Point (float x, float y) {
			this.x = x;
			this.y = y;
		}
	}

	class Point3 {	// Since Vector3 can't directly modify x, y, or z
		public float x;
		public float y;
		public float z;
		
		public Point3 (float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
	
	VectorObject2D vobject;
	VectorLine vline;
	
	Color32 color;
	Texture texture;
	float capLength;
	float lineWidth;
	Joins joins;
	LineType lineType;
	float drawStart;
	float drawEnd;
	float textureScale;
	float textureOffset;
	bool collider;
	bool smoothWidth;
	bool smoothColor;
	
	bool useTextureScale;
	bool oldUseTextureScale;
	float storedTextureScale;
	
	bool usePartialLine;
	bool oldUsePartialLine;
	
	static bool showStyle = true;
	static bool showTexture = true;
	static bool showPartial = true;
	static bool showPoints = false;
	static bool showPoints3D = false;
	static bool showColors = false;
	static bool showWidths = false;
	
	static bool showPointCoords;
	static bool oldShowPointCoords;
	static float scenePointSize;
	static float oldScenePointSize;
		
	static Vector3 v3right = Vector3.right;
	static Vector3 v3forward = Vector3.forward;
	static Vector3 v3up = Vector3.up;
	static Vector2 v2right = Vector2.right;
	static GUILayoutOption width40 = GUILayout.Width(40);
	static GUILayoutOption width60 = GUILayout.Width(60);
	static GUILayoutOption height19 = GUILayout.Height(19);
	static GUIStyle sceneLabel;
	static GUIStyle infoLabel;
	static GUIStyle wrapLabel;
	static Color handlesColor = new Color(1, 1, 1, .5f);
	
	List<Point> points;
	List<Point3> points3;
	List<Segment> segments;
	
	string nameString;
	static string info = "Press shift to add point, control to delete point, alt to move line";
	
	bool mouseDown = false;
	bool controlActive = false;
	bool showWarning;
	string warnMessage;
	Transform canvasTransform;
		
	private void OnEnable () {
		vobject = (target as VectorObject2D);
		vline = vobject.vectorLine;
		
		smoothWidth = vline.smoothWidth;
		smoothColor = vline.smoothColor;
		color = vline.color;
		texture = vline.texture;
		capLength = vline.capLength;
		lineWidth = vline.lineWidth;
		joins = vline.joins;
		lineType = vline.lineType;
		drawStart = vline.drawStart;
		drawEnd = vline.drawEnd;
		textureScale = vline.textureScale;
		textureOffset = vline.textureOffset;
		collider = vline.collider;
		
		showPointCoords = oldShowPointCoords = EditorPrefs.GetBool ("VectrosityShowPointCoords", false);
		scenePointSize = oldScenePointSize = EditorPrefs.GetFloat ("VectrosityScenePointSize", 15.0f);
		
		useTextureScale = oldUseTextureScale = (vline.textureScale > 0.0f);
		if (useTextureScale) {
			storedTextureScale = vline.textureScale;
		}
		
		if (vline.is2D) {
			int pointCount = vline.points2.Count;
			usePartialLine = (drawStart > 0 || drawEnd < pointCount-1);
			
			points = new List<Point>(pointCount);
			for (int i = 0; i < pointCount; i++) {
				points.Add (new Point(vline.points2[i].x, vline.points2[i].y));
			}
		}
		else {
			int pointCount = vline.points3.Count;
			usePartialLine = (drawStart > 0 || drawEnd < pointCount-1);
			
			points3 = new List<Point3>(pointCount);
			for (int i = 0; i < pointCount; i++) {
				points3.Add (new Point3(vline.points3[i].x, vline.points3[i].y, vline.points3[i].z));
			}			
		}
		
		SetSegmentLists();
		nameString = vline.name + ":";
		
		showWarning = false;
		var canvas = vobject.rectTransform.root.GetComponent<Canvas>();
		if (canvas != null) {
			if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
				SetWarning ("Only canvases using Screen Space Overlay are supported");
			}
			canvasTransform = canvas.transform;
		}
		else {
			SetWarning ("Line must be attached to a canvas");
		}
	}
	
	private void SetWarning (string message) {
		warnMessage = message;
		showWarning = true;
	}
	
	private void SetSegmentLists () {
		int segmentNumber = vline.GetSegmentNumber();
		segments = new List<Segment>(segmentNumber);
		for (int i = 0; i < segmentNumber; i++) {
			segments.Add (new Segment(vline.GetColor (i), vline.GetWidth (i)) );
		}
	}
	
	public override void OnInspectorGUI() {
		showStyle = EditorGUILayout.Foldout (showStyle, "Style");
		if (showStyle) {
			ShowStyle();
		}
		showTexture = EditorGUILayout.Foldout (showTexture, "Texture");
		if (showTexture) {
			ShowTexture();
		}
		showPartial = EditorGUILayout.Foldout (showPartial, "Partial Line");
		if (showPartial) {
			ShowPartial();
		}
		if (vline.is2D) {
			showPoints = EditorGUILayout.Foldout (showPoints, "Line Points");
			if (showPoints) {
				ShowPoints();
			}
		}
		else {
			showPoints3D = EditorGUILayout.Foldout (showPoints3D, "Line Points");
			if (showPoints3D) {
				ShowPoints3D();
			}
		}
		showColors = EditorGUILayout.Foldout (showColors, "Colors");
		if (showColors) {
			ShowColors();
		}
		showWidths = EditorGUILayout.Foldout (showWidths, "Widths");
		if (showWidths) {
			ShowWidths();
		}
		if (vline.name != vobject.gameObject.name) {
			vline.name = vobject.gameObject.name;
			nameString = vline.name + ":";
		}
	}
	
	private void ShowStyle () {
		EditorGUILayout.BeginVertical ("Box");
		lineWidth = EditorGUILayout.FloatField ("Width", lineWidth);
		if (lineWidth < 0.01f) {
			lineWidth = 0.01f;
		}
		if (vline.lineWidth != lineWidth) {
			vline.lineWidth = lineWidth;
			for (int i = 0; i < segments.Count; i++) {
				segments[i].width = lineWidth;
			}
			UpdateLine();
		}
		
		capLength = EditorGUILayout.FloatField ("Cap Length", capLength);
		if (capLength < 0) {
			capLength = 0;
		}
		if (vline.capLength != capLength) {
			vline.capLength = capLength;
			UpdateLine();
		}
		
		color = EditorGUILayout.ColorField ("Color", color);
		if (!Colors32Equal (vline.color, color)) {
			vline.color = color;
			for (int i = 0; i < segments.Count; i++) {
				segments[i].color = color;
			}
			UpdateLine();
		}
		
		lineType = (LineType)EditorGUILayout.EnumPopup ("LineType", lineType);
		if (vline.lineType != lineType) {
			if (lineType == LineType.Discrete && (points.Count & 1) != 0) {
				RemovePoint (points.Count - 1);
			}
			vline.lineType = lineType;
			UpdateLine();
			joins = vline.joins;	// In case of invalid joins
			SetSegmentLists();
		}
		
		joins = (Joins)EditorGUILayout.EnumPopup ("Joins", joins);
		if (vline.joins != joins) {
			vline.joins = joins;
			UpdateLine();
			joins = vline.joins;
		}
		
		collider = EditorGUILayout.Toggle ("Collider", collider);		
		if (vline.collider != collider) {
			vline.collider = collider;
			UpdateLine();
		}
		EditorGUILayout.EndVertical();
	}
	
	private void ShowTexture () {
		EditorGUILayout.BeginVertical ("Box");
		GUILayout.BeginHorizontal();
		texture = (EditorGUILayout.ObjectField ("Texture", texture, typeof(Texture), true) as Texture);
		if (vline.texture != texture) {
			vline.texture = texture;
			UpdateLine();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUI.enabled = (texture != null);
		useTextureScale = EditorGUILayout.Toggle ("Use Texture Scale", useTextureScale);
		if (oldUseTextureScale != useTextureScale) {
			oldUseTextureScale = useTextureScale;
			if (!useTextureScale) {
				storedTextureScale = textureScale;
				textureScale = vline.textureScale = 0.0f;
			}
			else {
				textureScale = storedTextureScale;
				if (textureScale == 0.0f) {
					textureScale = storedTextureScale = vline.textureScale = 1.0f;
				}
			}
			UpdateLine();
		}
		GUI.enabled = useTextureScale;
		if (texture != null && texture.wrapMode == TextureWrapMode.Clamp && useTextureScale) {
			EditorGUILayout.HelpBox ("Texture must have the Clamp Mode set to Repeat in order for texture scale to work", MessageType.Warning);
		}
		textureScale = EditorGUILayout.FloatField ("    Texture Scale", textureScale);
		if (textureScale < 0.0f) {
			textureScale = 0.0f;
		}
		if (vline.textureScale != textureScale) {
			vline.textureScale = textureScale;
			UpdateLine();
		}
		textureOffset = EditorGUILayout.FloatField ("    Texture Offset", textureOffset);
		if (vline.textureOffset != textureOffset) {
			vline.textureOffset = textureOffset;
			UpdateLine();
		}
		GUI.enabled = true;
		EditorGUILayout.EndVertical();
	}
	
	private void ShowPartial () {
		int pointsCount = (vline.is2D? vline.points2.Count : vline.points3.Count);
		EditorGUILayout.BeginVertical ("Box");
		usePartialLine = EditorGUILayout.Toggle ("Use Partial Line", usePartialLine);
		if (oldUsePartialLine != usePartialLine) {
			oldUsePartialLine = usePartialLine;
			if (!usePartialLine) {
				drawStart = vline.drawStart = 0;
				drawEnd = vline.drawEnd = pointsCount-1;
				UpdateLine();
			}
		}
		GUI.enabled = usePartialLine;
		drawStart = EditorGUILayout.FloatField ("    Draw Start", drawStart);
		if (drawStart < 0) {
			drawStart = 0;
		}
		else if (drawStart > pointsCount-1) {
			drawStart = pointsCount-1;
		}
		if (drawStart > drawEnd) {
			drawStart = drawEnd;
		}
		if (vline.drawStart != drawStart) {
			vline.drawStart = (int)Mathf.Round (drawStart);
			UpdateLine();
		}
		drawEnd = EditorGUILayout.FloatField ("    Draw End", drawEnd);
		if (drawEnd < 0) {
			drawEnd = 0;
		}
		else if (drawEnd > pointsCount-1) {
			drawEnd = pointsCount-1;
		}
		if (drawEnd < drawStart) {
			drawEnd = drawStart;
		}
		if (vline.drawEnd != drawEnd) {
			vline.drawEnd = (int)Mathf.Round (drawEnd);
			UpdateLine();
		}
		EditorGUILayout.MinMaxSlider (ref drawStart, ref drawEnd, 0, pointsCount-1);
		GUI.enabled = true;
		EditorGUILayout.EndVertical();
	}
	
	private void ShowPoints () {
		EditorGUILayout.BeginVertical ("Box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label ("Number of points: " + vline.points2.Count);
		GUILayout.FlexibleSpace();
		GUI.enabled = CanAddPoint();
		if (GUILayout.Button ("+", width40)) {
			AddPoint (vline.points2[vline.points2.Count - 1]);
		}
		GUI.enabled = CanRemovePoint();
		GUILayout.Space (15);
		if (GUILayout.Button ("-", width40)) {
			RemovePoint (points.Count - 1);
		}
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		GUILayout.Space (5);
		
		for (int i = 0; i < points.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("    " + i, width60, height19);
			GUILayout.Label ("X");
			points[i].x = EditorGUILayout.FloatField (points[i].x, width60);
			GUILayout.Label (" Y");
			points[i].y = EditorGUILayout.FloatField (points[i].y, width60);
			if (vline.points2[i].x != points[i].x || vline.points2[i].y != points[i].y) {
				vline.points2[i] = new Vector2(points[i].x, points[i].y);
				UpdateLine();
			}
			GUI.enabled = CanAddPoint();
			if (GUILayout.Button ("+", EditorStyles.miniButton, width40)) {
				InsertPoint (i, vline.points2[i]);
			}
			GUI.enabled = CanRemovePoint();
			if (GUILayout.Button ("-", EditorStyles.miniButton, width40)) {
				RemovePoint (i);
			}
			GUI.enabled = true;
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}

	private void ShowPoints3D () {
		EditorGUILayout.BeginVertical ("Box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label ("Number of points: " + vline.points3.Count);
		GUILayout.FlexibleSpace();
		GUI.enabled = CanAddPoint3D();
		if (GUILayout.Button ("+", width40)) {
			AddPoint3D (vline.points3[vline.points3.Count - 1]);
		}
		GUI.enabled = CanRemovePoint3D();
		GUILayout.Space (15);
		if (GUILayout.Button ("-", width40)) {
			RemovePoint3D (points.Count - 1);
		}
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		GUILayout.Space (5);
		
		for (int i = 0; i < points3.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("    " + i, width60, height19);
			GUILayout.Label ("X");
			points3[i].x = EditorGUILayout.FloatField (points3[i].x, width60);
			GUILayout.Label (" Y");
			points3[i].y = EditorGUILayout.FloatField (points3[i].y, width60);
			GUILayout.Label (" Z");
			points3[i].z = EditorGUILayout.FloatField (points3[i].z, width60);
			if (vline.points3[i].x != points3[i].x || vline.points3[i].y != points3[i].y || vline.points3[i].z != points3[i].z) {
				vline.points3[i] = new Vector3(points3[i].x, points3[i].y, points3[i].z);
				UpdateLine();
			}
			GUI.enabled = CanAddPoint3D();
			if (GUILayout.Button ("+", EditorStyles.miniButton, width40)) {
				InsertPoint3D (i, vline.points3[i]);
			}
			GUI.enabled = CanRemovePoint3D();
			if (GUILayout.Button ("-", EditorStyles.miniButton, width40)) {
				RemovePoint3D (i);
			}
			GUI.enabled = true;
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}
	
	private bool CanAddPoint () {
		return (points.Count < 16383 && vline.lineType != LineType.Discrete) || (points.Count < 32765 && vline.lineType == LineType.Discrete);
	}

	private bool CanAddPoint3D () {
		return (points3.Count < 16383 && vline.lineType != LineType.Discrete) || (points3.Count < 32765 && vline.lineType == LineType.Discrete);
	}
	
	private bool CanRemovePoint () {
		return (points.Count > 2);
	}

	private bool CanRemovePoint3D () {
		return (points3.Count > 2);
	}
	
	private void AddPoint (Vector2 v) {
		points.Add (new Point(v.x, v.y));
		vline.points2.Add (v);
		UpdateLine();
		if (lineType != LineType.Discrete || (lineType == LineType.Discrete && (vline.points2.Count & 1) == 0) ) {
			segments.Add (new Segment(color, lineWidth) );
		}
	}

	private void AddPoint3D (Vector3 v) {
		points3.Add (new Point3(v.x, v.y, v.z));
		vline.points3.Add (v);
		UpdateLine();
		if (lineType != LineType.Discrete || (lineType == LineType.Discrete && (vline.points3.Count & 1) == 0) ) {
			segments.Add (new Segment(color, lineWidth) );
		}
	}
	
	private void InsertPoint (int i, Vector2 v) {
		if (lineType == LineType.Discrete && (i & 1) != 0) {
			i--;
		}
		points.Insert (i, new Point(v.x, v.y));
		vline.points2.Insert (i, v);
		if (lineType == LineType.Discrete) {
			points.Insert (i, new Point(v.x, v.y));
			vline.points2.Insert (i, v);
		}
		int idx = GetSegmentIndex (i);
		segments.Insert (idx, new Segment(color, lineWidth) );
		if (!UpdateLineWithSegments()) {
			UpdateLine();
		}
	}

	private void InsertPoint3D (int i, Vector3 v) {
		if (lineType == LineType.Discrete && (i & 1) != 0) {
			i--;
		}
		points3.Insert (i, new Point3(v.x, v.y, v.z));
		vline.points3.Insert (i, v);
		if (lineType == LineType.Discrete) {
			points3.Insert (i, new Point3(v.x, v.y, v.z));
			vline.points3.Insert (i, v);
		}
		int idx = GetSegmentIndex (i);
		segments.Insert (idx, new Segment(color, lineWidth) );
		if (!UpdateLineWithSegments()) {
			UpdateLine();
		}
	}
	
	private void RemovePoint (int i) {
		if (lineType == LineType.Discrete && (i & 1) != 0) {
			i--;
		}
		points.RemoveAt (i);
		vline.points2.RemoveAt (i);
		if (lineType == LineType.Discrete && i < vline.points2.Count) {
			points.RemoveAt (i);
			vline.points2.RemoveAt (i);
		}
		int idx = GetSegmentIndex (i);
		segments.RemoveAt (idx);
		if (!UpdateLineWithSegments()) {
			UpdateLine();
		}
	}

	private void RemovePoint3D (int i) {
		if (lineType == LineType.Discrete && (i & 1) != 0) {
			i--;
		}
		points3.RemoveAt (i);
		vline.points3.RemoveAt (i);
		if (lineType == LineType.Discrete && i < vline.points3.Count) {
			points3.RemoveAt (i);
			vline.points3.RemoveAt (i);
		}
		int idx = GetSegmentIndex (i);
		segments.RemoveAt (idx);
		if (!UpdateLineWithSegments()) {
			UpdateLine();
		}
	}
	
	private int GetSegmentIndex (int i) {
		int idx = (lineType == LineType.Discrete)? i/2 : i;
		if (idx >= segments.Count) {
			idx = segments.Count-1;
		}
		return idx;
	}
	
	private bool UpdateLineWithSegments () {
		var doUpdate = false;
		for (int i = 0; i < segments.Count; i++) {
			if (!Colors32Equal (vline.GetColor (i), segments[i].color)) {
				vline.SetColor (segments[i].color, i);
				doUpdate = true;
			}
			if (vline.GetWidth (i) != segments[i].width) {
				vline.SetWidth (segments[i].width, i);
				doUpdate = true;
			}
		}
		if (doUpdate) {
			UpdateLine();
			return true;
		}
		return false;
	}
	
	private void ShowColors () {
		EditorGUILayout.BeginVertical ("Box");
		smoothColor = EditorGUILayout.Toggle ("Smooth Color", smoothColor);
		if (vline.smoothColor != smoothColor) {
			vline.smoothColor = smoothColor;
			UpdateLine();
		}
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Reset colors")) {
			for (int i = 0; i < segments.Count; i++) {
				segments[i].color = color;
				vline.SetColor (color, i);
			}
			UpdateLine();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label ("Number of segments: " + vline.GetSegmentNumber());
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space (5);
		
		for (int i = 0; i < segments.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("    " + i, width60, height19);
			segments[i].color = EditorGUILayout.ColorField (segments[i].color, width60);
			if (!Colors32Equal (vline.GetColor (i), segments[i].color)) {
				vline.SetColor (segments[i].color, i);
				UpdateLine();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}
	
	private bool Colors32Equal (Color32 c1, Color32 c2) {
		if (c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a) {
			return true;
		}
		return false;
	}
	
	private void ShowWidths () {
		EditorGUILayout.BeginVertical ("Box");
		smoothWidth = EditorGUILayout.Toggle ("Smooth Width", smoothWidth);
		if (vline.smoothWidth != smoothWidth) {
			vline.smoothWidth = smoothWidth;
			UpdateLine();
		}
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Reset widths")) {
			for (int i = 0; i < segments.Count; i++) {
				segments[i].width = lineWidth;
				vline.SetWidth (lineWidth, i);
			}
			UpdateLine();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label ("Number of segments: " + vline.GetSegmentNumber());
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space (5);
		
		for (int i = 0; i < segments.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("    " + i, width60, height19);
			segments[i].width = EditorGUILayout.FloatField (segments[i].width, width60);
			if (vline.GetWidth (i) != segments[i].width) {
				vline.SetWidth (segments[i].width, i);
				UpdateLine();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}
	
	private void SetupStyles () {
		sceneLabel = new GUIStyle();
		sceneLabel.normal.textColor = Color.white;
		infoLabel = new GUIStyle(EditorStyles.miniLabel);
		infoLabel.wordWrap = true;
		wrapLabel = new GUIStyle(EditorStyles.label);
		wrapLabel.wordWrap = true;
	}
	
	private void OnSceneGUI () {
		if (!vline.is2D) return;
		
		if (infoLabel == null) {
			SetupStyles();
		}
		Handles.color = handlesColor;
		var evt = Event.current;
		Vector2 mousePos = HandleUtility.GUIPointToWorldRay (evt.mousePosition).origin;
		if (evt.type == EventType.MouseDown && evt.button == 0 && !controlActive) {
			mouseDown = true;
		}
		else if (evt.type == EventType.MouseUp && evt.button == 0 && !controlActive) {
			mouseDown = false;
		}
		
		Handles.BeginGUI ();
		GUILayout.BeginArea (new Rect((Screen.width/EditorGUIUtility.pixelsPerPoint) - 180, (Screen.height/EditorGUIUtility.pixelsPerPoint) - 150, 170, 102), "", "box");
		GUILayout.Label (nameString, EditorStyles.boldLabel);
		if (showWarning) {
			GUILayout.Label (warnMessage, wrapLabel);
		}
		else {
			GUILayout.Label (info, infoLabel);
			GUILayout.Space (5);
			
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 60;
			EditorGUIUtility.fieldWidth = 35;
			scenePointSize = EditorGUILayout.Slider ("Point size", scenePointSize, .01f, 50.0f);
			if (oldScenePointSize != scenePointSize) {
				oldScenePointSize = scenePointSize = Mathf.Clamp (scenePointSize, .01f, 100.0f);
				EditorPrefs.SetFloat ("VectrosityScenePointSize", scenePointSize);
			}
			GUILayout.Space (60);	// The line point gizmos can't be selected for some reason if the actual slider is visible
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			showPointCoords = EditorGUILayout.Toggle ("", showPointCoords, GUILayout.Width(15));
			if (GUILayout.Button ("Show point coords", "label")) {
				showPointCoords = !showPointCoords;
			}
			if (oldShowPointCoords != showPointCoords) {
				oldShowPointCoords = showPointCoords;
				EditorPrefs.SetBool ("VectrosityShowPointCoords", showPointCoords);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		Handles.EndGUI();
		
		if (showWarning) return;
		
		for (int i = 0; i < points.Count; i++) {
#if OLDUNITY
			Vector2 sliderPoint = Handles.Slider2D (AdjustedPoint (vline.points2[i]), v3forward, v3right, v3up, scenePointSize, Handles.SphereCap, 1.0f);
#else
			Vector2 sliderPoint = Handles.Slider2D (AdjustedPoint (vline.points2[i]), v3forward, v3right, v3up, scenePointSize, Handles.SphereHandleCap, 1.0f);
#endif
			vline.points2[i] = InverseAdjustedPoint (sliderPoint);
			if (points[i].x != vline.points2[i].x || points[i].y != vline.points2[i].y) {
				points[i].x = vline.points2[i].x;
				points[i].y = vline.points2[i].y;
				UpdateLine();
			}
			if (showPointCoords) {
				Handles.Label (AdjustedPoint (vline.points2[i]) + v2right*5, vline.points2[i].ToString(), sceneLabel);
			}
		}
		
		controlActive = false;
		if (evt.shift) {
			AddScenePoint (mousePos);
		}
		else if (evt.control) {
			RemoveScenePoint (mousePos);
		}
		else if (evt.alt) {
			MoveScenePoints (mousePos);
		}
	}
	
	private void AddScenePoint (Vector2 mousePos) {
		if (mouseDown) return;
		controlActive = true;
		
		mousePos.x = (int)mousePos.x;
		mousePos.y = (int)mousePos.y;
		Vector2 adjustedMousePos = InverseAdjustedPoint (mousePos);
		
		var point0Distance = Vector2.Distance (adjustedMousePos, vline.points2[0]);
		int idx = (Vector2.Distance (adjustedMousePos, vline.points2[vline.points2.Count-1]) < point0Distance)? vline.points2.Count-1 : 0;
		if (lineType == LineType.Discrete) {	// Adding points to the beginning of a discrete line isn't user-friendly
			idx = vline.points2.Count-1;
		}
		
		Handles.color = Color.green;
		if (lineType == LineType.Continuous || (lineType == LineType.Discrete && (points.Count & 1) != 0)) {
			Handles.DrawDottedLine (mousePos, AdjustedPoint (vline.points2[idx]), 4);
		}

#if OLDUNITY
		if (Handles.Button (mousePos, Quaternion.identity, scenePointSize, scenePointSize, Handles.SphereCap)) {
#else
		if (Handles.Button (mousePos, Quaternion.identity, scenePointSize, scenePointSize, Handles.SphereHandleCap)) {
#endif
			if (idx == 0) {
				InsertPoint (0, adjustedMousePos);
			}
			else {
				AddPoint (adjustedMousePos);
			}
		}
		// Work-around for not being able to set Handles.selectedColor...draw on top of the button
#if OLDUNITY
		Handles.SphereCap (0, mousePos, Quaternion.identity, scenePointSize);
#else
		Handles.SphereHandleCap (0, mousePos, Quaternion.identity, scenePointSize, EventType.Repaint);
#endif
		Handles.color = handlesColor;
		
		if (showPointCoords) {
			Handles.Label (mousePos + v2right*10, adjustedMousePos.ToString(), sceneLabel);
		}
	}
	
	private void RemoveScenePoint (Vector2 mousePos) {
		if (mouseDown) return;
		controlActive = true;
		
		if (vline.points2.Count <= 2) return;
		
		var adjustedMousePos = InverseAdjustedPoint (mousePos);
		Handles.color = Color.red;
		int idx;
		var distance = ClosestPointDistance (adjustedMousePos, out idx);
#if OLDUNITY
		Handles.SphereCap (0, AdjustedPoint (vline.points2[idx]), Quaternion.identity, scenePointSize);
#else
		Handles.SphereHandleCap (0, AdjustedPoint (vline.points2[idx]), Quaternion.identity, scenePointSize, EventType.Repaint);
#endif
		if (distance > 40.0f) {
			Handles.DrawDottedLine (mousePos, AdjustedPoint (vline.points2[idx]), 10);
		}
		if (lineType == LineType.Discrete) {
			int idx2 = ((idx & 1) != 0)? idx-1 : idx+1;
#if OLDUNITY
			Handles.SphereCap (0, AdjustedPoint (vline.points2[idx2]), Quaternion.identity, scenePointSize);
#else
			Handles.SphereHandleCap (0, AdjustedPoint (vline.points2[idx2]), Quaternion.identity, scenePointSize, EventType.Repaint);
#endif
			Handles.DrawDottedLine (mousePos, AdjustedPoint (vline.points2[idx2]), 10);
		}
#if OLDUNITY
		if (Handles.Button (mousePos, Quaternion.identity, scenePointSize, scenePointSize, Handles.RectangleCap)) {
#else
		if (Handles.Button (mousePos, Quaternion.identity, scenePointSize, scenePointSize, Handles.RectangleHandleCap)) {
#endif
			RemovePoint (idx);
		}
#if OLDUNITY
		Handles.RectangleCap (0, mousePos, Quaternion.identity, scenePointSize);
#else
		Handles.RectangleHandleCap (0, mousePos, Quaternion.identity, scenePointSize, EventType.Repaint);
#endif
		Handles.color = handlesColor;
	}
	
	private void MoveScenePoints (Vector2 mousePos) {
		if (mouseDown) return;
		controlActive = true;
		
		var bounds = new Bounds(vline.points2[0], Vector3.zero);
		for (int i = 1; i < vline.points2.Count; i++) {
			bounds.Encapsulate (vline.points2[i]);
		}
		var offset = mousePos - AdjustedPoint (bounds.center);
		Handles.color = Color.green;
		if (lineType == LineType.Continuous) {
			for (int i = 0; i < vline.points2.Count-1; i++) {
				Handles.DrawDottedLine (AdjustedPoint (vline.points2[i]) + offset, AdjustedPoint (vline.points2[i+1]) + offset, 4);
			}
		}
		else if (lineType == LineType.Discrete) {
			for (int i = 0; i < vline.points2.Count-1; i += 2) {
				Handles.DrawDottedLine (AdjustedPoint (vline.points2[i]) + offset, AdjustedPoint (vline.points2[i+1]) + offset, 4);
			}
		}
		else {
			for (int i = 0; i < vline.points2.Count; i++) {
#if OLDUNITY
				Handles.DotCap (0, AdjustedPoint (vline.points2[i]) + offset, Quaternion.identity, lineWidth);
#else
				Handles.DotHandleCap (0, AdjustedPoint (vline.points2[i]) + offset, Quaternion.identity, lineWidth, EventType.Repaint);
#endif
			}
		}
		Handles.color = handlesColor;
		
#if OLDUNITY
		if (Handles.Button (mousePos, Quaternion.identity, scenePointSize, scenePointSize, Handles.DotCap)) {
#else
		if (Handles.Button (mousePos, Quaternion.identity, scenePointSize, scenePointSize, Handles.DotHandleCap)) {
#endif
			for (int i = 0; i < vline.points2.Count; i++) {
				vline.points2[i] = vline.points2[i] + offset;
			}
		}
	}
	
	private Vector2 AdjustedPoint (Vector2 v) {
		v.x += vline.rectTransform.position.x * (canvasTransform.localScale.x * vline.rectTransform.localScale.x);
		v.y += vline.rectTransform.position.y * (canvasTransform.localScale.y * vline.rectTransform.localScale.y);
		return v;
	}
	
	private Vector2 InverseAdjustedPoint (Vector2 v) {
		v.x -= vline.rectTransform.position.x / (canvasTransform.localScale.x / vline.rectTransform.localScale.x);
		v.y -= vline.rectTransform.position.y / (canvasTransform.localScale.y / vline.rectTransform.localScale.y);
		return v;
	}
	
	private float ClosestPointDistance (Vector2 pos, out int idx) {
		var shortestDistance = Vector2.Distance (pos, vline.points2[0]);
		idx = 0;
		for (int i = 1; i < points.Count; i++) {
			var thisDistance = Vector2.Distance (pos, vline.points2[i]);
			if (thisDistance < shortestDistance) {
				shortestDistance = thisDistance;
				idx = i;
			}
		}
		return shortestDistance;
	}
	
	private void UpdateLine () {
		vline.Draw();
		drawStart = vline.drawStart;
		drawEnd = vline.drawEnd;
		if (!Application.isPlaying) {
			EditorUtility.SetDirty (vobject);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
		}
	}
}