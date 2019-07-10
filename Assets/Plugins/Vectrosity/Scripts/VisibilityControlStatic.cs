// Version 5.5
// ©2017 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

namespace Vectrosity {
[AddComponentMenu("Vectrosity/VisibilityControlStatic")]
public class VisibilityControlStatic : MonoBehaviour {
	
	RefInt m_objectNumber;
	VectorLine m_vectorLine;
	bool m_destroyed = false;
	bool m_dontDestroyLine = false;
	Matrix4x4 m_originalMatrix;
	
	public RefInt objectNumber {
		get {return m_objectNumber;}
	}
	
	public void Setup (VectorLine line, bool makeBounds) {
		if (makeBounds) {
			VectorManager.SetupBoundsMesh (gameObject, line);
		}
		// Adjust points to this position, so the line doesn't have to be updated with the transform of this object
		// Also make sure the points are unique by creating a new list and copying the points over
		m_originalMatrix = transform.localToWorldMatrix;
		var thisPoints = new List<Vector3>(line.points3);
		for (int i = 0; i < thisPoints.Count; i++) {
			thisPoints[i] = m_originalMatrix.MultiplyPoint3x4 (thisPoints[i]);
		}
		line.points3 = thisPoints;
		m_vectorLine = line;
		
		VectorManager.VisibilityStaticSetup (line, out m_objectNumber);
		StartCoroutine (WaitCheck());
	}
	
	IEnumerator WaitCheck () {
		// Ensure that the line is drawn once even if the camera isn't moving
		// Otherwise this object would be invisible until the camera moves
		// However, the camera might not have been set up yet, so wait a frame and turn off if necessary
		VectorManager.DrawArrayLine (m_objectNumber.i);
		
		yield return null;
		yield return null;
		if (!GetComponent<Renderer>().isVisible) {
			m_vectorLine.active = false;
		}
	}
	
	void OnBecameVisible () {
		m_vectorLine.active = true;
		
		// Draw line now, otherwise's there's a 1-frame delay before the line is actually drawn in the next LateUpdate
		VectorManager.DrawArrayLine (m_objectNumber.i);
	}
	
	void OnBecameInvisible () {
		m_vectorLine.active = false;
	}
	
	void OnDestroy () {
		if (m_destroyed) return;	// Paranoia check
		m_destroyed = true;
		VectorManager.VisibilityStaticRemove (m_objectNumber.i);
		if (m_dontDestroyLine) return;
		VectorLine.Destroy (ref m_vectorLine);
	}
	
	public void DontDestroyLine () {
		m_dontDestroyLine = true;
	}
	
	public Matrix4x4 GetMatrix () {
		return m_originalMatrix;
	}
}
}