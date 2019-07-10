// Version 5.4
// ©2017 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using System.Collections.Generic;

namespace Vectrosity {

public partial class VectorLine {
	static string[] functionNames = {"VectorLine.SetColors: Length of color", "VectorLine.SetWidths: Length of line widths", "MakeCurve", "MakeSpline", "MakeEllipse"};
	enum FunctionName {SetColors, SetWidths, MakeCurve, MakeSpline, MakeEllipse}
	
	bool WrongArrayLength (int arrayLength, FunctionName functionName) {
		if (m_lineType == LineType.Continuous) {
			if (arrayLength != pointsCount-1) {
				Debug.LogError (functionNames[(int)functionName] + " list for \"" + name + "\" must be length of points array minus one for a continuous line (one entry per line segment). Expected " + (pointsCount-1) + ", got " + arrayLength);
				return true;
			}
		}
		else if (arrayLength != pointsCount/2) {
			Debug.LogError (functionNames[(int)functionName] + " list in \"" + name + "\" must be exactly half the length of points array for a discrete line (one entry per line segment). Expected " + (pointsCount/2) + ", got " + arrayLength);
			return true;
		}
		return false;
	}
	
	bool CheckArrayLength (FunctionName functionName, int segments, int index) {
		if (segments < 1) {
			Debug.LogError ("VectorLine." + functionNames[(int)functionName] + " needs at least 1 segment");
			return false;
		}
		if (index < 0) {
			Debug.LogError ("VectorLine." + functionNames[(int)functionName] + ": The index value for \"" + name + "\" must be >= 0");
			return false;
		}
		if (m_lineType == LineType.Points) {
			if (index + segments > pointsCount) {
				if (index == 0) {
					Debug.LogError ("VectorLine." + functionNames[(int)functionName] + ": The number of segments cannot exceed the number of points in the array for \"" + name + "\"");
					return false;
				}
				Debug.LogError ("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for \"" + name + "\"");
				return false;
			}
			return true;
		}
		
		if (m_lineType == LineType.Continuous) {
			if (index + (segments+1) > pointsCount) {
				if (index == 0) {
					Debug.LogError ("VectorLine." + functionNames[(int)functionName] + ": The length of the array for continuous lines needs to be at least the number of segments plus one for \"" + name + "\"");
					return false;
				}
				Debug.LogError ("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array (" + pointsCount + ") for \"" + name + "\"");
				return false;
			}
		}
		else {
			if (index + segments*2 > pointsCount) {
				if (index == 0) {
					Debug.LogError ("VectorLine." + functionNames[(int)functionName] + ": The length of the array for discrete lines needs to be at least twice the number of segments for \"" + name + "\"");
					return false;
				}
				Debug.LogError ("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array (" + pointsCount + ") for \"" + name + "\"");
				return false;
			}
		}
		return true;
	}

	public void MakeRect (Rect rect) {
		MakeRect (new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y+rect.height), 0);
	}

	public void MakeRect (Rect rect, int index) {
		MakeRect (new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y+rect.height), index);
	}

	public void MakeRect (Vector3 bottomLeft, Vector3 topRight) {
		MakeRect (bottomLeft, topRight, 0);
	}

	public void MakeRect (Vector3 bottomLeft, Vector3 topRight, int index) {
		if (m_lineType != LineType.Discrete) {
			if (index + 5 > pointsCount) {
				if (index == 0) {
					Debug.LogError ("VectorLine.MakeRect: The length of the array for continuous lines needs to be at least 5 for \"" + name + "\"");
					return;
				}
				Debug.LogError ("Calling VectorLine.MakeRect with an index of " + index + " would exceed the length of the Vector2 array for \"" + name + "\"");
				return;
			}
			if (m_is2D) {
				m_points2[index  ] = new Vector2(bottomLeft.x, bottomLeft.y);
				m_points2[index+1] = new Vector2(topRight.x,   bottomLeft.y);
				m_points2[index+2] = new Vector2(topRight.x,   topRight.y);
				m_points2[index+3] = new Vector2(bottomLeft.x, topRight.y);
				m_points2[index+4] = new Vector2(bottomLeft.x, bottomLeft.y);
			}
			else {
				m_points3[index  ] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
				m_points3[index+1] = new Vector3(topRight.x,   bottomLeft.y, bottomLeft.z);
				m_points3[index+2] = new Vector3(topRight.x,   topRight.y,   topRight.z);
				m_points3[index+3] = new Vector3(bottomLeft.x, topRight.y,   topRight.z);
				m_points3[index+4] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
			}
		}
		else {
			if (index + 8 > pointsCount) {
				if (index == 0) {
					Debug.LogError ("VectorLine.MakeRect: The length of the array for discrete lines needs to be at least 8 for \"" + name + "\"");
					return;
				}
				Debug.LogError ("Calling VectorLine.MakeRect with an index of " + index + " would exceed the length of the Vector2 array for \"" + name + "\"");
				return;
			}
			if (m_is2D) {
				m_points2[index  ] = new Vector2(bottomLeft.x, bottomLeft.y);
				m_points2[index+1] = new Vector2(topRight.x,   bottomLeft.y);
				m_points2[index+2] = new Vector2(topRight.x,   bottomLeft.y);
				m_points2[index+3] = new Vector2(topRight.x,   topRight.y);
				m_points2[index+4] = new Vector2(topRight.x,   topRight.y);
				m_points2[index+5] = new Vector2(bottomLeft.x, topRight.y);
				m_points2[index+6] = new Vector2(bottomLeft.x, topRight.y);
				m_points2[index+7] = new Vector2(bottomLeft.x, bottomLeft.y);
			}
			else {
				m_points3[index  ] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
				m_points3[index+1] = new Vector3(topRight.x,   bottomLeft.y, bottomLeft.z);
				m_points3[index+2] = new Vector3(topRight.x,   bottomLeft.y, bottomLeft.z);
				m_points3[index+3] = new Vector3(topRight.x,   topRight.y,   topRight.z);
				m_points3[index+4] = new Vector3(topRight.x,   topRight.y,   topRight.z);
				m_points3[index+5] = new Vector3(bottomLeft.x, topRight.y,   topRight.z);
				m_points3[index+6] = new Vector3(bottomLeft.x, topRight.y,   topRight.z);
				m_points3[index+7] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
			}
		}
	}
	
	public void MakeRoundedRect (Rect rect, float cornerRadius, int cornerSegments) {
		MakeRoundedRect (new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y+rect.height), cornerRadius, cornerSegments, 0);
	}

	public void MakeRoundedRect (Rect rect, float cornerRadius, int cornerSegments, int index) {
		MakeRoundedRect (new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y+rect.height), cornerRadius, cornerSegments, index);
	}

	public void MakeRoundedRect (Vector3 bottomLeft, Vector3 topRight, float cornerRadius, int cornerSegments) {
		MakeRoundedRect (bottomLeft, topRight, cornerRadius, cornerSegments, 0);
	}
	
	public void MakeRoundedRect (Vector3 bottomLeft, Vector3 topRight, float cornerRadius, int cornerSegments, int index) {
		if (cornerSegments < 1) {
			Debug.LogError ("VectorLine.MakeRoundedRect: cornerSegments value must be >= 1");
			return;
		}
		if (index < 0) {
			Debug.LogError ("VectorLine.MakeRoundedRect: index value must be >= 0");
			return;
		}
		if (!m_is2D && bottomLeft.z != topRight.z) {
			Debug.LogError ("VectorLine.MakeRoundedRect only works on the X/Y plane");
			return;
		}
		int neededCount = (m_lineType != LineType.Discrete)? cornerSegments * 4 + 5 + index : cornerSegments * 8 + 8 + index;
		if (pointsCount < neededCount) {
			Resize (neededCount);
		}
		
		if (bottomLeft.x > topRight.x) {
			Exchange (ref bottomLeft, ref topRight, 0);
		}
		if (bottomLeft.y > topRight.y) {
			Exchange (ref bottomLeft, ref topRight, 1);
		}
		bottomLeft += new Vector3(cornerRadius, cornerRadius);
		topRight -= new Vector3(cornerRadius, cornerRadius);
		MakeCircle (bottomLeft, cornerRadius, 4 * cornerSegments, index);
		
		int cornerPointCount = (m_lineType != LineType.Discrete)? cornerSegments + 1 : cornerSegments * 2;
		int originalCount = (m_lineType != LineType.Discrete)? cornerSegments : cornerSegments * 2;
		if (m_is2D) {
			CopyAndAddPoints (cornerPointCount, originalCount, 3, new Vector2(0, topRight.y - bottomLeft.y), index);
			CopyAndAddPoints (cornerPointCount, originalCount, 2, Vector2.zero, index);
			CopyAndAddPoints (cornerPointCount, originalCount, 1, new Vector2(topRight.x - bottomLeft.x, 0), index);
			CopyAndAddPoints (cornerPointCount, originalCount, 0, new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y), index);
			if (m_lineType != LineType.Discrete) {
				m_points2[cornerPointCount*4 + index] = m_points2[index];
			}
			else {
				m_points2[cornerPointCount*4 + 7 + index] = m_points2[index];
				m_points2[cornerPointCount*3 + 5 + index] = m_points2[cornerPointCount*3 + 6 + index];
				m_points2[cornerPointCount*2 + 3 + index] = m_points2[cornerPointCount*2 + 4 + index];
				m_points2[cornerPointCount + 1 + index] = m_points2[cornerPointCount + 2 + index];				
			}
		}
		else {
			CopyAndAddPoints (cornerPointCount, originalCount, 3, Vector2.zero, index);
			CopyAndAddPoints (cornerPointCount, originalCount, 2, new Vector2(0, topRight.y - bottomLeft.y), index);
			CopyAndAddPoints (cornerPointCount, originalCount, 1, new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y), index);
			CopyAndAddPoints (cornerPointCount, originalCount, 0, new Vector2(topRight.x - bottomLeft.x, 0), index);
			if (m_lineType != LineType.Discrete) {
				m_points3[cornerPointCount*4 + index] = m_points3[index];
			}
			else {
				m_points3[cornerPointCount*4 + 7 + index] = m_points3[index];
				m_points3[cornerPointCount*3 + 5 + index] = m_points3[cornerPointCount*3 + 6 + index];
				m_points3[cornerPointCount*2 + 3 + index] = m_points3[cornerPointCount*2 + 4 + index];
				m_points3[cornerPointCount + 1 + index] = m_points3[cornerPointCount + 2 + index];
			}
		}
	}
	
	private void CopyAndAddPoints (int cornerPointCount, int originalCount, int sectionNumber, Vector2 add, int index) {
		Vector3 add3 = add;
		for (int i = cornerPointCount-1; i >= 0; i--) {
			if (m_lineType != LineType.Discrete) {
				if (m_is2D) {
					m_points2[cornerPointCount*sectionNumber + i + index] = m_points2[originalCount*sectionNumber + i + index] + add;
				}
				else {
					m_points3[cornerPointCount*sectionNumber + i + index] = m_points3[originalCount*sectionNumber + i + index] + add3;
				}
			}
			else {
				if (m_is2D) {
					m_points2[cornerPointCount*sectionNumber + sectionNumber*2 + i + index] = m_points2[originalCount*sectionNumber + i + index] + add;
				}
				else {
					m_points3[cornerPointCount*sectionNumber + sectionNumber*2 + i + index] = m_points3[originalCount*sectionNumber + i + index] + add3;
				}
			}
		}
		if (m_lineType == LineType.Discrete) {
			int i = cornerPointCount*(sectionNumber+1) + sectionNumber*2 + index;
			if (m_is2D) {
				m_points2[i] = m_points2[i-1];
			}
			else {
				m_points3[i] = m_points3[i-1];
			}
		}
	}
	
	private void Exchange (ref Vector3 v1, ref Vector3 v2, int i) {
		var temp = v1[i];
		v1[i] = v2[i];
		v2[i] = temp;
	}

	public void MakeCircle (Vector3 origin, float radius) {
		MakeEllipse (origin, Vector3.forward, radius, radius, 0.0f, 0.0f, GetSegmentNumber(), 0.0f, 0);
	}
	
	public void MakeCircle (Vector3 origin, float radius, int segments) {
		MakeEllipse (origin, Vector3.forward, radius, radius, 0.0f, 0.0f, segments, 0.0f, 0);
	}

	public void MakeCircle (Vector3 origin, float radius, int segments, float pointRotation) {
		MakeEllipse (origin, Vector3.forward, radius, radius, 0.0f, 0.0f, segments, pointRotation, 0);
	}

	public void MakeCircle (Vector3 origin, float radius, int segments, int index) {
		MakeEllipse (origin, Vector3.forward, radius, radius, 0.0f, 0.0f, segments, 0.0f, index);
	}

	public void MakeCircle (Vector3 origin, float radius, int segments, float pointRotation, int index) {
		MakeEllipse (origin, Vector3.forward, radius, radius, 0.0f, 0.0f, segments, pointRotation, index);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius) {
		MakeEllipse (origin, upVector, radius, radius, 0.0f, 0.0f, GetSegmentNumber(), 0.0f, 0);
	}
	
	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments) {
		MakeEllipse (origin, upVector, radius, radius, 0.0f, 0.0f, segments, 0.0f, 0);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation) {
		MakeEllipse (origin, upVector, radius, radius, 0.0f, 0.0f, segments, pointRotation, 0);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments, int index) {
		MakeEllipse (origin, upVector, radius, radius, 0.0f, 0.0f, segments, 0.0f, index);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation, int index) {
		MakeEllipse (origin, upVector, radius, radius, 0.0f, 0.0f, segments, pointRotation, index);
	}

	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, 0.0f, 0.0f, GetSegmentNumber(), 0.0f, 0);
	}
	
	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius, int segments) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, 0.0f, 0.0f, segments, 0.0f, 0);
	}
	
	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius, int segments, int index) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, 0.0f, 0.0f, segments, 0.0f, index);
	}

	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius, int segments, float pointRotation) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, 0.0f, 0.0f, segments, pointRotation, 0);
	}

	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius) {
		MakeEllipse (origin, upVector, xRadius, yRadius, 0.0f, 0.0f, GetSegmentNumber(), 0.0f, 0);
	}

	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments) {
		MakeEllipse (origin, upVector, xRadius, yRadius, 0.0f, 0.0f, segments, 0.0f, 0);
	}
	
	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, int index) {
		MakeEllipse (origin, upVector, xRadius, yRadius, 0.0f, 0.0f, segments, 0.0f, index);
	}

	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation) {
		MakeEllipse (origin, upVector, xRadius, yRadius, 0.0f, 0.0f, segments, pointRotation, 0);
	}

	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation, int index) {
		MakeEllipse (origin, upVector, xRadius, yRadius, 0.0f, 0.0f, segments, pointRotation, index);
	}

	public void MakeArc (Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, GetSegmentNumber(), 0.0f, 0);
	}
	
	public void MakeArc (Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, segments, 0.0f, 0);
	}
	
	public void MakeArc (Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, int index) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, segments, 0.0f, index);
	}

	public void MakeArc (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees) {
		MakeEllipse (origin, upVector, xRadius, yRadius, startDegrees, endDegrees, GetSegmentNumber(), 0.0f, 0);
	}

	public void MakeArc (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments) {
		MakeEllipse (origin, upVector, xRadius, yRadius, startDegrees, endDegrees, segments, 0.0f, 0);
	}

	public void MakeArc (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, int index) {
		MakeEllipse (origin, upVector, xRadius, yRadius, startDegrees, endDegrees, segments, 0.0f, index);
	}
	
	private void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, float pointRotation, int index) {
		if (segments < 3) {
			Debug.LogError ("VectorLine.MakeEllipse needs at least 3 segments");
			return;
		}
		if (!CheckArrayLength (FunctionName.MakeEllipse, segments, index)) {
			return;
		}
		
		float totalDegrees, p;
		startDegrees = Mathf.Repeat (startDegrees, 360.0f);
		endDegrees = Mathf.Repeat (endDegrees, 360.0f);
		if (startDegrees == endDegrees) {
			totalDegrees = 360.0f;
			p = -pointRotation * Mathf.Deg2Rad;
		}
		else {
			totalDegrees = (endDegrees > startDegrees)? endDegrees - startDegrees : (360.0f - startDegrees) + endDegrees;
			p = startDegrees * Mathf.Deg2Rad;
		}
		float radians = (totalDegrees / segments) * Mathf.Deg2Rad;
		
		if (m_lineType != LineType.Discrete) {
			if (startDegrees != endDegrees) {
				segments++;
			}
			int i = 0;
			if (m_is2D) {
				Vector2 v2Origin = origin;
				for (i = 0; i < segments; i++) {
					m_points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Sin(p)*xRadius, .5f + Mathf.Cos(p)*yRadius);
					p += radians;
				}
				if (m_lineType != LineType.Points && startDegrees == endDegrees) {	// Copy point when making an ellipse so the shape is closed
					m_points2[index+i] = m_points2[index+(i-segments)];
				}
			}
			else {
				var thisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
				for (i = 0; i < segments; i++) {
					m_points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Sin(p)*xRadius, Mathf.Cos(p)*yRadius, 0.0f));
					p += radians;
				}
				if (m_lineType != LineType.Points && startDegrees == endDegrees) {	// Copy point when making an ellipse so the shape is closed
					m_points3[index+i] = m_points3[index+(i-segments)];
				}
			}
		}
		// Discrete
		else {
			if (m_is2D) {
				Vector2 v2Origin = origin;
				for (int i = 0; i < segments*2; i++) {
					m_points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Sin(p)*xRadius, .5f + Mathf.Cos(p)*yRadius);
					p += radians;
					i++;
					m_points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Sin(p)*xRadius, .5f + Mathf.Cos(p)*yRadius);
				}
			}
			else {
				var thisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
				for (int i = 0; i < segments*2; i++) {
					m_points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Sin(p)*xRadius, Mathf.Cos(p)*yRadius, 0.0f));
					p += radians;
					i++;
					m_points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Sin(p)*xRadius, Mathf.Cos(p)*yRadius, 0.0f));
				}
			}
		}
	}

	public void MakeCurve (Vector2[] curvePoints) {
		MakeCurve (curvePoints, GetSegmentNumber(), 0);
	}
	
	public void MakeCurve (Vector2[] curvePoints, int segments) {
		MakeCurve (curvePoints, segments, 0);
	}

	public void MakeCurve (Vector2[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			Debug.LogError ("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurve (curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}

	public void MakeCurve (Vector3[] curvePoints) {
		MakeCurve (curvePoints, GetSegmentNumber(), 0);
	}
	
	public void MakeCurve (Vector3[] curvePoints, int segments) {
		MakeCurve (curvePoints, segments, 0);
	}
	
	public void MakeCurve (Vector3[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			Debug.LogError ("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurve (curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}

	public void MakeCurve (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2) {
		MakeCurve (anchor1, control1, anchor2, control2, GetSegmentNumber(), 0);
	}
	
	public void MakeCurve (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments) {
		MakeCurve (anchor1, control1, anchor2, control2, segments, 0);
	}
	
	public void MakeCurve (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments, int index) {
		if (!CheckArrayLength (FunctionName.MakeCurve, segments, index)) {
			return;
		}
		
		if (m_lineType != LineType.Discrete) {
			int end = (m_lineType == LineType.Points)? segments : segments+1;
			if (m_is2D) {
				Vector2 anchor1a = anchor1; Vector2 anchor2a = anchor2;
				Vector2 control1a = control1; Vector2 control2a = control2;
				for (int i = 0; i < end; i++) {
					m_points2[index+i] = GetBezierPoint (ref anchor1a, ref control1a, ref anchor2a, ref control2a, (float)i/segments);
				}
			}
			else {
				for (int i = 0; i < end; i++) {
					m_points3[index+i] = GetBezierPoint3D (ref anchor1, ref control1, ref anchor2, ref control2, (float)i/segments);
				}
			}
		}
		
		else {
			int idx = 0;
			if (m_is2D) {
				Vector2 anchor1a = anchor1; Vector2 anchor2a = anchor2;
				Vector2 control1a = control1; Vector2 control2a = control2;
				for (int i = 0; i < segments; i++) {
					m_points2[index + idx++] = GetBezierPoint (ref anchor1a, ref control1a, ref anchor2a, ref control2a, (float)i/segments);
					m_points2[index + idx++] = GetBezierPoint (ref anchor1a, ref control1a, ref anchor2a, ref control2a, (float)(i+1)/segments);
				}
			}
			else {
				for (int i = 0; i < segments; i++) {
					m_points3[index + idx++] = GetBezierPoint3D (ref anchor1, ref control1, ref anchor2, ref control2, (float)i/segments);
					m_points3[index + idx++] = GetBezierPoint3D (ref anchor1, ref control1, ref anchor2, ref control2, (float)(i+1)/segments);
				}
			}
		}
	}
	
	private static Vector2 GetBezierPoint (ref Vector2 anchor1, ref Vector2 control1, ref Vector2 anchor2, ref Vector2 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		
		return new Vector2( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
						    (ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y );
	}

	private static Vector3 GetBezierPoint3D (ref Vector3 anchor1, ref Vector3 control1, ref Vector3 anchor2, ref Vector3 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		float cz = 3 * (control1.z - anchor1.z);
		float bz = 3 * (control2.z - control1.z) - cz;
		float az = anchor2.z - anchor1.z - cz - bz;
		
		return new Vector3( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
							(ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y,
							(az * (t*t*t)) + (bz * (t*t)) + (cz * t) + anchor1.z );
	}

	public void MakeSpline (Vector2[] splinePoints) {
		MakeSpline (splinePoints, null, GetSegmentNumber(), 0, false);
	}

	public void MakeSpline (Vector2[] splinePoints, bool loop) {
		MakeSpline (splinePoints, null, GetSegmentNumber(), 0, loop);
	}
	
	public void MakeSpline (Vector2[] splinePoints, int segments) {
		MakeSpline (splinePoints, null, segments, 0, false);
	}

	public void MakeSpline (Vector2[] splinePoints, int segments, bool loop) {
		MakeSpline (splinePoints, null, segments, 0, loop);
	}

	public void MakeSpline (Vector2[] splinePoints, int segments, int index) {
		MakeSpline (splinePoints, null, segments, index, false);
	}

	public void MakeSpline (Vector2[] splinePoints, int segments, int index, bool loop) {
		MakeSpline (splinePoints, null, segments, index, loop);
	}

	public void MakeSpline (Vector3[] splinePoints) {
		MakeSpline (null, splinePoints, GetSegmentNumber(), 0, false);
	}

	public void MakeSpline (Vector3[] splinePoints, bool loop) {
		MakeSpline (null, splinePoints, GetSegmentNumber(), 0, loop);
	}
	
	public void MakeSpline (Vector3[] splinePoints, int segments) {
		MakeSpline (null, splinePoints, segments, 0, false);
	}

	public void MakeSpline (Vector3[] splinePoints, int segments, bool loop) {
		MakeSpline (null, splinePoints, segments, 0, loop);
	}

	public void MakeSpline (Vector3[] splinePoints, int segments, int index) {
		MakeSpline (null, splinePoints, segments, index, false);
	}

	public void MakeSpline (Vector3[] splinePoints, int segments, int index, bool loop) {
		MakeSpline (null, splinePoints, segments, index, loop);
	}
		
	private void MakeSpline (Vector2[] splinePoints2, Vector3[] splinePoints3, int segments, int index, bool loop) {
		int pointsLength = (splinePoints2 != null)? splinePoints2.Length : splinePoints3.Length;		
		if (pointsLength < 2) {
			Debug.LogError ("VectorLine.MakeSpline needs at least 2 spline points");
			return;
		}
		if (splinePoints2 != null && !m_is2D) {
			Debug.LogError ("VectorLine.MakeSpline was called with a Vector2 spline points array, but the line uses Vector3 points");
			return;
		}
		if (splinePoints3 != null && m_is2D) {
			Debug.LogError ("VectorLine.MakeSpline was called with a Vector3 spline points array, but the line uses Vector2 points");
			return;
		}
		if (!CheckArrayLength (FunctionName.MakeSpline, segments, index)) {
			return;
		}

		var pointCount = index;
		var numberOfPoints = loop? pointsLength : pointsLength-1;
		var add = 1.0f / segments * numberOfPoints;
		float i, start = 0.0f;
		int j, p0 = 0, p2 = 0, p3 = 0;
		
		for (j = 0; j < numberOfPoints; j++) {
			p0 = j-1;
			p2 = j+1;
			p3 = j+2;
			if (p0 < 0) {
				p0 = loop? numberOfPoints-1 : 0;
			}
			if (loop && p2 > numberOfPoints-1) {
				p2 -= numberOfPoints;
			}
			if (p3 > numberOfPoints-1) {
				p3 = loop? p3-numberOfPoints : numberOfPoints;
			}
			if (m_lineType != LineType.Discrete) {
				if (m_is2D) {
					for (i = start; i <= 1.0f; i += add) {
						m_points2[pointCount++] = GetSplinePoint (ref splinePoints2[p0], ref splinePoints2[j], ref splinePoints2[p2], ref splinePoints2[p3], i);
					}
				}
				else {
					for (i = start; i <= 1.0f; i += add) {
						m_points3[pointCount++] = GetSplinePoint3D (ref splinePoints3[p0], ref splinePoints3[j], ref splinePoints3[p2], ref splinePoints3[p3], i);
					}
				}
			}
			else {
				if (m_is2D) {
					for (i = start; i <= 1.0f; i += add) {
						m_points2[pointCount++] = GetSplinePoint (ref splinePoints2[p0], ref splinePoints2[j], ref splinePoints2[p2], ref splinePoints2[p3], i);
						if (pointCount > index+1 && pointCount < index + (segments*2)) {
							m_points2[pointCount++] = m_points2[pointCount-2];
						}
					}
				}
				else {
					for (i = start; i <= 1.0f; i += add) {
						m_points3[pointCount++] = GetSplinePoint3D (ref splinePoints3[p0], ref splinePoints3[j], ref splinePoints3[p2], ref splinePoints3[p3], i);
						if (pointCount > index+1 && pointCount < index + (segments*2)) {
							m_points3[pointCount++] = m_points3[pointCount-2];
						}
					}
				}
			}
			start = i - 1.0f;
		}
		// The last point might not get done depending on number of splinePoints and segments, so ensure that it's done here
		if ( (m_lineType != LineType.Discrete && pointCount < index + (segments+1)) || (m_lineType == LineType.Discrete && pointCount < index + (segments*2)) ) {
			if (m_is2D) {
				m_points2[pointCount] = GetSplinePoint (ref splinePoints2[p0], ref splinePoints2[j-1], ref splinePoints2[p2], ref splinePoints2[p3], 1.0f);
			}
			else {
				m_points3[pointCount] = GetSplinePoint3D (ref splinePoints3[p0], ref splinePoints3[j-1], ref splinePoints3[p2], ref splinePoints3[p3], 1.0f);
			}
		}
	}

	private static Vector2 GetSplinePoint (ref Vector2 p0, ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, float t) {
		var px = Vector4.zero;
		var py = Vector4.zero;
		float dt0 = Mathf.Pow (VectorDistanceSquared (ref p0, ref p1), 0.25f);
		float dt1 = Mathf.Pow (VectorDistanceSquared (ref p1, ref p2), 0.25f);
		float dt2 = Mathf.Pow (VectorDistanceSquared (ref p2, ref p3), 0.25f);
		
		if (dt1 < 0.0001f) dt1 = 1.0f;
		if (dt0 < 0.0001f) dt0 = dt1;
		if (dt2 < 0.0001f) dt2 = dt1;
		
		InitNonuniformCatmullRom (p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2, ref px);
		InitNonuniformCatmullRom (p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2, ref py);
		
		return new Vector2(EvalCubicPoly (ref px, t), EvalCubicPoly (ref py, t));
	}

	private static Vector3 GetSplinePoint3D (ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, float t) {
		var px = Vector4.zero;
		var py = Vector4.zero;
		var pz = Vector4.zero;
		float dt0 = Mathf.Pow (VectorDistanceSquared (ref p0, ref p1), 0.25f);
		float dt1 = Mathf.Pow (VectorDistanceSquared (ref p1, ref p2), 0.25f);
		float dt2 = Mathf.Pow (VectorDistanceSquared (ref p2, ref p3), 0.25f);
		
		if (dt1 < 0.0001f) dt1 = 1.0f;
		if (dt0 < 0.0001f) dt0 = dt1;
		if (dt2 < 0.0001f) dt2 = dt1;
		
		InitNonuniformCatmullRom (p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2, ref px);
		InitNonuniformCatmullRom (p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2, ref py);
		InitNonuniformCatmullRom (p0.z, p1.z, p2.z, p3.z, dt0, dt1, dt2, ref pz);
		
		return new Vector3(EvalCubicPoly (ref px, t), EvalCubicPoly (ref py, t), EvalCubicPoly (ref pz, t));
	}
	
	private static float VectorDistanceSquared (ref Vector2 p, ref Vector2 q) {
		float dx = q.x - p.x;
		float dy = q.y - p.y;
		return dx*dx + dy*dy;
	}

	private static float VectorDistanceSquared (ref Vector3 p, ref Vector3 q) {
		float dx = q.x - p.x;
		float dy = q.y - p.y;
		float dz = q.z - p.z;
		return dx*dx + dy*dy + dz*dz;
	}
	
	private static void InitNonuniformCatmullRom (float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2, ref Vector4 p) {
		float t1 = ((x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1) * dt1;
		float t2 = ((x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2) * dt1;
		
		// Initialize cubic poly
		p.x = x1;
		p.y = t1;
		p.z = -3*x1 + 3*x2 - 2*t1 - t2;
		p.w = 2*x1 - 2*x2 + t1 + t2;
	}
	
	private static float EvalCubicPoly (ref Vector4 p, float t) {
		return p.x + p.y*t + p.z*(t*t) + p.w*(t*t*t);
	}
	
	public void MakeText (string text, Vector3 startPos, float size) {
		MakeText (text, startPos, size, 1.0f, 1.5f, true);
	}
	
	public void MakeText (string text, Vector3 startPos, float size, bool uppercaseOnly) {
		MakeText (text, startPos, size, 1.0f, 1.5f, uppercaseOnly);
	}
	
	public void MakeText (string text, Vector3 startPos, float size, float charSpacing, float lineSpacing) {
		MakeText (text, startPos, size, charSpacing, lineSpacing, true);
	}
	
	public void MakeText (string text, Vector3 startPos, float size, float charSpacing, float lineSpacing, bool uppercaseOnly) {
		if (m_lineType != LineType.Discrete) {
			Debug.LogError ("VectorLine.MakeText only works with a discrete line");
			return;
		}
		int charPointsLength = 0;
		
		// Get total number of points needed for all characters in the string
		for (int i = 0; i < text.Length; i++) {
			int charNum = System.Convert.ToInt32(text[i]);
			if (charNum < 0 || charNum > VectorChar.numberOfCharacters) {
				Debug.LogError ("VectorLine.MakeText: Character '" + text[i] + "' is not valid");
				return;
			}
			if (uppercaseOnly && charNum >= 97 && charNum <= 122) {
				charNum -= 32;
			}
			if (VectorChar.data[charNum] != null) {
				charPointsLength += VectorChar.data[charNum].Length;
			}
		}
		if (charPointsLength != pointsCount) {
			Resize (charPointsLength);
		}
		
		float charPos = 0.0f, linePos = 0.0f;
		int idx = 0;
		var scaleVector = new Vector2(size, size);

		for (int i = 0; i < text.Length; i++) {
			int charNum = System.Convert.ToInt32(text[i]);
			// Newline
			if (charNum == 10) {
				linePos -= lineSpacing;
				charPos = 0.0f;
			}
			// Space
			else if (charNum == 32) {
				charPos += charSpacing;
			}
			// Character
			else {
				if (uppercaseOnly && charNum >= 97 && charNum <= 122) {
					charNum -= 32;
				}
				int end = 0;
				if (VectorChar.data[charNum] != null) {
					end = VectorChar.data[charNum].Length;
				}
				else {
					charPos += charSpacing;
					continue;
				}
				if (m_is2D) {
					for (int j = 0; j < end; j++) {
						m_points2[idx++] = Vector2.Scale(VectorChar.data[charNum][j] + new Vector2(charPos, linePos), scaleVector) + (Vector2)startPos;
					}
				}
				else {
					for (int j = 0; j < end; j++) {
						m_points3[idx++] = Vector3.Scale((Vector3)VectorChar.data[charNum][j] + new Vector3(charPos, linePos, 0.0f), scaleVector) + startPos;
					}
				}
				charPos += charSpacing;
			}
		}
	}
	
	public void MakeWireframe (Mesh mesh) {
		if (m_lineType != LineType.Discrete) {
			Debug.LogError ("VectorLine.MakeWireframe only works with a discrete line");
			return;
		}
		if (m_is2D) {
			Debug.LogError ("VectorLine.MakeWireframe can only be used with Vector3 points, which \"" + name + "\" doesn't have");
			return;
		}
		if (mesh == null) {
			Debug.LogError ("VectorLine.MakeWireframe can't use a null mesh");
			return;
		}
		
		var meshVertices = mesh.vertices;
		var pairs = new Dictionary<Vector3Pair, bool>();
		var linePoints = new List<Vector3>();
		
		for (int i = 0; i < mesh.subMeshCount; i++) {
			var meshIndices = mesh.GetIndices (i);
			var polyCount = (mesh.GetTopology (i) == MeshTopology.Triangles)? 3 : 4;
			
			for (int j = 0; j < meshIndices.Length; j += polyCount) {
				for (int k = 0; k < polyCount; k++) {
					CheckPairPoints (pairs, meshVertices[meshIndices[j + k]], meshVertices[meshIndices[j + (k+1)%polyCount]], linePoints);
				}
			}
		}
		
		if (linePoints.Count != m_pointsCount) {
			Resize (linePoints.Count);
		}
		for (int i = 0; i < m_pointsCount; i++) {
			m_points3[i] = linePoints[i];
		}
	}

	private static void CheckPairPoints (Dictionary<Vector3Pair, bool> pairs, Vector3 p1, Vector3 p2, List<Vector3> linePoints) {
		var pair1 = new Vector3Pair(p1, p2);
		var pair2 = new Vector3Pair(p2, p1);
		if (!pairs.ContainsKey(pair1) && !pairs.ContainsKey(pair2)) {
			pairs[pair1] = true;
			pairs[pair2] = true;
			linePoints.Add(p1);
			linePoints.Add(p2);
		}
	}
	
	public void MakeCube (Vector3 position, float xSize, float ySize, float zSize) {
		MakeCube (position, xSize, ySize, zSize, 0);
	}
	
	public void MakeCube (Vector3 position, float xSize, float ySize, float zSize, int index) {
		if (m_lineType != LineType.Discrete) {
			Debug.LogError ("VectorLine.MakeCube only works with a discrete line");
			return;
		}
		if (m_is2D) {
			Debug.LogError ("VectorLine.MakeCube can only be used with Vector3 points, which \"" + name + "\" doesn't have");
			return;
		}
		if (index + 24 > pointsCount) {
			if (index == 0) {
				Debug.LogError ("VectorLine.MakeCube: The number of Vector3 points needs to be at least 24 for \"" + name + "\"");
				return;
			}
			Debug.LogError ("Calling VectorLine.MakeCube with an index of " + index + " would exceed the length of the Vector3 points for \"" + name + "\"");
			return;
		}
		
		xSize /= 2;
		ySize /= 2;
		zSize /= 2;
		// Top
		m_points3[index   ] = position + new Vector3(-xSize, ySize, -zSize);
		m_points3[index+1 ] = position + new Vector3(xSize, ySize, -zSize);
		m_points3[index+2 ] = position + new Vector3(xSize, ySize, -zSize);
		m_points3[index+3 ] = position + new Vector3(xSize, ySize, zSize);
		m_points3[index+4 ] = position + new Vector3(xSize, ySize, zSize);
		m_points3[index+5 ] = position + new Vector3(-xSize, ySize, zSize);
		m_points3[index+6 ] = position + new Vector3(-xSize, ySize, zSize);
		m_points3[index+7 ] = position + new Vector3(-xSize, ySize, -zSize);
		// Middle
		m_points3[index+8 ] = position + new Vector3(-xSize, -ySize, -zSize);
		m_points3[index+9 ] = position + new Vector3(-xSize, ySize, -zSize);
		m_points3[index+10] = position + new Vector3(xSize, -ySize, -zSize);
		m_points3[index+11] = position + new Vector3(xSize, ySize, -zSize);
		m_points3[index+12] = position + new Vector3(-xSize, -ySize, zSize);
		m_points3[index+13] = position + new Vector3(-xSize, ySize, zSize);
		m_points3[index+14] = position + new Vector3(xSize, -ySize, zSize);
		m_points3[index+15] = position + new Vector3(xSize, ySize, zSize);
		// Bottom
		m_points3[index+16] = position + new Vector3(-xSize, -ySize, -zSize);
		m_points3[index+17] = position + new Vector3(xSize, -ySize, -zSize);
		m_points3[index+18] = position + new Vector3(xSize, -ySize, -zSize);
		m_points3[index+19] = position + new Vector3(xSize, -ySize, zSize);
		m_points3[index+20] = position + new Vector3(xSize, -ySize, zSize);
		m_points3[index+21] = position + new Vector3(-xSize, -ySize, zSize);
		m_points3[index+22] = position + new Vector3(-xSize, -ySize, zSize);
		m_points3[index+23] = position + new Vector3(-xSize, -ySize, -zSize);
	}
}

public struct Vector3Pair {
	public Vector3 p1;
	public Vector3 p2;
	public Vector3Pair (Vector3 point1, Vector3 point2) {
		p1 = point1;
		p2 = point2;
	}
}

}