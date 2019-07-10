// Version 5.4.2
// ©2017 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using Vectrosity;
using System.Collections;

namespace Vectrosity {
[AddComponentMenu("Vectrosity/VisibilityControl")]
public class VisibilityControl : MonoBehaviour {
	
	RefInt m_objectNumber;
	VectorLine m_vectorLine;
	bool m_destroyed = false;
	bool m_dontDestroyLine = false;
	
	public RefInt objectNumber {
		get {return m_objectNumber;}
	}
	
	public void Setup (VectorLine line, bool makeBounds) {
		if (makeBounds) {
			VectorManager.SetupBoundsMesh (gameObject, line);
		}
		
		VectorManager.VisibilitySetup (transform, line, out m_objectNumber);
		m_vectorLine = line;
		VectorManager.DrawArrayLine2 (m_objectNumber.i);
		StartCoroutine (VisibilityTest());
	}
	
	IEnumerator VisibilityTest () {	// Since Renderer.isVisible doesn't work in Setup and needs to wait a couple frames
		yield return null;
		yield return null;
		if (!GetComponent<Renderer>().isVisible) {
			m_vectorLine.active = false;
		}
	}
	
	IEnumerator OnBecameVisible () {
		yield return new WaitForEndOfFrame();	// Since otherwise Unity 5.6 can't enable/disable renderers during OnBecameVisible
		m_vectorLine.active = true;
	}
	
	IEnumerator OnBecameInvisible () {
		yield return new WaitForEndOfFrame();
		m_vectorLine.active = false;
	}
	
	void OnDestroy () {
		if (m_destroyed) return;	// Paranoia check
		m_destroyed = true;
		VectorManager.VisibilityRemove (m_objectNumber.i);
		if (m_dontDestroyLine) return;
		VectorLine.Destroy (ref m_vectorLine);
	}
	
	public void DontDestroyLine () {
		m_dontDestroyLine = true;
	}
}
}