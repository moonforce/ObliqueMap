// Version 5.6
// ©2018 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Vectrosity {

public enum LineType {Continuous, Discrete, Points}
public enum Joins {Fill, Weld, None}
public enum EndCap {Front, Both, Mirror, Back, None}
public enum Visibility {Dynamic, Static, Always, None}
public enum Brightness {Fog, None}
enum CanvasState {None, OnCanvas, OffCanvas}

[System.Serializable]
public partial class VectorLine {
	public static string Version () {
		return "Vectrosity version 5.6";
	}
	
	[SerializeField]
	Vector3[] m_lineVertices;
	public Vector3[] lineVertices {
		get {return m_lineVertices;}
	}
	[SerializeField]
	Vector2[] m_lineUVs;
	public Vector2[] lineUVs {
		get {return m_lineUVs;}
	}
	[SerializeField]
	Color32[] m_lineColors;
	public Color32[] lineColors {
		get {return m_lineColors;}
	}
	[SerializeField]
	List<int> m_lineTriangles;
	public List<int> lineTriangles {
		get {return m_lineTriangles;}
	}
	[SerializeField]
	int m_vertexCount;
	
	[SerializeField]
	GameObject m_go;
	[SerializeField]
	RectTransform m_rectTransform;
	public RectTransform rectTransform {
		get {
			if (m_go != null) {
				return m_rectTransform;
			}
			return null;
		}
	}
	IVectorObject m_vectorObject;
	
	[SerializeField]
	Color32 m_color;
	public Color32 color {
		get {return m_color;}
		set {
			m_color = value;
			SetColor (value);
		}
	}
	[SerializeField]
	CanvasState m_canvasState;
	[SerializeField]
	bool m_is2D;
	public bool is2D {
		get {return m_is2D;}
	}
	[SerializeField]
	List<Vector2> m_points2;
	public List<Vector2> points2 {
		get {
			if (!m_is2D) {
				Debug.LogError ("Line \"" + name + "\" uses points3 rather than points2");
				return null;
			}
			return m_points2;
		}
		set {
			if (value == null) {
				Debug.LogError ("List for Line \"" + name + "\" must not be null");
				return;
			}
			m_points2 = value;
		}
	}
	[SerializeField]
	List<Vector3> m_points3;
	public List<Vector3> points3 {
		get {
			if (m_is2D) {
				Debug.LogError ("Line \"" + name + "\" uses points2 rather than points3");
				return null;
			}
			return m_points3;
		}
		set {
			if (value == null) {
				Debug.LogError ("List for Line \"" + name + "\" must not be null");
				return;
			}
			m_points3 = value;
		}
	}
	[SerializeField]
	int m_pointsCount;
	int pointsCount {
		get {
			return m_is2D? m_points2.Count : m_points3.Count;
		}
	}
	[SerializeField]
	Vector3[] m_screenPoints;
	[SerializeField]
	float[] m_lineWidths;
	[SerializeField]
	float m_lineWidth;
	public float lineWidth {
		get {return m_lineWidth;}
		set {
			m_lineWidth = value;
			float thisWidth = value * .5f;
			for (int i = 0; i < m_lineWidths.Length; i++) {
				m_lineWidths[i] = thisWidth;
			}
			m_maxWeldDistance = (value*2) * (value*2);
		}
	}
	[SerializeField]
	float m_maxWeldDistance;
	public float maxWeldDistance {
		get {return Mathf.Sqrt (m_maxWeldDistance);}
		set {m_maxWeldDistance = value * value;}
	}
	[SerializeField]
	float[] m_distances;
	[SerializeField]
	string m_name;
	public string name {
		get {return m_name;}
		set {
			m_name = value;
			if (m_go != null) {
				m_go.name = value;
			}
			if (m_vectorObject != null) {
				m_vectorObject.SetName (value);
			}
		}
	}
	[SerializeField]
	Material m_material;
	public Material material {
		get {return m_material;}
		set {
			if (m_vectorObject != null) {
				m_vectorObject.SetMaterial (value);
			}
			m_material = value;
		}
	}
	[SerializeField]
	Texture m_originalTexture;
	[SerializeField]
	Texture m_texture;
	public Texture texture {
		get {return m_texture;}
		set {
			if (m_capType != EndCap.None) {
				m_originalTexture = value;
				return;
			}
			if (m_vectorObject != null) {
				m_vectorObject.SetTexture (value);
			}
			m_texture = value;
		}
	}
	public int layer {
		get {
			if (m_go != null) {
				return m_go.layer;
			}
			return 0;
		}
		set {
			if (m_go != null) {
				m_go.layer = Mathf.Clamp (value, 0, 31);
			}
		}
	}
	[SerializeField]
	bool m_active = true;
	public bool active {
		get {return m_active;}
		set {
			m_active = value;
			if (m_vectorObject != null) {
				m_vectorObject.Enable (value);
			}
		}
	}
	[SerializeField]
	LineType m_lineType;
	public LineType lineType {
		get {return m_lineType;}
		set {
			if (value != m_lineType) {
				m_lineType = value;
				if (value == LineType.Points || (value == LineType.Discrete && m_joins == Joins.Fill)) {
					m_joins = Joins.None;
				}
				if (value == LineType.Discrete) {
					drawStart = m_drawStart;
					drawEnd = m_drawEnd;
				}
				if (value != LineType.Continuous && ((m_points2 != null && m_points2.Count > 16383) || (m_points3 != null && m_points3.Count > 16383))) {
					Resize (16383);
				}
				if (collider) {
					var collider2D = m_go.GetComponent<Collider2D>();
					if (collider2D != null) {
						Object.DestroyImmediate (collider2D);
					}
					AddColliderIfNeeded();
				}
				ResetLine();
			}
		}
	}
	[SerializeField]
	float m_capLength;
	public float capLength {
		get {return m_capLength;}
		set {
			if (m_lineType == LineType.Points) {
				Debug.LogError ("LineType.Points can't use capLength");
				return;
			}
			m_capLength = value;
		}
	}
	[SerializeField]
	bool m_smoothWidth = false;
	public bool smoothWidth {
		get {return m_smoothWidth;}
		set {
			m_smoothWidth = (m_lineType == LineType.Points)? false : value;
		}
	}
	[SerializeField]
	bool m_smoothColor = false;
	public bool smoothColor {
		get {return m_smoothColor;}
		set {
			bool oldValue = m_smoothColor;
			m_smoothColor = (m_lineType == LineType.Points)? false : value;
			if (m_smoothColor != oldValue) {
				int segments = GetSegmentNumber();
				for (int i = 0; i < segments; i++) {
					SetColor (GetColor(i), i);
				}
			}
		}
	}
	[SerializeField]
	Joins m_joins;
	public Joins joins {
		get {return m_joins;}
		set {
			if (m_lineType == LineType.Points || (m_lineType == LineType.Discrete && value == Joins.Fill)) return;
			if ((m_joins == Joins.Fill && value != Joins.Fill) || (m_joins != Joins.Fill && value == Joins.Fill)) {
				m_joins = value;
				ClearTriangles();
				SetupTriangles (0);
			}
			m_joins = value;
			if (m_joins == Joins.Weld) {
				if (m_canvasState == CanvasState.OnCanvas) {
					Draw();
				}
				else if (m_canvasState == CanvasState.OffCanvas) {
					Draw3D();
				}
			}
		}
	}
	[SerializeField]
	bool m_isAutoDrawing = false;
	public bool isAutoDrawing {
		get {return m_isAutoDrawing;}	
	}
	[SerializeField]
	int m_drawStart = 0;
	public int drawStart {
		get {return m_drawStart;}
		set {
			if (m_lineType == LineType.Discrete && (value & 1) != 0) {	// No odd numbers for discrete lines
				value++;
			}
			m_drawStart = Mathf.Clamp (value, 0, pointsCount-1);
		}
	}
	[SerializeField]
	int m_drawEnd = 0;
	public int drawEnd {
		get {return m_drawEnd;}
		set {
			if (m_lineType == LineType.Discrete && value != 0 && (value & 1) == 0) {	// No even numbers for discrete lines (except 0)
				value++;
			}
			m_drawEnd = Mathf.Clamp (value, 0, pointsCount-1);
		}
	}
	[SerializeField]
	int m_endPointsUpdate;
	public int endPointsUpdate {
		get {
			if (m_lineType != LineType.Discrete) {
				return m_endPointsUpdate;
			}
			// Actually works with odd numbers for discrete lines (except 0), but makes more intuitive sense to work with even numbers, so we fake it
			return (m_endPointsUpdate == 0)? 0 : m_endPointsUpdate + 1;
		}
		set {
			if (m_lineType == LineType.Discrete) {
				if (value > 1 && (value & 1) == 0) { // No even numbers for discrete lines
					value--;
				}
			}
			m_endPointsUpdate = Mathf.Max (0, value);
		}
	}
	[SerializeField]
	bool m_useNormals = false;
	[SerializeField]
	bool m_useTangents = false;
	[SerializeField]
	bool m_normalsCalculated = false;
	[SerializeField]
	bool m_tangentsCalculated = false;
	
	[SerializeField]
	EndCap m_capType = EndCap.None;
	[SerializeField]
	string m_endCap;
	public string endCap {
		get {return m_endCap;}
		set {
			if (m_lineType == LineType.Points) {
				Debug.LogError ("LineType.Points can't use end caps");
				return;
			}
			if (m_endCap == value) {
				return;
			}
			if (value == null || value == "") {
				RemoveEndCap();
				return;
			}
			if (capDictionary == null || !capDictionary.ContainsKey (value)) {
				Debug.LogError ("End cap \"" + value + "\" is not set up");
				return;
			}
			if (m_capType != EndCap.None) {
				RemoveEndCap();
			}
			m_endCap = value;
			m_capType = capDictionary[value].capType;
			if (m_capType != EndCap.None) {
				SetupEndCap (capDictionary[value].uvHeights);
			}
		}
	}
	[SerializeField]
	bool m_useCapColors = false;
	[SerializeField]
	Color32 m_frontColor;
	[SerializeField]
	Color32 m_backColor;
	[SerializeField]
	int m_frontEndCapIndex = -1;
	[SerializeField]
	int m_backEndCapIndex = -1;
		
	[SerializeField]
	float m_lineUVBottom;
	[SerializeField]
	float m_lineUVTop;
	[SerializeField]
	float m_frontCapUVBottom;
	[SerializeField]
	float m_frontCapUVTop;
	[SerializeField]
	float m_backCapUVBottom;
	[SerializeField]
	float m_backCapUVTop;
	[SerializeField]
	bool m_continuousTexture = false;
	public bool continuousTexture {
		get {return m_continuousTexture;}
		set {
			m_continuousTexture = value;
			if (value == false) {
				ResetTextureScale();
			}
		}
	}
	[SerializeField]
	Transform m_drawTransform;
	public Transform drawTransform {
		get {return m_drawTransform;}
		set {m_drawTransform = value;}
	}
	[SerializeField]
	bool m_viewportDraw;
	public bool useViewportCoords {
		get {return m_viewportDraw;}
		set {
			if (m_is2D) {
				m_viewportDraw = value;
			}
			else {
				Debug.LogError ("Line must use Vector2 points in order to use viewport coords");
			}
		}		
	}
	[SerializeField]
	float m_textureScale;
	[SerializeField]
	bool m_useTextureScale = false;
	[SerializeField]
	public float textureScale {
		get {return m_textureScale;}
		set {
			m_textureScale = value;
			if (m_textureScale == 0.0f) {
				m_useTextureScale = false;
				ResetTextureScale();
			}
			else {
				m_useTextureScale = true;
			}
		}
	}
	[SerializeField]
	float m_textureOffset;
	public float textureOffset {
		get {return m_textureOffset;}
		set {
			m_textureOffset = value;
			SetTextureScale();
		}	
	}
	[SerializeField]
	bool m_useMatrix = false;
	[SerializeField]
	Matrix4x4 m_matrix;
	public Matrix4x4 matrix {
		get {return m_matrix;}
		set {
			m_matrix = value;
			m_useMatrix = (m_matrix != Matrix4x4.identity);
		}
	}
	public int drawDepth {
		get {
			if (m_canvasState == CanvasState.OffCanvas) {
				Debug.LogError ("VectorLine.drawDepth can't be used with lines made with Draw3D");
				return 0;
			}
			return m_go.transform.GetSiblingIndex();
		}
		set {
			if (m_canvasState == CanvasState.OffCanvas) {
				Debug.LogError ("VectorLine.drawDepth can't be used with lines made with Draw3D");
				return;
			}
			m_go.transform.SetSiblingIndex (value);
		}
	}
	[SerializeField]
	bool m_collider = false;
	public bool collider {
		get {return m_collider;}
		set {
			m_collider = value;
			AddColliderIfNeeded();
			m_go.GetComponent<Collider2D>().enabled = value;
		}
	}
	[SerializeField]
	bool m_trigger = false;
	public bool trigger {
		get {return m_trigger;}
		set {
			m_trigger = value;
			if (m_go.GetComponent<Collider2D>() != null) {
				m_go.GetComponent<Collider2D>().isTrigger = value;
			}
		}
	}
	[SerializeField]
	PhysicsMaterial2D m_physicsMaterial;
	public PhysicsMaterial2D physicsMaterial {
		get {return m_physicsMaterial;}
		set {
			AddColliderIfNeeded();
			m_physicsMaterial = value;
			m_go.GetComponent<Collider2D>().sharedMaterial = value;
		}
	}
	[SerializeField]
	bool m_alignOddWidthToPixels = false;
	public bool alignOddWidthToPixels {
		get {return m_alignOddWidthToPixels;}
		set {
			var offset = value? 0.5f : 0.0f;
			m_rectTransform.anchoredPosition = new Vector2(offset, offset);
			m_alignOddWidthToPixels = value;
		}
	}
	
	// Static VectorLine variables
	static Vector3 v3zero = Vector3.zero;	// Faster than using Vector3.zero since that returns a new instance
	static Canvas m_canvas;
	public static Canvas canvas {
		get {
			if (m_canvas == null) {
				SetupVectorCanvas();
			}
			return m_canvas;
		}
	}
	static Transform camTransform;
	static Camera cam3D;
	static Vector3 oldPosition;
	static Vector3 oldRotation;
	public static Vector3 camTransformPosition {
		get {return camTransform.position;}
	}
	public static bool camTransformExists {
		get {return camTransform != null;}
	}
	static bool lineManagerCreated = false; 
	static LineManager m_lineManager;
	public static LineManager lineManager {
		get {
			// This prevents OnDestroy functions that reference VectorManager from creating LineManager again when editor play mode is stopped
			// Checking m_lineManager == null can randomly fail, since the order of objects being Destroyed is undefined
			if (!lineManagerCreated) {
				lineManagerCreated = true;
				var lineManagerGO = new GameObject("LineManager");
				m_lineManager = lineManagerGO.AddComponent<LineManager>();
				m_lineManager.enabled = false;
				MonoBehaviour.DontDestroyOnLoad (m_lineManager);
			}
			return m_lineManager;
		}
	}
	static Dictionary<string, CapInfo> capDictionary;
	
	private void AddColliderIfNeeded () {
		if (m_go.GetComponent<Collider2D>() == null) {
			m_go.AddComponent ((m_lineType == LineType.Continuous)? typeof(EdgeCollider2D) : typeof(PolygonCollider2D));
			m_go.GetComponent<Collider2D>().isTrigger = m_trigger;
			m_go.GetComponent<Collider2D>().sharedMaterial = m_physicsMaterial;
		}
	}
	
	// Vector3 constructors
	public VectorLine (string name, List<Vector3> points, float width) {
		m_points3 = points;
		SetupLine (name, null, width, LineType.Discrete, Joins.None, false);
	}
	public VectorLine (string name, List<Vector3> points, Texture texture, float width) {
		m_points3 = points;
		SetupLine (name, texture, width, LineType.Discrete, Joins.None, false);
	}
	
	public VectorLine (string name, List<Vector3> points, float width, LineType lineType) {
		m_points3 = points;
		SetupLine (name, null, width, lineType, Joins.None, false);
	}
	public VectorLine (string name, List<Vector3> points, Texture texture, float width, LineType lineType) {
		m_points3 = points;
		SetupLine (name, texture, width, lineType, Joins.None, false);
	}
	
	public VectorLine (string name, List<Vector3> points, float width, LineType lineType, Joins joins) {
		m_points3 = points;
		SetupLine (name, null, width, lineType, joins, false);
	}
	public VectorLine (string name, List<Vector3> points, Texture texture, float width, LineType lineType, Joins joins) {
		m_points3 = points;
		SetupLine (name, texture, width, lineType, joins, false);
	}
	
	// Vector2 constructors
	public VectorLine (string name, List<Vector2> points, float width) {
		m_points2 = points;
		SetupLine (name, null, width, LineType.Discrete, Joins.None, true);
	}
	public VectorLine (string name, List<Vector2> points, Texture texture, float width) {
		m_points2 = points;
		SetupLine (name, texture, width, LineType.Discrete, Joins.None, true);
	}
	
	public VectorLine (string name, List<Vector2> points, float width, LineType lineType) {
		m_points2 = points;
		SetupLine (name, null, width, lineType, Joins.None, true);
	}
	public VectorLine (string name, List<Vector2> points, Texture texture, float width, LineType lineType) {
		m_points2 = points;
		SetupLine (name, texture, width, lineType, Joins.None, true);
	}
	
	public VectorLine (string name, List<Vector2> points, float width, LineType lineType, Joins joins) {
		m_points2 = points;
		SetupLine (name, null, width, lineType, joins, true);
	}
	public VectorLine (string name, List<Vector2> points, Texture texture, float width, LineType lineType, Joins joins) {
		m_points2 = points;
		SetupLine (name, texture, width, lineType, joins, true);
	}
	
	protected void SetupLine (string lineName, Texture texture, float width, LineType lineType, Joins joins, bool use2D) {
		m_is2D = use2D;
		m_lineType = lineType;
		if (joins == Joins.Fill && m_lineType != LineType.Continuous) {
			Debug.LogError ("VectorLine: Must use LineType.Continuous if using Joins.Fill for \"" + lineName + "\"");
			return;
		}
		if (joins == Joins.Weld && m_lineType == LineType.Points) {
			Debug.LogError ("VectorLine: LineType.Points can't use Joins.Weld for \"" + lineName + "\"");
			return;
		}
		if ( (m_is2D && m_points2 == null) || (!m_is2D && m_points3 == null) ) {
			Debug.LogError ("VectorLine: the points array is null for \"" + lineName + "\"");
			return;
		}
		
		if (m_is2D) {
			// Initialize using the capacity, if the list was declared with a capacity but has no contents
			m_pointsCount = (m_points2.Capacity > 0 && m_points2.Count == 0)? m_points2.Capacity : m_points2.Count;
			int count = m_pointsCount - m_points2.Count;
			for (int i = 0; i < count; i++) {
				m_points2.Add (Vector2.zero);
			}
		}
		else {
			m_pointsCount = (m_points3.Capacity > 0 && m_points3.Count == 0)? m_points3.Capacity : m_points3.Count;
			int count = m_pointsCount - m_points3.Count;
			for (int i = 0; i < count; i++) {
				m_points3.Add (Vector3.zero);
			}
		}
		name = lineName;
		if (!SetVertexCount()) return;
		
		m_go = new GameObject(name);
		m_canvasState = CanvasState.None;
		layer = LayerMask.NameToLayer ("UI");
		m_rectTransform = m_go.AddComponent<RectTransform>();
		SetupTransform (m_rectTransform);
		m_texture = texture;
		
		m_lineVertices = new Vector3[m_vertexCount];
		m_lineUVs = new Vector2[m_vertexCount];
		m_lineColors = new Color32[m_vertexCount];
		m_lineUVBottom = 0.0f;
		m_lineUVTop = 1.0f;
		SetUVs (0, GetSegmentNumber());
		m_lineTriangles = new List<int>();
		
		color = Color.white;
		m_maxWeldDistance = (width*2) * (width*2);
		m_joins = joins;
		m_lineWidths = new float[1];
		m_lineWidths[0] = width * .5f;
		m_lineWidth = width;
		if (!m_is2D) {
			m_screenPoints = new Vector3[m_vertexCount];
		}
		m_drawStart = 0;
		m_drawEnd = m_pointsCount-1;
		
		SetupTriangles (0);
	}
	
	private void SetupTriangles (int startVert) {
		int triangleCount = 0, end = 0;
		if (pointsCount > 0) {
			if (m_lineType == LineType.Points) {
				triangleCount = pointsCount*6;
				end = pointsCount*4;
			}
			else if (m_lineType == LineType.Continuous) {
				triangleCount = (m_joins == Joins.Fill)? (pointsCount-1)*12 : (pointsCount-1)*6;
				end = (pointsCount-1)*4;
			}
			else {
				triangleCount = pointsCount/2 * 6;
				end = pointsCount*2;
			}
		}
		if (m_capType != EndCap.None) {
			triangleCount += 12;
		}
		
		if (m_lineTriangles.Count > triangleCount) {
			m_lineTriangles.RemoveRange (triangleCount, m_lineTriangles.Count - triangleCount);
			if (m_joins == Joins.Fill) {
				SetLastFillTriangles();	// Calls m_vectorObject.UpdateTris();
				return;
			}
			if (m_vectorObject != null) {
				m_vectorObject.UpdateTris();
			}
			return;
		}
		
		if (m_joins == Joins.Fill) {
			if (startVert >= 4) {	// Do fill for previous segment if this is added to already-existing segments
				int i = m_lineTriangles.Count - 6;
				m_lineTriangles[i  ] = startVert-3; m_lineTriangles[i+1] = startVert; m_lineTriangles[i+2] = startVert+3;
				m_lineTriangles[i+3] = startVert-2; m_lineTriangles[i+4] = startVert; m_lineTriangles[i+5] = startVert+3;				
			}
			for (int i = startVert; i < end; i += 4) {
				m_lineTriangles.Add (i  ); m_lineTriangles.Add (i+1); m_lineTriangles.Add (i+3);	// Segment
				m_lineTriangles.Add (i+1); m_lineTriangles.Add (i+2); m_lineTriangles.Add (i+3);
				
				m_lineTriangles.Add (i+1); m_lineTriangles.Add (i+4); m_lineTriangles.Add (i+7);	// Fill
				m_lineTriangles.Add (i+2); m_lineTriangles.Add (i+4); m_lineTriangles.Add (i+7);
			}
			SetLastFillTriangles();
		}
		else {
			for (int i = startVert; i < end; i += 4) {
				m_lineTriangles.Add (i  ); m_lineTriangles.Add (i+1); m_lineTriangles.Add (i+3);
				m_lineTriangles.Add (i+1); m_lineTriangles.Add (i+2); m_lineTriangles.Add (i+3);
			}
		}
		
		if (m_vectorObject != null) {
			m_vectorObject.UpdateTris();
		}
	}
	
	private void SetLastFillTriangles () {
		if (pointsCount < 2) return;
		
		int i = (pointsCount-1) * 12 + ((m_capType != EndCap.None)? 12 : 0);
		var updateTris = false;
		// If the first point equals the last point (like with a square), reset the fill triangles appropriately
		if ( (m_is2D && m_points2[0] == m_points2[points2.Count-1]) || (!m_is2D && m_points3[0] == m_points3[points3.Count-1]) ) {
			if (m_lineTriangles[i-4] != 3 && m_lineTriangles[i-1] != 3) {
				updateTris = true;
			}
			m_lineTriangles[i-6] = m_vertexCount-3; m_lineTriangles[i-5] = 0; m_lineTriangles[i-4] = 3;
			m_lineTriangles[i-3] = m_vertexCount-2; m_lineTriangles[i-2] = 0; m_lineTriangles[i-1] = 3;
		}
		else {
			if (m_lineTriangles[i-4] == 3 && m_lineTriangles[i-1] == 3) {
				updateTris = true;
			}
			m_lineTriangles[i-6] = 0; m_lineTriangles[i-5] = 0; m_lineTriangles[i-4] = 0;
			m_lineTriangles[i-3] = 0; m_lineTriangles[i-2] = 0; m_lineTriangles[i-1] = 0;
		}
		if (updateTris && m_vectorObject != null) {
			m_vectorObject.UpdateTris();
		}
	}
	
	private void SetupEndCap (float[] uvHeights) {
		int newVertexCount = m_vertexCount + 8;
		if (newVertexCount > 65534) {
			Debug.LogError ("VectorLine: exceeded maximum vertex count of 65534 for \"" + m_name + "\"...use fewer points");
			return;
		}
		
		ResizeMeshArrays (newVertexCount);
		int idx = 0;
		if (m_joins == Joins.Fill) {
			for (int i = newVertexCount-8; i < newVertexCount; i += 4) {
				m_lineTriangles.Insert (  idx, i  ); m_lineTriangles.Insert (1+idx, i+1); m_lineTriangles.Insert (2+idx, i+3);
				m_lineTriangles.Insert (3+idx, i+1); m_lineTriangles.Insert (4+idx, i+2); m_lineTriangles.Insert (5+idx, i+3);
				idx += 6;
			}
		}
		else {
			for (int i = newVertexCount-8; i < newVertexCount; i += 4) {
				m_lineTriangles.Insert (  idx, i  ); m_lineTriangles.Insert (1+idx, i+1); m_lineTriangles.Insert (2+idx, i+3);
				m_lineTriangles.Insert (3+idx, i+1); m_lineTriangles.Insert (4+idx, i+2); m_lineTriangles.Insert (5+idx, i+3);
				idx += 6;
			}
		}
		
		var endColorIndex = (newVertexCount >= 12)? newVertexCount-12 : 0;
		for (int i = newVertexCount-8; i < newVertexCount-4; i++) {
			m_lineColors[i] = m_lineColors[0];
			m_lineColors[i+4] = m_lineColors[endColorIndex];
		}
		
		m_lineUVBottom = uvHeights[0];
		m_lineUVTop = uvHeights[1];
		m_backCapUVBottom = uvHeights[2];
		m_backCapUVTop = uvHeights[3];
		m_frontCapUVBottom = uvHeights[4];
		m_frontCapUVTop = uvHeights[5];
		SetUVs (0, GetSegmentNumber());
		SetEndCapUVs();
		
		if (m_vectorObject != null) {
			m_vectorObject.UpdateTris();
			m_vectorObject.UpdateUVs();
		}
		SetEndCapColors();
		
		m_originalTexture = m_texture;
		m_texture = capDictionary[m_endCap].texture;
		if (m_vectorObject != null) {
			m_vectorObject.SetTexture (m_texture);
		}
	}
	
	private void ResetLine () {
		SetVertexCount();
		m_lineVertices = new Vector3[m_vertexCount];
		m_lineUVs = new Vector2[m_vertexCount];
		m_lineColors = new Color32[m_vertexCount];
		if (!m_is2D) {
			m_screenPoints = new Vector3[m_vertexCount];
		}
		SetUVs (0, GetSegmentNumber());
		SetColor (m_color);
		int max = GetSegmentNumber();
		SetupWidths (max);
		ClearTriangles();
		SetupTriangles (0);
		if (m_vectorObject != null) {
			m_vectorObject.UpdateMeshAttributes();
		}
		if (m_canvasState == CanvasState.OnCanvas) {
			Draw();
		}
		else if (m_canvasState == CanvasState.OffCanvas) {
			Draw3D();
		}
	}
	
	private void SetEndCapUVs () {
		m_lineUVs[m_vertexCount+3] = new Vector2 (0.0f, m_frontCapUVTop);	// Front	// UL
		m_lineUVs[m_vertexCount  ] = new Vector2 (1.0f, m_frontCapUVTop);				// UR
		m_lineUVs[m_vertexCount+1] = new Vector2 (1.0f, m_frontCapUVBottom);			// LR
		m_lineUVs[m_vertexCount+2] = new Vector2 (0.0f, m_frontCapUVBottom);			// LL
		if (capDictionary[m_endCap].capType == EndCap.Mirror) {
			m_lineUVs[m_vertexCount+7] = new Vector2 (0.0f, m_frontCapUVBottom);	// Front mirrored
			m_lineUVs[m_vertexCount+4] = new Vector2 (1.0f, m_frontCapUVBottom);
			m_lineUVs[m_vertexCount+5] = new Vector2 (1.0f, m_frontCapUVTop);
			m_lineUVs[m_vertexCount+6] = new Vector2 (0.0f, m_frontCapUVTop);
		}
		else {
			m_lineUVs[m_vertexCount+7] = new Vector2 (0.0f, m_backCapUVTop);	// Back
			m_lineUVs[m_vertexCount+4] = new Vector2 (1.0f, m_backCapUVTop);
			m_lineUVs[m_vertexCount+5] = new Vector2 (1.0f, m_backCapUVBottom);
			m_lineUVs[m_vertexCount+6] = new Vector2 (0.0f, m_backCapUVBottom);
		}
	}
		
	private void RemoveEndCap () {
		if (m_capType == EndCap.None) return;
		
		m_endCap = null;
		m_capType = EndCap.None;
		ResizeMeshArrays (m_vertexCount);
		m_lineTriangles.RemoveRange (0, 12);
		m_lineUVBottom = 0.0f;
		m_lineUVTop = 1.0f;
		SetUVs (0, GetSegmentNumber());
		if (m_useTextureScale) {
			SetTextureScale();
		}
		texture = m_originalTexture;
		m_vectorObject.UpdateMeshAttributes();
		if (m_collider) {
			SetCollider (m_canvasState == CanvasState.OnCanvas);
		}
	}
	
	private static void SetupTransform (RectTransform rectTransform) {
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.zero;
		rectTransform.pivot = Vector2.zero;
		rectTransform.anchoredPosition = Vector2.zero;
	}
	
	private void ResizeMeshArrays (int newCount) {
		System.Array.Resize (ref m_lineVertices, newCount);
		System.Array.Resize (ref m_lineUVs, newCount);
		System.Array.Resize (ref m_lineColors, newCount);
	}

	public void Resize (int newCount) {
		if (newCount < 0) {
			Debug.LogError ("VectorLine.Resize: the new count must be >= 0");
			return;
		}
		if (newCount == pointsCount) return;
		
		if (m_is2D) {
			if (newCount > m_pointsCount) {
				for (int i = 0; i < newCount - m_pointsCount; i++) {
					m_points2.Add (Vector2.zero);
				}
			}
			else {
				m_points2.RemoveRange (newCount, m_pointsCount - newCount);
			}
		}
		else {
			if (newCount > m_pointsCount) {
				for (int i = 0; i < newCount - m_pointsCount; i++) {
					m_points3.Add (v3zero);
				}
			}
			else {
				m_points3.RemoveRange (newCount, m_pointsCount - newCount);
			}
		}
		Resize();
	}
	
	private void Resize () {
		int oldCount = m_pointsCount;
		int originalSegmentCount = m_pointsCount;
		if (m_lineType != LineType.Points) {
			originalSegmentCount = (m_lineType == LineType.Continuous)? Mathf.Max (0, m_pointsCount-1) : m_pointsCount/2;
		}
		bool adjustDrawEnd = (m_drawEnd == m_pointsCount-1 || m_drawEnd < 1);
		if (!SetVertexCount()) return;
		
		m_pointsCount = pointsCount;
		int baseArrayLength = m_lineVertices.Length - ((m_capType == EndCap.None)? 0 : 8);
		if (baseArrayLength < m_vertexCount) {
			if (baseArrayLength == 0) {
				baseArrayLength = 4;
			}
			while (baseArrayLength < m_pointsCount) {
				baseArrayLength *= 2;
			}
			baseArrayLength = Mathf.Min (baseArrayLength, MaxPoints());
			ResizeMeshArrays ((m_capType == EndCap.None)? baseArrayLength*4 : baseArrayLength*4 + 8);
			if (!m_is2D) {
				System.Array.Resize (ref m_screenPoints, baseArrayLength*4);
			}
		}
		if (m_lineWidths.Length > 1) {
			if (m_lineType != LineType.Points) {
				baseArrayLength = (m_lineType == LineType.Continuous)? baseArrayLength-1 : baseArrayLength/2;
			}
			if (baseArrayLength > m_lineWidths.Length) {
				ResizeLineWidths (baseArrayLength);
			}
		}
		
		if (adjustDrawEnd) {
			m_drawEnd = m_pointsCount-1;
		}
		m_drawStart = Mathf.Clamp (m_drawStart, 0, m_pointsCount-1);
		m_drawEnd = Mathf.Clamp (m_drawEnd, 0, m_pointsCount-1);
		if (m_pointsCount > originalSegmentCount) {
			SetColor (m_color, originalSegmentCount, GetSegmentNumber());
			SetUVs (originalSegmentCount, GetSegmentNumber());
		}
		if (m_pointsCount < oldCount) {
			ZeroVertices (m_pointsCount, oldCount);
		}
		
		if (m_capType != EndCap.None) {
			SetEndCapUVs();
			SetEndCapColors();
		}
		SetupTriangles (originalSegmentCount*4);
		if (m_vectorObject != null) {
			m_vectorObject.UpdateMeshAttributes();
		}
	}
	
	void ResizeLineWidths (int newSize) {
		if (newSize > m_lineWidths.Length) {
			var newWidths = new float[newSize];
			for (int i = 0; i < m_lineWidths.Length; i++) {
				newWidths[i] = m_lineWidths[i];
			}
			for (int i = m_lineWidths.Length; i < newSize; i++) {
				newWidths[i] = m_lineWidth * .5f;
			}
			m_lineWidths = newWidths;
		}	
	}
	
	private void SetUVs (int startIndex, int endIndex) {
		var uv1 = new Vector2(0.0f, m_lineUVTop);
		var uv2 = new Vector2(1.0f, m_lineUVTop);
		var uv3 = new Vector2(1.0f, m_lineUVBottom);
		var uv4 = new Vector2(0.0f, m_lineUVBottom);
		int idx = startIndex * 4;
		for (int i = startIndex; i < endIndex; i++) {
			m_lineUVs[idx  ] = uv1;
			m_lineUVs[idx+1] = uv2;
			m_lineUVs[idx+2] = uv3;
			m_lineUVs[idx+3] = uv4;
			idx += 4;
		}
		if (m_vectorObject != null) {
			m_vectorObject.UpdateUVs();
		}
	}
	
	private bool SetVertexCount () {
		m_vertexCount = Mathf.Max (0, GetSegmentNumber() * 4);
		if (m_lineType == LineType.Discrete && (pointsCount & 1) != 0) {
			m_vertexCount += 4;
		}
		int max = 65534;
		if (m_capType != EndCap.None) {
			max -= 8;
		}
		if (m_vertexCount > max) {
			Debug.LogError ("VectorLine: exceeded maximum vertex count of 65534 for \"" + name + "\"...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines, minus two if end caps are used)");
			return false;
		}
		return true;
	}
	
	private int MaxPoints () {
		if (m_capType != EndCap.None) {
			return 16381;
		}
		return 16383;
	}
	
	public void AddNormals () {
		m_useNormals = true;
		m_normalsCalculated = false;
	}
	
	public void AddTangents () {
		if (!m_useNormals) {
			m_useNormals = true;
			m_normalsCalculated = false;
		}
		m_useTangents = true;
		m_tangentsCalculated = false;
	}
	
	public Vector4[] CalculateTangents (Vector3[] normals) {
		if (!m_useNormals) {
			m_vectorObject.UpdateNormals();
			m_useNormals = true;
			m_normalsCalculated = true;
		}
		
		int vertCount = m_vectorObject.VertexCount();
		var tan1 = new Vector3[vertCount];
		var tan2 = new Vector3[vertCount];
		int triCount = m_lineTriangles.Count;
		for (int i = 0; i < triCount; i += 3) {
			int i1 = m_lineTriangles[i];
			int i2 = m_lineTriangles[i+1];
			int i3 = m_lineTriangles[i+2];
			
			Vector3 v1 = m_lineVertices[i1];
			Vector3 v2 = m_lineVertices[i2];
			Vector3 v3 = m_lineVertices[i3];
			
			Vector2 w1 = m_lineUVs[i1];
			Vector2 w2 = m_lineUVs[i2];
			Vector2 w3 = m_lineUVs[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float r = 1.0f / (s1 * t2 - s2 * t1);
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		var tangents = new Vector4[vertCount];
		for (int i = 0; i < m_vertexCount; i++) {
			Vector3 n = normals[i];
			Vector3 t = tan1[i];
			tangents[i] = (t - n * Vector3.Dot(n, t)).normalized;
			tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
		}
		
		return tangents;
	}
	
	public static GameObject SetupVectorCanvas () {
		GameObject go = GameObject.Find ("VectorCanvas");
		Canvas canvas;
		if (go != null) {
			canvas = go.GetComponent<Canvas>();
		}
		else {
			go = new GameObject ("VectorCanvas");
			go.layer = LayerMask.NameToLayer ("UI");
			canvas = go.AddComponent<Canvas>();
		}
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 1;
		m_canvas = canvas;
		return go;
	}
	
	public static void SetCanvasCamera (Camera cam) {
		if (m_canvas == null) {
			SetupVectorCanvas();
		}
		m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
		m_canvas.worldCamera = cam;
	}
	
	public void SetCanvas (GameObject canvasObject) {
		SetCanvas (canvasObject, true);
	}
	
	public void SetCanvas (GameObject canvasObject, bool worldPositionStays) {
		var canvas = canvasObject.GetComponent<Canvas>();
		if (canvas == null) {
			Debug.LogError ("VectorLine.SetCanvas: canvas object must have a Canvas component");
			return;
		}
		SetCanvas (canvas, worldPositionStays);
	}

	public void SetCanvas (Canvas canvas) {
		SetCanvas (canvas, true);
	}
	
	public void SetCanvas (Canvas canvas, bool worldPositionStays) {
		if (m_canvasState == CanvasState.OffCanvas) {
			Debug.LogError ("VectorLine.SetCanvas only works with lines made with Draw, not Draw3D.");
			return;
		}
		if (canvas == null) {
			Debug.LogError ("VectorLine.SetCanvas: canvas must not be null");
			return;
		}
		if (canvas.renderMode == RenderMode.WorldSpace) {
			Debug.LogError ("VectorLine.SetCanvas: canvas must be screen space overlay or screen space camera");
			return;
		}
		m_go.transform.SetParent (canvas.transform, worldPositionStays);
	}

	public void SetMask (GameObject maskObject) {
		SetMask (maskObject, true);
	}
	
	public void SetMask (GameObject maskObject, bool worldPositionStays) {
		var mask = maskObject.GetComponent<Mask>();
		if (mask == null) {
			Debug.LogError ("VectorLine.SetMask: mask object must have a Mask component");
			return;
		}
		SetMask (mask, worldPositionStays);
	}

	public void SetMask (Mask mask) {
		SetMask (mask, true);
	}
	
	public void SetMask (Mask mask, bool worldPositionStays) {
		if (m_canvasState == CanvasState.OffCanvas) {
			Debug.LogError ("VectorLine.SetMask only works with lines made with Draw, not Draw3D.");
			return;
		}
		if (mask == null) {
			Debug.LogError ("VectorLine.SetMask: mask must not be null");
			return;
		}
		m_go.transform.SetParent (mask.transform, worldPositionStays);
	}
	
	private bool CheckCamera3D () {
		if (!m_is2D && !cam3D) {
			SetCamera3D();
			if (!cam3D) {
				Debug.LogError ("No camera available...use VectorLine.SetCamera3D to assign a camera");
				return false;
			}
		}
		return true;
	}
	
	public static void SetCamera3D () {
		if (Camera.main == null) {
			Debug.LogError ("VectorLine.SetCamera3D: no camera tagged \"Main Camera\" found. Please call SetCamera3D with a specific camera instead.");
			return;
		}
		SetCamera3D (Camera.main);
	}

	public static void SetCamera3D (GameObject cameraObject) {
		var camera = cameraObject.GetComponent<Camera>();
		if (camera == null) {
			Debug.LogError ("VectorLine.SetCamera3D: camera object must have a Camera component");
			return;
		}
		SetCamera3D (camera);
	}
	
	public static void SetCamera3D (Camera camera) {
		camTransform = camera.transform;
		cam3D = camera;
		oldPosition = camTransform.position + Vector3.one;
		oldRotation = camTransform.eulerAngles + Vector3.one;
	}
	
	public static bool CameraHasMoved () {
		return oldPosition != camTransform.position || oldRotation != camTransform.eulerAngles;
	}
	
	public static void UpdateCameraInfo () {
		oldPosition = camTransform.position;
		oldRotation = camTransform.eulerAngles;	
	}
	
	public int GetSegmentNumber () {
		if (m_lineType == LineType.Points) {
			return pointsCount;	
		}
		if (m_lineType == LineType.Continuous) {
			return (pointsCount == 0)? 0 : pointsCount-1;
		}
		return pointsCount/2;
	}
	
	private void SetEndCapColors () {
		if (m_lineVertices.Length < 4) return;
		
		if (m_capType <= EndCap.Mirror) {	// Front
			int vIndex = (m_lineType == LineType.Continuous)? m_drawStart * 4 : m_drawStart * 2;
			for (int i = 0; i < 4; i++) {
				m_lineColors[i + m_vertexCount] = m_useCapColors? m_frontColor : m_lineColors[i + vIndex];
			}
		}
		if (m_capType >= EndCap.Both) {	// Back
			int end = m_drawEnd;
			if (m_lineType == LineType.Continuous) {
				if (m_drawEnd == pointsCount) end--;
			}
			else {
				if (end < pointsCount) end++;
			}
			int vIndex = end * (m_lineType == LineType.Continuous? 4 : 2) - 2;
			if (vIndex < 0) {
				vIndex = 0;
			}
			for (int i = 4; i < 8; i++) {
				m_lineColors[i + m_vertexCount] = m_useCapColors? m_backColor : m_lineColors[vIndex];
			}
		}
		if (m_vectorObject != null) {
			m_vectorObject.UpdateColors();
		}
	}
	
	public void SetEndCapColor (Color32 color) {
		SetEndCapColor (color, color);
	}
	
	public void SetEndCapColor (Color32 frontColor, Color32 backColor) {
		if (m_capType == EndCap.None) {
			Debug.LogError ("VectorLine.SetEndCapColor: the line \"" + name + "\" does not have any end caps");
			return;
		}
		m_useCapColors = true;
		m_frontColor = frontColor;
		m_backColor = backColor;
		SetEndCapColors();
	}
	
	public void SetEndCapIndex (EndCap endCap, int index) {
		if (m_capType == EndCap.None) {
			Debug.LogError ("VectorLine.SetEndCapIndex: the line \"" + name + "\" does not have any end caps");
			return;
		}
		if (endCap != EndCap.Front && endCap != EndCap.Back) {
			Debug.LogError ("VectorLine.SetEndCapIndex: endCap must be EndCap.Front or EndCap.Back");
			return;
		}
		if (index < 0) {
			index = 0;
		}
		if (endCap == EndCap.Front) {
			m_frontEndCapIndex = index;
		}
		else if (endCap == EndCap.Back) {
			m_backEndCapIndex = index;
		}
	}
	
	public void SetColor (Color32 color) {
		SetColor (color, 0, pointsCount);
	}
	
	public void SetColor (Color32 color, int index) {
		SetColor (color, index, index);
	}
	
	public void SetColor (Color32 color, int startIndex, int endIndex) {
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		int max = GetSegmentNumber();
		startIndex = Mathf.Clamp (startIndex*4, 0, max*4);
		endIndex = Mathf.Clamp ((endIndex + 1)*4, 0, max*4);
		
		if (!m_smoothColor) {
			for (int i = startIndex; i < endIndex; i++) {
				m_lineColors[i] = color;
			}
		}
		else {
			if (startIndex == 0) {
				m_lineColors[0] = color;
				m_lineColors[3] = color;
			}
			for (int i = startIndex; i < endIndex; i += 4) {
				m_lineColors[i+1] = color;
				m_lineColors[i+2] = color;
				if (i+4 < m_vertexCount) {
					m_lineColors[i+4] = color;
					m_lineColors[i+7] = color;
				}
			}
		}
		
		if (m_capType != EndCap.None && (startIndex <= 0 || endIndex >= max-1)) {
			SetEndCapColors();
		}
		if (m_vectorObject != null) {
			m_vectorObject.UpdateColors();
		}
	}

	public void SetColors (List<Color32> lineColors) {
		if (lineColors == null) {
			Debug.LogError ("VectorLine.SetColors: lineColors list must not be null");
			return;
		}
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		if (m_lineType != LineType.Points) {
			if (WrongArrayLength (lineColors.Count, FunctionName.SetColors)) {
				return;
			}
		}
		else if (lineColors.Count != pointsCount) {
			Debug.LogError ("VectorLine.SetColors: Length of lineColors list in \"" + name + "\" must be same length as points list");
			return;
		}
		
		int start, end;
		SetSegmentStartEnd (out start, out end);
		if (start == 0 && end == 0) return;
		
		int idx = start*4;
		if (m_lineType == LineType.Points) {
			end++;
		}
		
		if (smoothColor) {
			m_lineColors[idx  ] = lineColors[start];
			m_lineColors[idx+3] = lineColors[start];
			m_lineColors[idx+2] = lineColors[start];
			m_lineColors[idx+1] = lineColors[start];
			idx += 4;
			for (int i = start+1; i < end; i++) {
				m_lineColors[idx  ] = lineColors[i-1];
				m_lineColors[idx+3] = lineColors[i-1];
				m_lineColors[idx+2] = lineColors[i];
				m_lineColors[idx+1] = lineColors[i];
				idx += 4;
			}
		}
		else {	// Not smooth Color
			for (int i = start; i < end; i++) {
				m_lineColors[idx  ] = lineColors[i];
				m_lineColors[idx+1] = lineColors[i];
				m_lineColors[idx+2] = lineColors[i];
				m_lineColors[idx+3] = lineColors[i];
				idx += 4;
			}
		}

		if (m_capType != EndCap.None) {
			SetEndCapColors();
		}
		if (m_vectorObject != null) {
			m_vectorObject.UpdateColors();
		}
	}
	
	private void SetSegmentStartEnd (out int start, out int end) {
		start = (m_lineType != LineType.Discrete)? m_drawStart : m_drawStart/2;
		end = m_drawEnd;
		if (m_lineType == LineType.Discrete) {
			end = m_drawEnd/2;
			if (m_drawEnd%2 != 0) {
				end++;
			}
		}
	}
	
	public Color32 GetColor (int index) {
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		if (m_vertexCount == 0) {
			return m_color;
		}
		int i = index*4 + 2;
		if (i < 0 || i >= m_vertexCount) {
			Debug.LogError ("VectorLine.GetColor: index " + index + " out of range");
			return Color.clear;
		}		
		return m_lineColors[i];		
	}
	
	private void SetupWidths (int max) {
		if ((max >= 2 && m_lineWidths.Length == 1) || (max >= 2 && m_lineWidths.Length != max)) {
			ResizeLineWidths (max);
		}
	}
	
	public void SetWidth (float width) {
		m_lineWidth = width;
		SetWidth (width, 0, pointsCount);
	}
	
	public void SetWidth (float width, int index) {
		SetWidth (width, index, index);
	}
	
	public void SetWidth (float width, int startIndex, int endIndex) {
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		int max = GetSegmentNumber();
		SetupWidths (max);
		startIndex = Mathf.Clamp (startIndex, 0, Mathf.Max (max-1, 0));
		endIndex = Mathf.Clamp (endIndex, 0, Mathf.Max (max-1, 0));
		for (int i = startIndex; i <= endIndex; i++) {
			m_lineWidths[i] = width * .5f;
		}
	}

	public void SetWidths (List<float> lineWidths) {
		SetWidths (lineWidths, null, lineWidths.Count, true);
	}
	
	public void SetWidths (List<int> lineWidths) {
		SetWidths (null, lineWidths, lineWidths.Count, false);
	}
	
	private void SetWidths (List<float> lineWidthsFloat, List<int> lineWidthsInt, int arrayLength, bool doFloat) {
		if ((doFloat && lineWidthsFloat == null) || (!doFloat && lineWidthsInt == null)) {
			Debug.LogError ("VectorLine.SetWidths: line widths list must not be null");
			return;
		}
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		if (m_lineType == LineType.Points) {
			if (arrayLength != pointsCount) {
				Debug.LogError ("VectorLine.SetWidths: line widths list must be the same length as the points list for \"" + name + "\"");
				return;
			}
		}
		else if (WrongArrayLength (arrayLength, FunctionName.SetWidths)) {
			return;
		}
		
		if (m_lineWidths.Length != arrayLength) {
			System.Array.Resize (ref m_lineWidths, arrayLength);
		}
		
		if (doFloat) {
			for (int i = 0; i < arrayLength; i++) {
				m_lineWidths[i] = lineWidthsFloat[i] * .5f;
			}
		}
		else {
			for (int i = 0; i < arrayLength; i++) {
				m_lineWidths[i] = (float)lineWidthsInt[i] * .5f;
			}
		}
	}
	
	public float GetWidth (int index) {
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		int max = GetSegmentNumber();
		if (index < 0 || index >= max) {
			Debug.LogError ("VectorLine.GetWidth: index " + index + " out of range...must be >= 0 and < " + max);
			return 0;
		}
		if (index >= m_lineWidths.Length) {
			return m_lineWidth;
		}
		return m_lineWidths[index] * 2;
	}
	
	public static VectorLine SetLine (Color color, params Vector2[] points) {
		return SetLine (color, 0.0f, points);
	}

	public static VectorLine SetLine (Color color, float time, params Vector2[] points) {
		if (points.Length < 2) {
			Debug.LogError ("VectorLine.SetLine needs at least two points");
			return null;
		}
		var line = new VectorLine("Line", new List<Vector2>(points), null, 1.0f, LineType.Continuous, Joins.None);
		line.color = color;
		if (time > 0.0f) {
			lineManager.DisableLine (line, time);
		}
		line.Draw();
		return line;
	}

	public static VectorLine SetLine (Color color, params Vector3[] points) {
		return SetLine (color, 0.0f, points);
	}
	
	public static VectorLine SetLine (Color color, float time, params Vector3[] points) {
		if (points.Length < 2) {
			Debug.LogError ("VectorLine.SetLine needs at least two points");
			return null;
		}
		var line = new VectorLine("SetLine", new List<Vector3>(points), null, 1.0f, LineType.Continuous, Joins.None);
		line.color = color;
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		line.Draw();
		return line;
	}

	public static VectorLine SetLine3D (Color color, params Vector3[] points) {
		return SetLine3D (color, 0.0f, points);
	}
		
	public static VectorLine SetLine3D (Color color, float time, params Vector3[] points) {
		if (points.Length < 2) {
			Debug.LogError ("VectorLine.SetLine3D needs at least two points");
			return null;
		}
		var line = new VectorLine("SetLine3D", new List<Vector3>(points), null, 1.0f, LineType.Continuous, Joins.None);
		line.color = color;
		line.Draw3DAuto (time);
		return line;
	}

	public static VectorLine SetRay (Color color, Vector3 origin, Vector3 direction) {
		return SetRay (color, 0.0f, origin, direction);
	}

	public static VectorLine SetRay (Color color, float time, Vector3 origin, Vector3 direction) {
		var line = new VectorLine("SetRay", new List<Vector3>(new Vector3[] {origin, new Ray(origin, direction).GetPoint (direction.magnitude)}), null, 1.0f, LineType.Continuous, Joins.None);
		line.color = color;
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		line.Draw();
		return line;
	}

	public static VectorLine SetRay3D (Color color, Vector3 origin, Vector3 direction) {
		return SetRay3D (color, 0.0f, origin, direction);
	}

	public static VectorLine SetRay3D (Color color, float time, Vector3 origin, Vector3 direction) {
		var line = new VectorLine("SetRay3D", new List<Vector3>(new Vector3[] {origin, new Ray(origin, direction).GetPoint (direction.magnitude)}), null, 1.0f, LineType.Continuous, Joins.None);
		line.color = color;
		line.Draw3DAuto (time);
		return line;
	}
	
	private void CheckNormals () {
		if (m_useNormals && !m_normalsCalculated) {
			m_vectorObject.UpdateNormals();
			m_normalsCalculated = true;
		}
		if (m_useTangents && !m_tangentsCalculated) {
			m_vectorObject.UpdateTangents();
			m_tangentsCalculated = true;
		}
	}
	
	private void CheckLine (bool draw3D) {
		if (m_capType != EndCap.None) {
			DrawEndCap (draw3D);
		}
		if (m_continuousTexture) {
			SetContinuousTexture();
		}
		if (m_joins == Joins.Fill) {
			SetLastFillTriangles();
		}
	}
	
	private void DrawEndCap (bool draw3D) {
		int vIndex;
		if (m_capType <= EndCap.Mirror) {	// Draw front
			if (m_frontEndCapIndex != -1) {
				vIndex = m_frontEndCapIndex;
				if (m_lineType == LineType.Discrete && (vIndex & 1) != 0) {	// No odd numbers for discrete lines
					vIndex++;
				}
				vIndex = Mathf.Clamp (vIndex, drawStart, drawEnd) * 4;
			}
			else {
				vIndex = m_drawStart * 4;
			}
			int widthIndex = (m_lineWidths.Length > 1)? m_drawStart : 0;
			if (m_lineType == LineType.Discrete) {
				widthIndex /= 2;
				vIndex /= 2;
			}
			if (!draw3D) {
				var d = (m_lineVertices[vIndex] - m_lineVertices[vIndex+1]).normalized * m_lineWidths[widthIndex] * 2.0f * capDictionary[m_endCap].ratio1;
				var d2 = d * capDictionary[m_endCap].offset1;
				
				m_lineVertices[m_vertexCount  ] = m_lineVertices[vIndex  ] + d + d2;	// Set end cap vertex positions, including offset
				m_lineVertices[m_vertexCount+3] = m_lineVertices[vIndex+3] + d + d2;
				m_lineVertices[vIndex  ] += d2;	// Move line vertices based on offset
				m_lineVertices[vIndex+3] += d2;
			}
			else {
				var d = (m_screenPoints[vIndex] - m_screenPoints[vIndex+1]).normalized * m_lineWidths[widthIndex] * 2.0f * capDictionary[m_endCap].ratio1;
				var d2 = d * capDictionary[m_endCap].offset1;
				
				m_lineVertices[m_vertexCount  ] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex  ] + d + d2);
				m_lineVertices[m_vertexCount+3] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex+3] + d + d2);
				m_lineVertices[vIndex  ] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex  ] + d2);
				m_lineVertices[vIndex+3] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex+3] + d2);
			}
			m_lineVertices[m_vertexCount+2] = m_lineVertices[vIndex+3];
			m_lineVertices[m_vertexCount+1] = m_lineVertices[vIndex  ];
			
			if (capDictionary[m_endCap].scale1 != 1.0f) {
				ScaleCapVertices (m_vertexCount, capDictionary[m_endCap].scale1, (m_lineVertices[m_vertexCount+1] + m_lineVertices[m_vertexCount+2]) / 2);
			}
			
			m_lineTriangles[0] = m_vertexCount  ; m_lineTriangles[1] = m_vertexCount+1; m_lineTriangles[2] = m_vertexCount+3;
			m_lineTriangles[3] = m_vertexCount+1; m_lineTriangles[4] = m_vertexCount+2; m_lineTriangles[5] = m_vertexCount+3;
		}
		
		if (m_capType >= EndCap.Both) {	// Draw back
			int end = m_drawEnd;
			if (m_lineType == LineType.Continuous) {
				if (m_drawEnd == pointsCount) end--;
			}
			else {
				if (end < pointsCount) end++;
			}
			if (m_backEndCapIndex != -1) {
				vIndex = m_backEndCapIndex;
				if (m_lineType == LineType.Discrete && (vIndex & 1) != 0) {
					vIndex++;
				}
				vIndex = Mathf.Clamp (vIndex, drawStart, end) * 4;
			}
			else {
				vIndex = end * 4;
			}
			int widthIndex = (m_lineWidths.Length > 1)? end-1 : 0;
			if (widthIndex < 0) {
				widthIndex = 0;
			}
			if (m_lineType == LineType.Discrete) {
				widthIndex /= 2;
				vIndex /= 2;
			}
			if (vIndex < 4) {
				vIndex = 4;
			}
			if (!draw3D) {
				var d = (m_lineVertices[vIndex-2] - m_lineVertices[vIndex-1]).normalized * m_lineWidths[widthIndex] * 2.0f * capDictionary[m_endCap].ratio2;
				var d2 = d * capDictionary[m_endCap].offset2;
				
				m_lineVertices[m_vertexCount+6] = m_lineVertices[vIndex-2] + d + d2;
				m_lineVertices[m_vertexCount+5] = m_lineVertices[vIndex-3] + d + d2;
				m_lineVertices[vIndex-3] += d2;
				m_lineVertices[vIndex-2] += d2;
			}
			else {
				var d = (m_screenPoints[vIndex-2] - m_screenPoints[vIndex-1]).normalized * m_lineWidths[widthIndex] * 2.0f * capDictionary[m_endCap].ratio2;
				var d2 = d * capDictionary[m_endCap].offset2;
				
				m_lineVertices[m_vertexCount+6] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex-2] + d + d2);
				m_lineVertices[m_vertexCount+5] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex-3] + d + d2);
				m_lineVertices[vIndex-3] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex-3] + d2);
				m_lineVertices[vIndex-2] = cam3D.ScreenToWorldPoint (m_screenPoints[vIndex-2] + d2);
			}
			m_lineVertices[m_vertexCount+4] = m_lineVertices[vIndex-3];
			m_lineVertices[m_vertexCount+7] = m_lineVertices[vIndex-2];
			
			if (capDictionary[m_endCap].scale2 != 1.0f) {
				ScaleCapVertices (m_vertexCount+4, capDictionary[m_endCap].scale2, (m_lineVertices[m_vertexCount+4] + m_lineVertices[m_vertexCount+7]) / 2);
			}
			
			m_lineTriangles[6] = m_vertexCount+4; m_lineTriangles[7 ] = m_vertexCount+5; m_lineTriangles[8 ] = m_vertexCount+7;
			m_lineTriangles[9] = m_vertexCount+5; m_lineTriangles[10] = m_vertexCount+6; m_lineTriangles[11] = m_vertexCount+7;
		}
		
		if (m_drawStart > 0 || m_drawEnd < pointsCount) {
			SetEndCapColors();
		}
	}
	
	private void ScaleCapVertices (int offset, float scale, Vector3 center) {
		m_lineVertices[offset  ] = (m_lineVertices[offset  ] - center) * scale + center;
		m_lineVertices[offset+1] = (m_lineVertices[offset+1] - center) * scale + center;
		m_lineVertices[offset+2] = (m_lineVertices[offset+2] - center) * scale + center;
		m_lineVertices[offset+3] = (m_lineVertices[offset+3] - center) * scale + center;
	}
	
	private void SetContinuousTexture () {
		int idx = 0;
		float offset = 0.0f;
		SetDistances();
		int end = m_distances.Length-1;
		float totalDistance = m_distances[end];
		
		for (int i = 0; i < end; i++) {
			m_lineUVs[idx  ].x = offset;
			m_lineUVs[idx+3].x = offset;
			offset = 1.0f / (totalDistance / m_distances[i+1]);
			m_lineUVs[idx+1].x = offset;
			m_lineUVs[idx+2].x = offset;
			idx += 4;
		}
		
		if (m_vectorObject != null) {
			m_vectorObject.UpdateUVs();
		}
	}
	
	private bool UseMatrix (out Matrix4x4 thisMatrix) {
		if (m_drawTransform != null) {
			thisMatrix = m_drawTransform.localToWorldMatrix;
			return true;
		}
		else if (m_useMatrix) {
			thisMatrix = m_matrix;
			return true;
		}
		thisMatrix = Matrix4x4.identity;
		return false;
	}
	
	private bool CheckPointCount () {
		if (pointsCount < ((m_lineType == LineType.Points)? 1 : 2)) {
			ClearTriangles();
			m_vectorObject.ClearMesh();
			m_pointsCount = pointsCount;
			m_drawEnd = 0;
			return false;
		}
		return true;
	}
	
	private void ClearTriangles () {
		if (m_capType == EndCap.None) {
			m_lineTriangles.Clear();
		}
		else {
			m_lineTriangles.RemoveRange (12, m_lineTriangles.Count - 12);
		}
	}
	
	private void SetupDrawStartEnd (out int start, out int end, bool clearVertices) {
		start = 0;
		end = m_pointsCount - 1;
		if (m_drawStart > 0) {
			start = m_drawStart;
			if (m_lineType == LineType.Discrete && start == pointsCount - 1) {
				start++;
			}
			if (clearVertices) {
				ZeroVertices (0, start);
			}
		}
		if (m_drawEnd < m_pointsCount - 1) {
			end = m_drawEnd;
			if (end < 0) {
				end = 0;
			}
			if (clearVertices) {
				ZeroVertices (end, m_pointsCount);
			}
		}
		if (m_endPointsUpdate > 0) {
			start = Mathf.Max (0, end - m_endPointsUpdate);
		}
	}
	
	private void ZeroVertices (int startIndex, int endIndex) {
		if (m_lineType != LineType.Discrete) {
			startIndex *= 4;
			endIndex *= 4;
			if (endIndex > m_vertexCount) {
				endIndex -= 4;
			}
			for (int i = startIndex; i < endIndex; i += 4) {
				m_lineVertices[i  ] = v3zero;
				m_lineVertices[i+1] = v3zero;
				m_lineVertices[i+2] = v3zero;
				m_lineVertices[i+3] = v3zero;
			}
		}
		else {
			startIndex *= 2;
			endIndex *= 2;
			for (int i = startIndex; i < endIndex; i += 2) {
				m_lineVertices[i  ] = v3zero;
				m_lineVertices[i+1] = v3zero;
			}
		}
	}
	
	private void SetupCanvasState (CanvasState wantedState) {
		if (wantedState == CanvasState.OnCanvas) {
			if (m_go == null) return;
			// See if any parent object is already a canvas, and if not, parent line object to the vector canvas
			var parentObject = m_go.transform.parent;
			var doSetCanvasParent = true;
			while (parentObject != null) {
				if (parentObject.GetComponent<Canvas>() != null) {
					doSetCanvasParent = false;
					break;
				}
				parentObject = parentObject.parent;
			}
			if (doSetCanvasParent) {
				if (m_canvas == null) {
					SetupVectorCanvas();
				}
				m_go.transform.SetParent (m_canvas.transform, true);
			}
			m_canvasState = CanvasState.OnCanvas;
			
			if (m_go.GetComponent<VectorObject3D>() != null) {
				Object.DestroyImmediate (m_go.GetComponent<VectorObject3D>());
				Object.DestroyImmediate (m_go.GetComponent<MeshFilter>());
				Object.DestroyImmediate (m_go.GetComponent<MeshRenderer>());
			}
			if (m_go.GetComponent<VectorObject2D>() == null) {
				m_vectorObject = m_go.AddComponent<VectorObject2D>();
			}
			else {
				m_vectorObject = m_go.GetComponent<VectorObject2D>();
			}
			m_vectorObject.SetVectorLine (this, m_texture, m_material, false);
			return;
		}
		// OffCanvas
		if (m_go == null) return;
		m_go.transform.SetParent (null);
		m_canvasState = CanvasState.OffCanvas;
		if (m_go.GetComponent<VectorObject2D>() != null) {
			m_go.GetComponent<VectorObject2D>().DestroyNow();
			Object.DestroyImmediate (m_go.GetComponent<VectorObject2D>());
		}
		m_vectorObject = m_go.GetComponent<VectorObject3D>();
		if (m_vectorObject == null) {
			m_vectorObject = m_go.AddComponent<VectorObject3D>();
		}
		var useCustomMaterial = true;
		if (m_material == null) {
			var mat = Resources.Load ("DefaultLine3D") as Material;
			if (mat == null) {
				Debug.LogError ("No DefaultLine3D material found in Resources");
				return;
			}
			m_material = new Material(mat);
			useCustomMaterial = false;
		}
		m_vectorObject.SetVectorLine (this, m_texture, m_material, useCustomMaterial);
	}
	
	public void Draw () {
		if (!m_active) return;
		if (m_canvasState != CanvasState.OnCanvas) {
			SetupCanvasState (CanvasState.OnCanvas);
		}
		if (m_vectorObject == null) {	// In case the reference is lost in the editor
			m_vectorObject = m_go.GetComponent<VectorObject2D>();
		}
		if (!CheckPointCount() || m_lineWidths == null) return;
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		if (m_lineType == LineType.Points) {
			DrawPoints();
			return;
		}
		
		Matrix4x4 thisMatrix;
		bool useMatrix = UseMatrix (out thisMatrix);
		int start = 0, end = 0;
		SetupDrawStartEnd (out start, out end, true);
		if (m_is2D) {
			Line2D (start, end, thisMatrix, useMatrix);
		}
		else {
			Line3D (start, end, thisMatrix, useMatrix);
		}
		
		CheckNormals();
		CheckLine (false);
		if (m_useTextureScale) {
			SetTextureScale();
		}
		m_vectorObject.UpdateVerts();
		if (m_collider) {
			SetCollider (true);
		}
	}
	
	private void Line2D (int start, int end, Matrix4x4 thisMatrix, bool useTransformMatrix) {
		Vector3 p1 = v3zero, p2 = v3zero, v1 = v3zero, px = v3zero;
		Vector2 scaleFactor = new Vector2(Screen.width, Screen.height);
		
		int add = 0, idx = 0, widthIdx = 0;
		int widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		if (m_lineType == LineType.Continuous) {
			add = 1;
			idx = start*4;
		}
		else {
			add = 2;
			widthIdx /= 2;
			idx = start*2;
		}
		float normalizedDistance = 0.0f;
		bool doSmooth = (smoothWidth == true && m_lineWidths.Length > 1);
		
		for (int i = start; i < end; i += add) {
			if (useTransformMatrix) {
				p1 = thisMatrix.MultiplyPoint3x4 (m_points2[i]);
				p2 = thisMatrix.MultiplyPoint3x4 (m_points2[i+1]);
			}
			else {
				p1.x = m_points2[i].x; p1.y = m_points2[i].y;
				p2.x = m_points2[i+1].x; p2.y = m_points2[i+1].y;
			}
			if (m_viewportDraw) {
				p1.x *= scaleFactor.x; p1.y *= scaleFactor.y;
				p2.x *= scaleFactor.x; p2.y *= scaleFactor.y;
			}
			if (p1.x == p2.x && p1.y == p2.y) {
				SkipQuad (ref idx, ref widthIdx, ref widthIdxAdd);
				continue;
			}
			
			if (m_capLength == 0.0f) {
				px.x = p2.y - p1.y; px.y = p1.x - p2.x;
				normalizedDistance = ( 1.0f / (float)System.Math.Sqrt ((px.x * px.x) + (px.y * px.y)) );
				px *= normalizedDistance * m_lineWidths[widthIdx];
				m_lineVertices[idx  ].x = p1.x - px.x; m_lineVertices[idx  ].y = p1.y - px.y; 
				m_lineVertices[idx+3].x = p1.x + px.x; m_lineVertices[idx+3].y = p1.y + px.y;
				if (doSmooth && i < end-add) {
					px.x = p2.y - p1.y; px.y = p1.x - p2.x;
					px *= normalizedDistance * m_lineWidths[widthIdx+1];
				}
			}
			else {
				px.x = p2.x - p1.x; px.y = p2.y - p1.y;
				px *= ( 1.0f / (float)System.Math.Sqrt ((px.x * px.x) + (px.y * px.y)) );
				p1 -= px * m_capLength;
				p2 += px * m_capLength;
				
				v1.x = px.y; v1.y = -px.x;
				px = v1 * m_lineWidths[widthIdx];
				m_lineVertices[idx  ].x = p1.x - px.x; m_lineVertices[idx  ].y = p1.y - px.y;
				m_lineVertices[idx+3].x = p1.x + px.x; m_lineVertices[idx+3].y = p1.y + px.y;
				if (doSmooth && i < end-add) {
					px = v1 * m_lineWidths[widthIdx+1];
				}
			}
			m_lineVertices[idx+2].x = p2.x + px.x; m_lineVertices[idx+2].y = p2.y + px.y;
			m_lineVertices[idx+1].x = p2.x - px.x; m_lineVertices[idx+1].y = p2.y - px.y;
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		if (m_joins == Joins.Weld) {
			if (m_lineType == LineType.Continuous) {
				WeldJoins (start*4 + (start == 0? 4 : 0), end*4, Approximately (m_points2[0], m_points2[m_pointsCount-1]));
			}
			else {
				if ((end & 1) == 0) {	// end should be odd for discrete lines
					end--;
				}
				WeldJoinsDiscrete (start + 1, end, Approximately (m_points2[0], m_points2[m_pointsCount-1]));
			}
		}
		CheckDrawStartFill (start);
	}
	
	private void Line3D (int start, int end, Matrix4x4 thisMatrix, bool useTransformMatrix) {
		if (!CheckCamera3D()) return;		
		Vector3 pos1 = v3zero, pos2 = v3zero, v1 = v3zero, px = v3zero, p1 = v3zero, p2 = v3zero;
		float normalizedDistance = 0.0f;
		int widthIdx = 0, widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		int idx = start * 2;
		int add = 2;
		
		if (m_lineType == LineType.Continuous) {
			idx = start * 4;
			add = 1;
		}
		
		var cameraPlane = new Plane(camTransform.forward, camTransform.position + camTransform.forward * cam3D.nearClipPlane);
		var ray = new Ray(v3zero, v3zero);
		float screenHeight = Screen.height;
		bool doSmooth = (smoothWidth == true && m_lineWidths.Length > 1);
		
		for (int i = start; i < end; i += add) {
			if (useTransformMatrix) {
				p1 = thisMatrix.MultiplyPoint3x4 (m_points3[i]);
				p2 = thisMatrix.MultiplyPoint3x4 (m_points3[i+1]);
			}
			else {
				p1 = m_points3[i];
				p2 = m_points3[i+1];
			}
			pos1 = cam3D.WorldToScreenPoint (p1);
			pos2 = cam3D.WorldToScreenPoint (p2);
			
			if ((pos1.x == pos2.x && pos1.y == pos2.y) || IntersectAndDoSkip (ref pos1, ref pos2, ref p1, ref p2, ref screenHeight, ref ray, ref cameraPlane)) {
				SkipQuad (ref idx, ref widthIdx, ref widthIdxAdd);
				continue;
			}
			
			if (m_capLength == 0.0f) {
				px.x = pos2.y - pos1.y; px.y = pos1.x - pos2.x;
				normalizedDistance = 1.0f / (float)System.Math.Sqrt ((px.x * px.x) + (px.y * px.y));
				px.x *= normalizedDistance * m_lineWidths[widthIdx]; px.y *= normalizedDistance * m_lineWidths[widthIdx];
				m_lineVertices[idx  ].x = pos1.x - px.x; m_lineVertices[idx  ].y = pos1.y - px.y;
				m_lineVertices[idx+3].x = pos1.x + px.x; m_lineVertices[idx+3].y = pos1.y + px.y;
				if (doSmooth && i < end - add) {
					px.x = pos2.y - pos1.y; px.y = pos1.x - pos2.x;
					px.x *= normalizedDistance * m_lineWidths[widthIdx+1]; px.y *= normalizedDistance * m_lineWidths[widthIdx+1];
				}
			}
			else {
				px.x = pos2.x - pos1.x; px.y = pos2.y - pos1.y;
				px *= ( 1.0f / (float)System.Math.Sqrt ((px.x * px.x) + (px.y * px.y)) );
				pos1 -= px * m_capLength;
				pos2 += px * m_capLength;
				
				v1.x = px.y; v1.y = -px.x;
				px = v1 * m_lineWidths[widthIdx];
				m_lineVertices[idx  ].x = pos1.x - px.x; m_lineVertices[idx  ].y = pos1.y - px.y;
				m_lineVertices[idx+3].x = pos1.x + px.x; m_lineVertices[idx+3].y = pos1.y + px.y;
				if (doSmooth && i < end-add) {
					px = v1 * m_lineWidths[widthIdx+1];
				}
			}
			m_lineVertices[idx+2].x = pos2.x + px.x; m_lineVertices[idx+2].y = pos2.y + px.y;
			m_lineVertices[idx+1].x = pos2.x - px.x; m_lineVertices[idx+1].y = pos2.y - px.y;
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		if (m_joins == Joins.Weld && end - start > 1) { // Only weld if there's more than one segment
			if (m_lineType == LineType.Continuous) {
				WeldJoins (start*4 + (start == 0? 4 : 0), end*4, start == 0 && end == m_pointsCount-1 && Approximately (m_points3[0], m_points3[m_pointsCount-1]));
			}
			else {
				if ((end & 1) == 0) {	// end should be odd for discrete lines
					end--;
				}
				WeldJoinsDiscrete (start + 1, end, start == 0 && end == m_pointsCount-1 && Approximately (m_points3[0], m_points3[m_pointsCount-1]));
			}
		}
		CheckDrawStartFill (start);
	}
	
	void CheckDrawStartFill (int start) {
		// Prevent drawStart > 0 and Joins.Fill from creating a triangle that connects to the origin
		if (m_joins == Joins.Fill) {
			int idx = start * 4;
			if (m_drawStart > 0 && m_lineVertices.Length > idx && idx-3 >= 0) {
				m_lineVertices[idx-1] = m_lineVertices[idx];
				m_lineVertices[idx-2] = m_lineVertices[idx];
				m_lineVertices[idx-3] = m_lineVertices[idx];
			}
		}
	}

	public void Draw3D () {
		if (!m_active) return;
		if (m_is2D) {
			Debug.LogError ("VectorLine.Draw3D can only be used with a Vector3 array, which \"" + name + "\" doesn't have");
			return;
		}
		if (m_canvasState != CanvasState.OffCanvas) {
			SetupCanvasState (CanvasState.OffCanvas);
		}
		if (!CheckPointCount() || m_lineWidths == null) return;
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		if (!CheckCamera3D()) return;
		if (m_lineType == LineType.Points) {
			DrawPoints3D();
			return;
		}
		
		int start = 0, end = 0, add = 0, widthIdx = 0;
		SetupDrawStartEnd (out start, out end, true);
		Matrix4x4 thisMatrix;
		bool useTransformMatrix = UseMatrix (out thisMatrix);
		bool doSmooth = (smoothWidth == true && m_lineWidths.Length > 1);
		
		int idx = 0, widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		if (m_lineType == LineType.Continuous) {
			add = 1;
			idx = start*4;
		}
		else {
			widthIdx /= 2;
			add = 2;
			idx = start*2;
		}
		Vector3 thisLine = v3zero, px = v3zero, pos1 = v3zero, pos2 = v3zero, p1 = v3zero, p2 = v3zero;
		var cameraPlane = new Plane(camTransform.forward, camTransform.position + camTransform.forward * cam3D.nearClipPlane);
		var ray = new Ray(v3zero, v3zero);
		float screenHeight = Screen.height;
		
		for (int i = start; i < end; i += add) {
			if (useTransformMatrix) {
				p1 = thisMatrix.MultiplyPoint3x4 (m_points3[i]);
				p2 = thisMatrix.MultiplyPoint3x4 (m_points3[i+1]);
			}
			else {
				p1 = m_points3[i];
				p2 = m_points3[i+1];
			}
			pos1 = cam3D.WorldToScreenPoint (p1);
			pos2 = cam3D.WorldToScreenPoint (p2);
			
			if ((pos1.x == pos2.x && pos1.y == pos2.y) || IntersectAndDoSkip (ref pos1, ref pos2, ref p1, ref p2, ref screenHeight, ref ray, ref cameraPlane)) {
				SkipQuad3D (ref idx, ref widthIdx, ref widthIdxAdd);
				continue;
			}
			
			px.x = pos2.y - pos1.y; px.y = pos1.x - pos2.x;
			thisLine = px / (float)System.Math.Sqrt (px.x * px.x + px.y * px.y);
			px.x = thisLine.x * m_lineWidths[widthIdx]; px.y = thisLine.y * m_lineWidths[widthIdx];
			
			// Screenpoints used for Joins.Weld and end caps
			m_screenPoints[idx  ].x = pos1.x - px.x; m_screenPoints[idx  ].y = pos1.y - px.y; m_screenPoints[idx  ].z = pos1.z - px.z;
			m_screenPoints[idx+3].x = pos1.x + px.x; m_screenPoints[idx+3].y = pos1.y + px.y; m_screenPoints[idx+3].z = pos1.z + px.z; 
			m_lineVertices[idx  ] = cam3D.ScreenToWorldPoint (m_screenPoints[idx]);
			m_lineVertices[idx+3] = cam3D.ScreenToWorldPoint (m_screenPoints[idx+3]);
			if (doSmooth && i < end-add) {
				px.x = thisLine.x * m_lineWidths[widthIdx+1]; px.y = thisLine.y * m_lineWidths[widthIdx+1];
			}
			m_screenPoints[idx+2].x = pos2.x + px.x; m_screenPoints[idx+2].y = pos2.y + px.y; m_screenPoints[idx+2].z = pos2.z + px.z;
			m_screenPoints[idx+1].x = pos2.x - px.x; m_screenPoints[idx+1].y = pos2.y - px.y; m_screenPoints[idx+1].z = pos2.z - px.z;
			m_lineVertices[idx+2] = cam3D.ScreenToWorldPoint (m_screenPoints[idx+2]);
			m_lineVertices[idx+1] = cam3D.ScreenToWorldPoint (m_screenPoints[idx+1]);
			
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		if (m_joins == Joins.Weld && end - start > 1) { // Only weld if there's more than one segment
			if (m_lineType == LineType.Continuous) {
				WeldJoins3D (start*4 + (start == 0? 4 : 0), end*4, start == 0 && end == m_pointsCount-1 && Approximately (m_points3[0], m_points3[m_pointsCount-1]));
			}
			else {
				if ((end & 1) == 0) {	// End should be odd for discrete lines
					end--;
				}
				WeldJoinsDiscrete3D (start + 1, end, start == 0 && end == m_pointsCount-1 && Approximately (m_points3[0], m_points3[m_pointsCount-1]));
			}
		}
		CheckDrawStartFill (start);
		
		CheckLine (true);
		if (m_useTextureScale) {
			SetTextureScale();
		}
		m_vectorObject.UpdateVerts();
		CheckNormals();
		if (m_collider) {
			SetCollider (false);
		}
	}
	
	private bool IntersectAndDoSkip (ref Vector3 pos1, ref Vector3 pos2, ref Vector3 p1, ref Vector3 p2, ref float screenHeight, ref Ray ray, ref Plane cameraPlane) {
		// If point is behind camera, intersect segment with camera plane to avoid glitches
		if (pos1.z < 0.0f) {
			if (pos2.z < 0.0f) {	// If both points are behind camera, skip
				return true;
			}
			pos1 = cam3D.WorldToScreenPoint (PlaneIntersectionPoint (ref ray, ref cameraPlane, ref p2, ref p1));
			// If WorldToScreenPoint produces weird coords compared to the actual point, it (hopefully) means the segment isn't visible, so skip 
			Vector3 relativeP = camTransform.InverseTransformPoint (p1);
			if ((relativeP.y < -1.0f && pos1.y > screenHeight) || (relativeP.y > 1.0f && pos1.y < 0.0f)) {
				return true;
			}
		}
		if (pos2.z < 0.0f) {
			pos2 = cam3D.WorldToScreenPoint (PlaneIntersectionPoint (ref ray, ref cameraPlane, ref p1, ref p2));
			Vector3 relativeP = camTransform.InverseTransformPoint (p2);
			if ((relativeP.y < -1.0f && pos2.y > screenHeight) || (relativeP.y > 1.0f && pos2.y < 0.0f)) {
				return true;
			}
		}
		return false;
	}
	
	private Vector3 PlaneIntersectionPoint (ref Ray ray, ref Plane plane, ref Vector3 p1, ref Vector3 p2) {
		ray.origin = p1;
		ray.direction = p2 - p1;
		float rayDistance = 0.0f;
		plane.Raycast (ray, out rayDistance);
		return ray.GetPoint (rayDistance);
	}
	
	private void DrawPoints () {
		if (!CheckCamera3D()) return;
		
		Matrix4x4 thisMatrix;
		bool useMatrix = UseMatrix (out thisMatrix);
		
		int start, end;
		SetupDrawStartEnd (out start, out end, true);
		Vector2 scaleFactor = new Vector2(Screen.width, Screen.height);
		
		Vector3 p1;
		int idx = start*4;
		int widthIdxAdd = (m_lineWidths.Length > 1)? 1 : 0;
		int widthIdx = start;
		var v1 = new Vector3(m_lineWidths[0], m_lineWidths[0], 0.0f);
		var v2 = new Vector3(-m_lineWidths[0], m_lineWidths[0], 0.0f);
		
		if (m_is2D) {
			for (int i = start; i <= end; i++) {
				if (useMatrix) {
					p1 = thisMatrix.MultiplyPoint3x4 (m_points2[i]);
				}
				else {
					p1.x = m_points2[i].x; p1.y = m_points2[i].y;
				}
				if (m_viewportDraw) {
					p1.x *= scaleFactor.x; p1.y *= scaleFactor.y;
				}
				
				if (widthIdxAdd != 0) {
					v1.x = v1.y = v2.y = m_lineWidths[widthIdx];
					v2.x = -m_lineWidths[widthIdx];
					widthIdx++;
				}
				
				m_lineVertices[idx  ].x = p1.x + v2.x; m_lineVertices[idx  ].y = p1.y + v2.y;
				m_lineVertices[idx+3].x = p1.x - v1.x; m_lineVertices[idx+3].y = p1.y - v1.y;
				m_lineVertices[idx+1].x = p1.x + v1.x; m_lineVertices[idx+1].y = p1.y + v1.y;
				m_lineVertices[idx+2].x = p1.x - v2.x; m_lineVertices[idx+2].y = p1.y - v2.y;
				idx += 4;
			}
		}
		else {
			for (int i = start; i <= end; i++) {
				p1 = useMatrix? cam3D.WorldToScreenPoint (thisMatrix.MultiplyPoint3x4(m_points3[i])) :
								cam3D.WorldToScreenPoint (m_points3[i]);
				if (p1.z < 0) {
					SkipQuad (ref idx, ref widthIdx, ref widthIdxAdd);
					continue;
				}
				
				if (widthIdxAdd != 0) {
					v1.x = v1.y = v2.y = m_lineWidths[widthIdx];
					v2.x = -m_lineWidths[widthIdx];
					widthIdx++;
				}
				
				m_lineVertices[idx  ].x = p1.x + v2.x; m_lineVertices[idx  ].y = p1.y + v2.y;
				m_lineVertices[idx+3].x = p1.x - v1.x; m_lineVertices[idx+3].y = p1.y - v1.y;
				m_lineVertices[idx+1].x = p1.x + v1.x; m_lineVertices[idx+1].y = p1.y + v1.y;
				m_lineVertices[idx+2].x = p1.x - v2.x; m_lineVertices[idx+2].y = p1.y - v2.y;
				idx += 4;
			}
		}
		CheckNormals();
		m_vectorObject.UpdateVerts();
	}

	private void DrawPoints3D () {
		if (!m_active) return;
		Matrix4x4 thisMatrix;
		bool useMatrix = UseMatrix (out thisMatrix);
		
		int start = 0, end = 0, widthIdx = 0;
		SetupDrawStartEnd (out start, out end, true);
				
		int idx = start*4;
		int widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		Vector3 p1 = v3zero, v1 = v3zero, v2 = v3zero;
		for (int i = start; i <= end; i++) {
			p1 = useMatrix? cam3D.WorldToScreenPoint (thisMatrix.MultiplyPoint3x4 (m_points3[i])) :
							cam3D.WorldToScreenPoint (m_points3[i]);
			if (p1.z < 0) {
				SkipQuad (ref idx, ref widthIdx, ref widthIdxAdd);
				continue;
			}
			v1.x = v1.y = v2.y = m_lineWidths[widthIdx];
			v2.x = -m_lineWidths[widthIdx];
			
			m_lineVertices[idx  ] = cam3D.ScreenToWorldPoint (p1 + v2);
			m_lineVertices[idx+3] = cam3D.ScreenToWorldPoint (p1 - v1);
			m_lineVertices[idx+1] = cam3D.ScreenToWorldPoint (p1 + v1);
			m_lineVertices[idx+2] = cam3D.ScreenToWorldPoint (p1 - v2);
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		CheckNormals();
		m_vectorObject.UpdateVerts();
	}
	
	private void SkipQuad (ref int idx, ref int widthIdx, ref int widthIdxAdd) {
		m_lineVertices[idx  ] = v3zero;
		m_lineVertices[idx+1] = v3zero;
		m_lineVertices[idx+2] = v3zero;
		m_lineVertices[idx+3] = v3zero;
		
		idx += 4;
		widthIdx += widthIdxAdd;
	}
	
	private void SkipQuad3D (ref int idx, ref int widthIdx, ref int widthIdxAdd) {
		m_lineVertices[idx  ] = v3zero;
		m_lineVertices[idx+1] = v3zero;
		m_lineVertices[idx+2] = v3zero;
		m_lineVertices[idx+3] = v3zero;
		
		m_screenPoints[idx  ] = v3zero;
		m_screenPoints[idx+1] = v3zero;
		m_screenPoints[idx+2] = v3zero;
		m_screenPoints[idx+3] = v3zero;
		
		idx += 4;
		widthIdx += widthIdxAdd;
	}
	
	private void WeldJoins (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			SetIntersectionPoint (m_vertexCount-4, m_vertexCount-3, 0, 1);
			SetIntersectionPoint (m_vertexCount-1, m_vertexCount-2, 3, 2);
		}
		for (int i = start; i < end; i+= 4) {
			SetIntersectionPoint (i-4, i-3, i,   i+1);
			SetIntersectionPoint (i-1, i-2, i+3, i+2);
		}
	}

	private void WeldJoinsDiscrete (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			SetIntersectionPoint (m_vertexCount-4, m_vertexCount-3, 0, 1);
			SetIntersectionPoint (m_vertexCount-1, m_vertexCount-2, 3, 2);
		}
		int idx = (start+1) / 2 * 4;
		if (m_is2D) {
			for (int i = start; i < end; i+= 2) {
				if (m_points2[i] == m_points2[i+1]) {
					SetIntersectionPoint (idx-4, idx-3, idx,   idx+1);
					SetIntersectionPoint (idx-1, idx-2, idx+3, idx+2);
				}
				idx += 4;
			}
		}
		else {
			for (int i = start; i < end; i+= 2) {
				if (m_points3[i] == m_points3[i+1]) {
					SetIntersectionPoint (idx-4, idx-3, idx,   idx+1);
					SetIntersectionPoint (idx-1, idx-2, idx+3, idx+2);
				}
				idx += 4;
			}
		}
	}
	
	private void SetIntersectionPoint (int p1, int p2, int p3, int p4) {
		var l1a = m_lineVertices[p1]; var l1b = m_lineVertices[p2];
		var l2a = m_lineVertices[p3]; var l2b = m_lineVertices[p4];
		if ((l1a.x == l1b.x && l1a.y == l1b.y) || (l2a.x == l2b.x && l2a.y == l2b.y)) return;

		float d = (l2b.y - l2a.y)*(l1b.x - l1a.x) - (l2b.x - l2a.x)*(l1b.y - l1a.y);
		if (d > -0.005f && d < 0.005f) {	// Sometimes nearly parallel lines have errors, so just average the points together
			if (Mathf.Abs (l1b.x - l2a.x) < .005f && Mathf.Abs (l1b.y - l2a.y) < .005f) {	// But only if the points are mostly the same
				m_lineVertices[p2] = (l1b + l2a) * 0.5f;
				m_lineVertices[p3] = m_lineVertices[p2];
			}
			return;	// Otherwise that means the line is going back on itself, so do nothing
		}
		float n = ( (l2b.x - l2a.x)*(l1a.y - l2a.y) - (l2b.y - l2a.y)*(l1a.x - l2a.x) ) / d;
		
		var v3 = new Vector3(l1a.x + (n * (l1b.x - l1a.x)), l1a.y + (n * (l1b.y - l1a.y)), l1a.z);
		if ((v3 - l1b).sqrMagnitude > m_maxWeldDistance) return;
		m_lineVertices[p2] = v3;
		m_lineVertices[p3] = v3;
	}
	
	private void WeldJoins3D (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			SetIntersectionPoint3D (m_vertexCount-4, m_vertexCount-3, 0, 1);
			SetIntersectionPoint3D (m_vertexCount-1, m_vertexCount-2, 3, 2);
		}
		if (m_drawStart > 0) {
			start += 4;
		}
		for (int i = start; i < end; i+= 4) {
			SetIntersectionPoint3D (i-4, i-3, i,   i+1);
			SetIntersectionPoint3D (i-1, i-2, i+3, i+2);
		}
	}
	
	private void WeldJoinsDiscrete3D (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			SetIntersectionPoint3D (m_vertexCount-4, m_vertexCount-3, 0, 1);
			SetIntersectionPoint3D (m_vertexCount-1, m_vertexCount-2, 3, 2);
		}
		int idx = (start+1) / 2 * 4;
		for (int i = start; i < end; i+= 2) {
			if (m_points3[i] == m_points3[i+1]) {
				SetIntersectionPoint3D (idx-4, idx-3, idx,   idx+1);
				SetIntersectionPoint3D (idx-1, idx-2, idx+3, idx+2);
			}
			idx += 4;
		}
	}

	private void SetIntersectionPoint3D (int p1, int p2, int p3, int p4) {
		var l1a = m_screenPoints[p1]; var l1b = m_screenPoints[p2];
		var l2a = m_screenPoints[p3]; var l2b = m_screenPoints[p4];
		if ((l1a.x == l1b.x && l1a.y == l1b.y) || (l2a.x == l2b.x && l2a.y == l2b.y)) return;

		float d = (l2b.y - l2a.y)*(l1b.x - l1a.x) - (l2b.x - l2a.x)*(l1b.y - l1a.y);
		if (d > -0.005f && d < 0.005f) {	// Sometimes nearly parallel lines have errors, so just average the points together
			if (Mathf.Abs (l1b.x - l2a.x) < .005f && Mathf.Abs (l1b.y - l2a.y) < .005f) {	// But only if the points are mostly the same
				m_lineVertices[p2] = cam3D.ScreenToWorldPoint ((l1b + l2a) * 0.5f);
				m_lineVertices[p3] = m_lineVertices[p2];
			}
			return;	// Otherwise that means the line is going back on itself, so do nothing
		}
		float n = ( (l2b.x - l2a.x)*(l1a.y - l2a.y) - (l2b.y - l2a.y)*(l1a.x - l2a.x) ) / d;
		
		var v3 = new Vector3(l1a.x + (n * (l1b.x - l1a.x)), l1a.y + (n * (l1b.y - l1a.y)), l1a.z);
		if ((v3 - l1b).sqrMagnitude > m_maxWeldDistance) return;
		m_lineVertices[p2] = cam3D.ScreenToWorldPoint (v3);
		m_lineVertices[p3] = m_lineVertices[p2];
	}
	
	public static void LineManagerCheckDistance () {
		lineManager.StartCheckDistance();
	}
	
	public static void LineManagerDisable () {
		lineManager.DisableIfUnused();
	}
	
	public static void LineManagerEnable () {
		lineManager.EnableIfUsed();
	}
	
	public void Draw3DAuto () {
		Draw3DAuto (0.0f);
	}
	
	public void Draw3DAuto (float time) {
		if (time < 0.0f) time = 0.0f;
		lineManager.AddLine (this, m_drawTransform, time);
		m_isAutoDrawing = true;
		Draw3D();
	}
	
	public void StopDrawing3DAuto () {
		lineManager.RemoveLine (this);
		m_isAutoDrawing = false;
	}
	
	private void SetTextureScale () {
		if (pointsCount != m_pointsCount) {
			Resize();
		}
		int start, end;
		SetupDrawStartEnd (out start, out end, false);
		int add = (m_lineType != LineType.Discrete)? 1 : 2;
		int idx = 0;
		int widthIdx = 0;
		int widthIdxAdd = m_lineWidths.Length == 1? 0 : 1;
		float thisScale = 1.0f / m_textureScale;
		var useTransformMatrix = (m_drawTransform != null);
		var thisMatrix = useTransformMatrix? m_drawTransform.localToWorldMatrix : Matrix4x4.identity;
		var p1 = Vector2.zero;
		var p2 = Vector2.zero;
		var px = Vector2.zero;
		float offset = m_textureOffset;
		float capAdd = m_capLength*2;
		
		if (m_is2D) {
			for (int i = 0; i < end; i += add) {
				if (!m_viewportDraw) {
					if (useTransformMatrix) {
						p1 = thisMatrix.MultiplyPoint3x4 (m_points2[i]);
						p2 = thisMatrix.MultiplyPoint3x4 (m_points2[i+1]);
					}
					else {
						p1.x = m_points2[i].x; p1.y = m_points2[i].y;
						p2.x = m_points2[i+1].x; p2.y = m_points2[i+1].y;
					}
				}
				else {
					if (useTransformMatrix) {
						p1 = thisMatrix.MultiplyPoint3x4 (new Vector2(m_points2[i].x * Screen.width, m_points2[i].y * Screen.height));
						p2 = thisMatrix.MultiplyPoint3x4 (new Vector2(m_points2[i+1].x * Screen.width, m_points2[i+1].y * Screen.height));
					}
					else {
						p1 = new Vector2(m_points2[i].x * Screen.width, m_points2[i].y * Screen.height);
						p2 = new Vector2(m_points2[i+1].x * Screen.width, m_points2[i+1].y * Screen.height);
					}
				}
				px.x = p2.x - p1.x; px.y = p2.y - p1.y;
				float xPos = thisScale / (m_lineWidths[widthIdx]*2 / ((float)System.Math.Sqrt ((px.x * px.x) + (px.y * px.y)) + capAdd) );
				m_lineUVs[idx  ].x = offset;
				m_lineUVs[idx+3].x = offset;
				m_lineUVs[idx+2].x = xPos + offset;
				m_lineUVs[idx+1].x = xPos + offset;
				idx += 4;
				offset = (offset + xPos) % 1;
				widthIdx += widthIdxAdd;
			}
		}
		else {
			if (!CheckCamera3D()) return;
			
			for (int i = 0; i < end; i += add) {
				if (useTransformMatrix) {
					p1 = cam3D.WorldToScreenPoint (thisMatrix.MultiplyPoint3x4 (m_points3[i]));
					p2 = cam3D.WorldToScreenPoint (thisMatrix.MultiplyPoint3x4 (m_points3[i+1]));
				}
				else {
					p1 = cam3D.WorldToScreenPoint (m_points3[i]);
					p2 = cam3D.WorldToScreenPoint (m_points3[i+1]);
				}
				px.x = p1.x - p2.x; px.y = p1.y - p2.y;
				float xPos = thisScale / (m_lineWidths[widthIdx]*2 / (float)System.Math.Sqrt (px.x * px.x + px.y * px.y));
				m_lineUVs[idx  ].x = offset;
				m_lineUVs[idx+3].x = offset;
				m_lineUVs[idx+2].x = xPos + offset;
				m_lineUVs[idx+1].x = xPos + offset;
				idx += 4;
				offset = (offset + xPos) % 1;
				widthIdx += widthIdxAdd;
			}
		}
		
		if (m_vectorObject != null) {
			m_vectorObject.UpdateUVs();
		}
	}

	private void ResetTextureScale () {
		for (int i = 0; i < m_vertexCount; i += 4) {
			m_lineUVs[i  ].x = 0.0f;
			m_lineUVs[i+3].x = 0.0f;
			m_lineUVs[i+2].x = 1.0f;
			m_lineUVs[i+1].x = 1.0f;
		}
		if (m_vectorObject != null) {
			m_vectorObject.UpdateUVs();
		}
	}
	
	private void SetCollider (bool convertToWorldSpace) {
		if (!cam3D) {
			SetCamera3D();
			if (!cam3D) {
				Debug.LogError ("No camera available...use VectorLine.SetCamera3D to assign a camera");
				return;
			}
		}
		if (cam3D.transform.rotation != Quaternion.identity) {
			Debug.LogWarning ("The line collider will not be correct if the camera is rotated");
		}
		
		var v3 = new Vector3(0.0f, 0.0f, -cam3D.transform.position.z);
		int min = drawStart;
		int max = drawEnd;
		var addFrontCap = (m_capType != EndCap.None && m_capType <= EndCap.Mirror && drawStart == 0);
		var addBackCap = (m_capType != EndCap.None && m_capType >= EndCap.Both && drawEnd == pointsCount-1);
		int i = 0;
		if (m_lineType == LineType.Continuous) {
			var collider = m_go.GetComponent (typeof(EdgeCollider2D)) as EdgeCollider2D;
			int totalCount = (max - min) * 4 + 1;
			if (addFrontCap) {
				totalCount += 4;
			}
			if (addBackCap) {
				totalCount += 4;
			}
			var path = new Vector2[totalCount];
			
			int startIdx = 0;
			int endIdx = path.Length - 2;
			if (convertToWorldSpace) {
				if (addFrontCap) {
					i = m_vertexCount;
					SetPathWorldVerticesContinuous (ref i, ref v3, ref startIdx, ref endIdx, path);
				}
				for (i = min*4; i < max*4; i += 4) {
					SetPathWorldVerticesContinuous (ref i, ref v3, ref startIdx, ref endIdx, path);
				}
				if (addBackCap) {
					i = m_vertexCount + 4;
					SetPathWorldVerticesContinuous (ref i, ref v3, ref startIdx, ref endIdx, path);
				}
			}
			else {
				if (addFrontCap) {
					i = m_vertexCount;
					SetPathVerticesContinuous (ref i, ref startIdx, ref endIdx, path);
				}
				for (i = min*4; i < max*4; i += 4) {
					SetPathVerticesContinuous (ref i, ref startIdx, ref endIdx, path);
				}
				if (addFrontCap) {
					i = m_vertexCount + 4;
					SetPathVerticesContinuous (ref i, ref startIdx, ref endIdx, path);
				}
			}
			path[path.Length - 1] = path[0];
			collider.points = path;
		}
		else {	// Discrete line
			var collider = m_go.GetComponent (typeof(PolygonCollider2D)) as PolygonCollider2D;
			var path = new Vector2[4];
			int totalCount = ((max - min) + 1) / 2;
			if (addFrontCap) {
				totalCount++;
			}
			if (addBackCap) {
				totalCount++;
			}
			collider.pathCount = totalCount;
			
			int end = (max + 1) / 2 * 4;
			int pIdx = 0;
			if (convertToWorldSpace) {
				if (addFrontCap) {
					i = m_vertexCount;
					SetPathWorldVerticesDiscrete (ref i, ref v3, ref pIdx, path, collider);
				}
				for (i = min / 2 * 4; i < end; i += 4) {
					SetPathWorldVerticesDiscrete (ref i, ref v3, ref pIdx, path, collider);
				}
				if (addBackCap) {
					i = m_vertexCount + 4;
					SetPathWorldVerticesDiscrete (ref i, ref v3, ref pIdx, path, collider);
				}
			}
			else {
				if (addFrontCap) {
					i = m_vertexCount;
					SetPathVerticesDiscrete (ref i, ref pIdx, path, collider);
				}
				for (i = min / 2 * 4; i < end; i += 4) {
					SetPathVerticesDiscrete (ref i, ref pIdx, path, collider);
				}
				if (addBackCap) {
					i = m_vertexCount + 4;
					SetPathVerticesDiscrete (ref i, ref pIdx, path, collider);
				}
			}
		}
	}
	
	private void SetPathVerticesContinuous (ref int i, ref int startIdx, ref int endIdx, Vector2[] path) {
		path[startIdx  ].x = m_lineVertices[i  ].x;	path[startIdx  ].y = m_lineVertices[i  ].y;
		path[startIdx+1].x = m_lineVertices[i+1].x;	path[startIdx+1].y = m_lineVertices[i+1].y;
		path[endIdx  ].x = m_lineVertices[i+3].x; 	path[endIdx  ].y = m_lineVertices[i+3].y;
		path[endIdx-1].x = m_lineVertices[i+2].x;	path[endIdx-1].y = m_lineVertices[i+2].y;
		startIdx += 2;
		endIdx -= 2;
	}
	
	private void SetPathWorldVerticesContinuous (ref int i, ref Vector3 v3, ref int startIdx, ref int endIdx, Vector2[] path) {
		v3.x = m_lineVertices[i  ].x; v3.y = m_lineVertices[i  ].y;
		path[startIdx  ] = cam3D.ScreenToWorldPoint (v3);
		v3.x = m_lineVertices[i+1].x; v3.y = m_lineVertices[i+1].y;
		path[startIdx+1] = cam3D.ScreenToWorldPoint (v3);
		v3.x = m_lineVertices[i+3].x; v3.y = m_lineVertices[i+3].y;
		path[endIdx  ] = cam3D.ScreenToWorldPoint (v3);
		v3.x = m_lineVertices[i+2].x; v3.y = m_lineVertices[i+2].y;
		path[endIdx-1] = cam3D.ScreenToWorldPoint (v3);
		startIdx += 2;
		endIdx -= 2;
	}

	private void SetPathVerticesDiscrete (ref int i, ref int pIdx, Vector2[] path, PolygonCollider2D collider) {
		path[0].x = m_lineVertices[i  ].x; path[0].y = m_lineVertices[i  ].y;
		path[1].x = m_lineVertices[i+3].x; path[1].y = m_lineVertices[i+3].y;
		path[2].x = m_lineVertices[i+2].x; path[2].y = m_lineVertices[i+2].y;
		path[3].x = m_lineVertices[i+1].x; path[3].y = m_lineVertices[i+1].y;
		collider.SetPath (pIdx++, path);
	}
	
	private void SetPathWorldVerticesDiscrete (ref int i, ref Vector3 v3, ref int pIdx, Vector2[] path, PolygonCollider2D collider) {
		v3.x = m_lineVertices[i  ].x; v3.y = m_lineVertices[i  ].y;
		path[0] = cam3D.ScreenToWorldPoint (v3);
		v3.x = m_lineVertices[i+3].x; v3.y = m_lineVertices[i+3].y;
		path[1] = cam3D.ScreenToWorldPoint (v3);
		v3.x = m_lineVertices[i+2].x; v3.y = m_lineVertices[i+2].y;
		path[2] = cam3D.ScreenToWorldPoint (v3);
		v3.x = m_lineVertices[i+1].x; v3.y = m_lineVertices[i+1].y;
		path[3] = cam3D.ScreenToWorldPoint (v3);
		collider.SetPath (pIdx++, path);
	}
	
	static int endianDiff1;
	static int endianDiff2;
	static byte[] byteBlock;
	
	public static List<Vector3> BytesToVector3List (byte[] lineBytes) {
		if (lineBytes.Length % 12 != 0) {
			Debug.LogError ("VectorLine.BytesToVector3Array: Incorrect input byte length...must be a multiple of 12");
			return null;
		}
		
		SetupByteBlock();
		var points = new List<Vector3>(lineBytes.Length/12);
		for (int i = 0; i < lineBytes.Length; i += 12) {
			points.Add (new Vector3( ConvertToFloat (lineBytes, i),
									 ConvertToFloat (lineBytes, i+4),
									 ConvertToFloat (lineBytes, i+8) ));
		}
		return points;
	}
	
	public static List<Vector2> BytesToVector2List (byte[] lineBytes) {
		if (lineBytes.Length % 8 != 0) {
			Debug.LogError ("VectorLine.BytesToVector2Array: Incorrect input byte length...must be a multiple of 8");
			return null;
		}
		
		SetupByteBlock();
		var points = new List<Vector2>(lineBytes.Length/8);
		for (int i = 0; i < lineBytes.Length; i += 8) {
			points.Add (new Vector2( ConvertToFloat (lineBytes, i),
									 ConvertToFloat (lineBytes, i+4) ));
		}
		return points;
	}
	
	private static void SetupByteBlock () {
		if (byteBlock == null) {byteBlock = new byte[4];}
		if (System.BitConverter.IsLittleEndian) {endianDiff1 = 0; endianDiff2 = 0;}
		else {endianDiff1 = 3; endianDiff2 = 1;}	
	}
	
	// Unfortunately we can't just use System.BitConverter.ToSingle as-is...we need a function to handle both big-endian and little-endian systems
	private static float ConvertToFloat (byte[] bytes, int i) {
		byteBlock[    endianDiff1] = bytes[i];
		byteBlock[1 + endianDiff2] = bytes[i+1];
		byteBlock[2 - endianDiff2] = bytes[i+2];
		byteBlock[3 - endianDiff1] = bytes[i+3];
		return System.BitConverter.ToSingle (byteBlock, 0);
	}
	
	public static void Destroy (ref VectorLine line) {
		DestroyLine (ref line);
	}

	public static void Destroy (VectorLine[] lines) {
		for (int i = 0; i < lines.Length; i++) {
			DestroyLine (ref lines[i]);
		}
	}

	public static void Destroy (List<VectorLine> lines) {
		for (int i = 0; i < lines.Count; i++) {
			var line = lines[i];
			DestroyLine (ref line);
		}
	}
	
	private static void DestroyLine (ref VectorLine line) {
		if (line != null) {
			Object.Destroy (line.m_go);
			if (line.m_vectorObject != null) {
				line.m_vectorObject.Destroy();
			}
			if (line.isAutoDrawing) {
				line.StopDrawing3DAuto();
			}
			line = null;
		}
	}
	
	public static void Destroy (ref VectorLine line, GameObject go) {
		Destroy (ref line);
		if (go != null) {
			Object.Destroy (go);
		}
	}
	
	public void SetDistances () {
		if (m_lineType == LineType.Points) return;
		if (m_distances == null || m_distances.Length != ((m_lineType != LineType.Discrete)? pointsCount : pointsCount/2 + 1)) {
			m_distances = new float[(m_lineType != LineType.Discrete)? pointsCount : pointsCount/2 + 1];
		}
		
		var totalDistance = 0.0d;
		int thisPointsLength = pointsCount-1;
		
		if (is2D) {
			if (m_lineType != LineType.Discrete) {
				for (int i = 0; i < thisPointsLength; i++) {
					Vector2 diff = m_points2[i] - m_points2[i+1];
					totalDistance += System.Math.Sqrt (diff.x*diff.x + diff.y*diff.y); // Same as Vector2.Distance, but with double instead of float
					m_distances[i+1] = (float)totalDistance;
				}
			}
			else {
				int count = 1;
				for (int i = 0; i < thisPointsLength; i += 2) {
					Vector2 diff = m_points2[i] - m_points2[i+1];
					totalDistance += System.Math.Sqrt (diff.x*diff.x + diff.y*diff.y);
					m_distances[count++] = (float)totalDistance;
				}
			}
		}
		else {
			if (m_lineType != LineType.Discrete) {
				for (int i = 0; i < thisPointsLength; i++) {
					Vector3 diff = m_points3[i] - m_points3[i+1];
					totalDistance += System.Math.Sqrt (diff.x*diff.x + diff.y*diff.y + diff.z*diff.z); // Same as Vector3.Distance, but with double instead of float
					m_distances[i+1] = (float)totalDistance;
				}
			}
			else {
				int count = 1;
				for (int i = 0; i < thisPointsLength; i += 2) {
					Vector3 diff = m_points3[i] - m_points3[i+1];
					totalDistance += System.Math.Sqrt (diff.x*diff.x + diff.y*diff.y + diff.z*diff.z);
					m_distances[count++] = (float)totalDistance;
				}
			}
		}
	}
	
	public float GetLength () {
		if (m_distances == null || m_distances.Length != ((m_lineType != LineType.Discrete)? pointsCount : pointsCount/2 + 1)) {
			SetDistances();
		}
		return m_distances[m_distances.Length-1];
	}

	public Vector2 GetPoint01 (float distance) {
		int index;
		return GetPoint (Mathf.Lerp(0.0f, GetLength(), distance), out index);
	}

	public Vector2 GetPoint01 (float distance, out int index) {
		return GetPoint (Mathf.Lerp(0.0f, GetLength(), distance), out index);
	}

	public Vector2 GetPoint (float distance) {
		int index;
		return GetPoint (distance, out index);
	}

	public Vector2 GetPoint (float distance, out int index) {
		if (!m_is2D) {
			Debug.LogError ("VectorLine.GetPoint only works with Vector2 points");
			index = 0;
			return Vector2.zero;
		}
		
		SetDistanceIndex (out index, distance);
		Vector2 point;
		if (m_lineType != LineType.Discrete) {
			point = Vector2.Lerp(m_points2[index-1], m_points2[index], Mathf.InverseLerp(m_distances[index-1], m_distances[index], distance));
		}
		else {
			point = Vector2.Lerp(m_points2[(index-1)*2], m_points2[(index-1)*2+1], Mathf.InverseLerp(m_distances[index-1], m_distances[index], distance));
		}
		if (m_drawTransform) {
			point = m_drawTransform.localToWorldMatrix.MultiplyPoint3x4 (point);
		}
		index--;
		return point;
	}

	public Vector3 GetPoint3D01 (float distance) {
		int index;
		return GetPoint3D (Mathf.Lerp(0.0f, GetLength(), distance), out index);
	}

	public Vector3 GetPoint3D01 (float distance, out int index) {
		return GetPoint3D (Mathf.Lerp(0.0f, GetLength(), distance), out index);
	}

	public Vector3 GetPoint3D (float distance) {
		int index;
		return GetPoint3D (distance, out index);
	}
	
	public Vector3 GetPoint3D (float distance, out int index) {
		if (m_is2D) {
			Debug.LogError ("VectorLine.GetPoint3D only works with Vector3 points");
			index = 0;
			return Vector3.zero;
		}
		
		SetDistanceIndex (out index, distance);
		Vector3 point;
		if (m_lineType != LineType.Discrete) {
			point = Vector3.Lerp (m_points3[index-1], m_points3[index], Mathf.InverseLerp (m_distances[index-1], m_distances[index], distance));			
		}
		else {
			point = Vector3.Lerp (m_points3[(index-1)*2], m_points3[(index-1)*2+1], Mathf.InverseLerp (m_distances[index-1], m_distances[index], distance));
		}
		if (m_drawTransform) {
			point = m_drawTransform.localToWorldMatrix.MultiplyPoint3x4 (point);
		}
		index--;
		return point;
	}
	
	void SetDistanceIndex (out int i, float distance) {
		if (m_distances == null) {
			SetDistances();
		}
		i = m_drawStart + 1;
		if (m_lineType == LineType.Discrete) {
			i = (i + 1) / 2;
		}
		if (i >= m_distances.Length) {
			i = m_distances.Length - 1;
		}
		int end = m_drawEnd;
		if (m_lineType == LineType.Discrete) {
			end = (end + 1) / 2;
		}
		while (distance > m_distances[i] && i < end) {
			i++;
		}
	}

	public static void SetEndCap (string name, EndCap capType) {
		SetEndCap (name, capType, 0.0f, 0.0f, 1.0f, 1.0f, null);
	}
	
	public static void SetEndCap (string name, EndCap capType, params Texture2D[] textures) {
		SetEndCap (name, capType, 0.0f, 0.0f, 1.0f, 1.0f, textures);
	}
	
	public static void SetEndCap (string name, EndCap capType, float offset, params Texture2D[] textures) {
		SetEndCap (name, capType, offset, offset, 1.0f, 1.0f, textures);
	}
	
	public static void SetEndCap (string name, EndCap capType, float offsetFront, float offsetBack, params Texture2D[] textures) {
		SetEndCap (name, capType, offsetFront, offsetBack, 1.0f, 1.0f, textures);
	}
	
	public static void SetEndCap (string name, EndCap capType, float offsetFront, float offsetBack, float scaleFront, float scaleBack, params Texture2D[] textures) {
		if (capDictionary == null) {
			capDictionary = new Dictionary<string, CapInfo>();
		}
		if (name == null || name == "") {
			Debug.LogError ("VectorLine.SetEndCap: must supply a name");
			return;
		}
		if (capDictionary.ContainsKey (name) && capType != EndCap.None) {
			Debug.LogError ("VectorLine.SetEndCap: end cap \"" + name + "\" has already been set up");
			return;
		}
		if (capType == EndCap.None) {
			RemoveEndCap (name);
			return;
		}
		
		if ( (capType == EndCap.Front || capType == EndCap.Back || capType == EndCap.Mirror) && textures.Length < 2) {
			Debug.LogError ("VectorLine.SetEndCap (\"" + name + "\"): must supply two textures when using SetEndCap with EndCap.Front, EndCap.Back, or EndCap.Mirror");
			return;
		}
		if (textures[0] == null || textures[1] == null) {
			Debug.LogError ("VectorLine.SetEndCap (\"" + name + "\"): end cap textures must not be null");
			return;
		}
		if (textures[0].width != textures[0].height) {
			Debug.LogError ("VectorLine.SetEndCap (\"" + name + "\"): the line texture must be square");
			return;
		}
		if (textures[1].height != textures[0].height) {
			Debug.LogError ("VectorLine.SetEndCap (\"" + name + "\"): all textures must be the same height");
			return;
		}
		if (capType == EndCap.Both) {
			if (textures.Length < 3) {
				Debug.LogError ("VectorLine.SetEndCap (\"" + name + "\"): must supply three textures when using SetEndCap with EndCap.Both");
				return;
			}
			if (textures[2] == null) {
				Debug.LogError ("VectorLine.SetEndCap (\"" + name + "\"): end cap textures must not be null");
				return;
			}
			if (textures[2].height != textures[0].height) {
				Debug.LogError ("VectorLine.SetEndCap (\"" + name + "\"): all textures must be the same height");
				return;
			}
		}
		
		var lineTex = textures[0] as Texture2D;
		var frontTex = textures[1] as Texture2D;
		var backTex = (textures.Length == 3)? textures[2] as Texture2D : null;
		int pad = 4;
		int width = lineTex.width;
		float ratio1 = 0.0f, ratio2 = 0.0f;
		int frontHeight = 0, backHeight = 0;
		Color32[] frontPixels = null, backPixels = null;
		if (capType == EndCap.Front) {
			frontPixels = GetRotatedPixels (frontTex);
			frontHeight = frontTex.width;
			backPixels = GetRowPixels (frontPixels, pad, 0, width);
			backHeight = pad;
			ratio1 = frontTex.width / (float)frontTex.height;
		}
		else if (capType == EndCap.Back) {
			backPixels = GetRotatedPixels (frontTex);	// Since there's only one texture, it's the front, even if it's used for the back
			backHeight = frontTex.width;
			frontPixels = GetRowPixels (backPixels, pad, backHeight-1, width);
			frontHeight = pad;
			ratio2 = frontTex.width / (float)frontTex.height;
		}
		else if (capType == EndCap.Both) {
			frontPixels = GetRotatedPixels (frontTex);
			frontHeight = frontTex.width;
			backPixels = GetRotatedPixels (backTex);
			backHeight = backTex.width;
			ratio1 = frontTex.width / (float)frontTex.height;
			ratio2 = backTex.width / (float)backTex.height;
		}
		else if (capType == EndCap.Mirror) {
			frontPixels = GetRotatedPixels (frontTex);
			frontHeight = frontTex.width;
			backPixels = GetRowPixels (frontPixels, pad, 0, width);
			backHeight = pad;
			ratio1 = frontTex.width / (float)frontTex.height;
			ratio2 = ratio1;
		}
		int height = lineTex.height + frontHeight + backHeight + pad*4;
		
		var linePixels = lineTex.GetPixels32();
		var clearPixels = new Color32[pad * width];
		Color32 c = Color.clear;
		for (int i = 0; i < pad * width; i++) {
			clearPixels[i] = c;
		}
		var padPixels1 = GetRowPixels (backPixels, pad, backHeight-1, width);
		var padPixels2 = GetRowPixels (frontPixels, pad, 0, width);		
		
		var useMipmaps = (lineTex.mipmapCount > 1);
		var tex = new Texture2D(width, height, TextureFormat.ARGB32, useMipmaps);
		tex.name = lineTex.name + " end cap";
		tex.wrapMode = lineTex.wrapMode;
		tex.filterMode = lineTex.filterMode;
		
		// Combine textures into one
		float uvRatio = 1.0f/height;
		var uvHeights = new float[6];
		int yPos = 0;
		tex.SetPixels32 (0, 0, width, pad, clearPixels);
		yPos += pad;
		uvHeights[0] = uvRatio * yPos;	// Line bottom
		tex.SetPixels32 (0, yPos, width, lineTex.height, linePixels);
		yPos += lineTex.height;
		uvHeights[1] = uvRatio * yPos;	// Line top
		tex.SetPixels32 (0, yPos, width, pad, clearPixels);
		yPos += pad;
		uvHeights[2] = uvRatio * yPos;	// Back bottom
		tex.SetPixels32 (0, yPos, width, backHeight, backPixels);
		yPos += backHeight;
		uvHeights[3] = uvRatio * yPos;	// Back top
		tex.SetPixels32 (0, yPos, width, pad, padPixels1);
		yPos += pad;
		tex.SetPixels32 (0, yPos, width, pad, padPixels2);
		yPos += pad;
		uvHeights[4] = uvRatio * yPos;	// Front bottom
		tex.SetPixels32 (0, yPos, width, frontHeight, frontPixels);
		uvHeights[5] = uvRatio * (yPos + frontHeight);	// Front top		
		tex.Apply (useMipmaps, true);		

		capDictionary.Add (name, new CapInfo(capType, tex, ratio1, ratio2, offsetFront, offsetBack, scaleFront, scaleBack, uvHeights));
	}
	
	private static Color32[] GetRowPixels (Color32[] texPixels, int numberOfRows, int row, int w) {
		var pixels = new Color32[w * numberOfRows];
		for (int i = 0; i < numberOfRows; i++) {
			System.Array.Copy (texPixels, row*w, pixels, i*w, w);
		}
		return pixels;
	}
	
	private static Color32[] GetRotatedPixels (Texture2D tex) {
		var pixels = tex.GetPixels32();
		var rotatedPixels = new Color32[pixels.Length];
		int w = tex.width;
		int h = tex.height;
		int x2 = 0;
		for (int y = 0; y < h; y++) {
			int y2 = tex.width-1;
			for (int x = 0; x < w; x++) {
				rotatedPixels[y2*h + x2] = pixels[y*w + x];
				y2--;
			}
			x2++;
		}
		
		return rotatedPixels;
	}
	
	public static void RemoveEndCap (string name) {
		if (!capDictionary.ContainsKey (name)) {
			Debug.LogError ("VectorLine: RemoveEndCap: \"" + name + "\" has not been set up");
			return;
		}
		Object.Destroy (capDictionary[name].texture);
		capDictionary.Remove (name);
	}
	
	public bool Selected (Vector2 p) {
		int temp;
		return Selected (p, 0, 0, out temp, cam3D);
	}

	public bool Selected (Vector2 p, out int index) {
		return Selected (p, 0, 0, out index, cam3D);
	}

	public bool Selected (Vector2 p, int extraDistance, out int index) {
		return Selected (p, extraDistance, 0, out index, cam3D);
	}

	public bool Selected (Vector2 p, int extraDistance, int extraLength, out int index) {
		return Selected (p, extraDistance, extraLength, out index, cam3D);
	}

	public bool Selected (Vector2 p, Camera cam) {
		int temp;
		return Selected (p, 0, 0, out temp, cam);
	}

	public bool Selected (Vector2 p, out int index, Camera cam) {
		return Selected (p, 0, 0, out index, cam);
	}

	public bool Selected (Vector2 p, int extraDistance, out int index, Camera cam) {
		return Selected (p, extraDistance, 0, out index, cam);
	}
	
	public bool Selected (Vector2 p, int extraDistance, int extraLength, out int index, Camera cam) {
		if (cam == null) {
			SetCamera3D();
			if (!cam3D) {
				Debug.LogError ("VectorLine.Selected: camera cannot be null. If there is no camera tagged \"MainCamera\", supply one manually");
				index = 0;
				return false;
			}
			cam = cam3D;
		}
		int wAdd = m_lineWidths.Length == 1? 0 : 1;
		int wIdx = (m_lineType != LineType.Discrete)? m_drawStart - wAdd : m_drawStart/2 - wAdd;
		if (m_lineWidths.Length == 1) {
			wAdd = 0;
			wIdx = 0;
		}
		else {
			wAdd = 1;
		}
		int end = m_drawEnd;
		var useTransformMatrix = (m_drawTransform != null);
		var thisMatrix = useTransformMatrix? m_drawTransform.localToWorldMatrix : Matrix4x4.identity;
		var scaleFactor = new Vector2(Screen.width, Screen.height);
		
		if (m_lineType == LineType.Points) {
			if (end == pointsCount) {
				end--;
			}
			Vector2 thisPoint;
			
			if (m_is2D) {
				for (int i = m_drawStart; i <= end; i++) {
					wIdx += wAdd;
					float size = m_lineWidths[wIdx] + extraDistance;
					thisPoint = useTransformMatrix? (Vector2)thisMatrix.MultiplyPoint3x4 (m_points2[i]) : m_points2[i];
					if (m_viewportDraw) {
						thisPoint.x *= scaleFactor.x;
						thisPoint.y *= scaleFactor.y;
					}
					if (p.x >= thisPoint.x - size && p.x <= thisPoint.x + size && p.y >= thisPoint.y - size && p.y <= thisPoint.y + size) {
						index = i;
						return true;
					}
				}
				index = -1;
				return false;
			}
			
			for (int i = m_drawStart; i <= end; i++) {
				wIdx += wAdd;
				float size = m_lineWidths[wIdx] + extraDistance;
				thisPoint = useTransformMatrix? cam.WorldToScreenPoint (thisMatrix.MultiplyPoint3x4 (m_points3[i])) : cam.WorldToScreenPoint (m_points3[i]);
				if (p.x >= thisPoint.x - size && p.x <= thisPoint.x + size && p.y >= thisPoint.y - size && p.y <= thisPoint.y + size) {
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}
		
		float t = 0.0f;
		int add = (m_lineType != LineType.Discrete)? 1 : 2;
		Vector2 p1, p2, d = Vector2.zero;
		if ((m_lineType != LineType.Discrete) && m_drawEnd == pointsCount) {
			end--;
		}
		
		if (m_is2D) {
			for (int i = m_drawStart; i < end; i += add) {
				wIdx += wAdd;
				if (useTransformMatrix) {
					p1 = thisMatrix.MultiplyPoint3x4 (m_points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4 (m_points2[i+1]);
				}
				else {
					p1.x = m_points2[i].x; p1.y = m_points2[i].y;
					p2.x = m_points2[i+1].x; p2.y = m_points2[i+1].y;
				}
				if (m_viewportDraw) {
					p1.x *= scaleFactor.x;
					p1.y *= scaleFactor.y;
					p2.x *= scaleFactor.x;
					p2.y *= scaleFactor.y;
				}
				
				// Extend line segment
				if (extraLength > 0) {
					d = (p1 - p2).normalized * extraLength;
					p1.x += d.x; p1.y += d.y;
					p2.x -= d.x; p2.y -= d.y;
				}
				
				// Do nothing if the point is beyond the line segment end points
				t = Vector2.Dot (p - p1, p2 - p1) / (p2 - p1).sqrMagnitude;
				if (t < 0.0f || t > 1.0f) {
					continue;
				}
				
				// If the distance of the point to the line is <= the line width
				if ((p - (p1 + t * (p2 - p1))).sqrMagnitude <= (m_lineWidths[wIdx] + extraDistance) * (m_lineWidths[wIdx] + extraDistance)) {
					index = (m_lineType != LineType.Discrete)? i : i/2;
					return true;
				}
			}
			index = -1;
			return false;
		}
		
		Vector3 screenPoint1, screenPoint2 = v3zero;
		for (int i = m_drawStart; i < end; i += add) {
			wIdx += wAdd;
			if (useTransformMatrix) {
				screenPoint1 = cam.WorldToScreenPoint (thisMatrix.MultiplyPoint3x4 (m_points3[i]));
				screenPoint2 = cam.WorldToScreenPoint (thisMatrix.MultiplyPoint3x4 (m_points3[i+1]));
			}
			else {
				screenPoint1 = cam.WorldToScreenPoint (m_points3[i]);
				screenPoint2 = cam.WorldToScreenPoint (m_points3[i+1]);
			}
			if (screenPoint1.z < 0 || screenPoint2.z < 0) {
				continue;
			}
			p1.x = (int)screenPoint1.x; p2.x = (int)screenPoint2.x;
			p1.y = (int)screenPoint1.y; p2.y = (int)screenPoint2.y;
			if (p1.x == p2.x && p1.y == p2.y) {
				continue;
			}
			
			// Extend line segment
			if (extraLength > 0) {
				d = (p1 - p2).normalized * extraLength;
				p1.x += d.x; p1.y += d.y;
				p2.x -= d.x; p2.y -= d.y;
			}
			
			// Do nothing if the point is beyond the line segment end points
			t = Vector2.Dot (p - p1, p2 - p1) / (p2 - p1).sqrMagnitude;
			if (t < 0.0f || t > 1.0f) {
				continue;
			}
			
			// If the distance of the point to the line is <= the line width
			if ((p - (p1 + t * (p2 - p1))).sqrMagnitude <= (m_lineWidths[wIdx] + extraDistance) * (m_lineWidths[wIdx] + extraDistance)) {
				index = (m_lineType != LineType.Discrete)? i : i/2;
				return true;
			}
		}
		index = -1;
		return false;
	}
	
	bool Approximately (Vector2 p1, Vector2 p2) {
		return Approximately (p1.x, p2.x) && Approximately (p1.y, p2.y);
	}

	bool Approximately (Vector3 p1, Vector3 p2) {
		return Approximately (p1.x, p2.x) && Approximately (p1.y, p2.y) && Approximately (p1.z, p2.z);
	}

	bool Approximately (float a, float b) {
		return Mathf.Round (a*100)/100 == Mathf.Round (b*100)/100;
	}
}
}