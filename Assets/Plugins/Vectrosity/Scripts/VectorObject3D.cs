// Version 5.5
// ©2017 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Vectrosity {

public class VectorObject3D : MonoBehaviour, IVectorObject {
	
	bool m_updateVerts = true;
	bool m_updateUVs = true;
	bool m_updateColors = true;
	bool m_updateNormals = false;
	bool m_updateTangents = false;
	bool m_updateTris = true;
	Mesh m_mesh;
	VectorLine m_vectorLine;
	Material m_material;
	bool m_useCustomMaterial = false;
			
	public void SetVectorLine (VectorLine vectorLine, Texture tex, Material mat, bool useCustomMaterial) {
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshFilter>();
		m_vectorLine = vectorLine;
		m_material = mat;
		m_material.mainTexture = tex;
		GetComponent<MeshRenderer>().sharedMaterial = m_material;
		m_useCustomMaterial = useCustomMaterial;
		SetupMesh();
	}
	
	public void Destroy () {
		Destroy (m_mesh);
		if (!m_useCustomMaterial) {
			Destroy (m_material);
		}
	}
	
	public void Enable (bool enable) {
		if (this == null) return;	// Prevent random null ref error when stopping play in editor
		GetComponent<MeshRenderer>().enabled = enable;
	}
	
	public void SetTexture (Texture tex) {
		GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tex;
	}
	
	public void SetMaterial (Material mat) {
		m_material = mat;
		m_useCustomMaterial = true;
		GetComponent<MeshRenderer>().sharedMaterial = mat;
		if (mat != null) {
			GetComponent<MeshRenderer>().sharedMaterial.mainTexture = m_vectorLine.texture;
		}
	}
	
	void SetupMesh () {
		m_mesh = new Mesh();
		m_mesh.name = m_vectorLine.name;
		m_mesh.hideFlags = HideFlags.HideAndDontSave;
		GetComponent<MeshFilter>().mesh = m_mesh;
	}
	
	void LateUpdate () {
		if (m_updateVerts) {
			SetVerts();
		}
		if (m_updateUVs) {
			if (m_vectorLine.lineUVs.Length == m_mesh.vertexCount) {
				m_mesh.uv = m_vectorLine.lineUVs;
			}
			m_updateUVs = false;
		}
		if (m_updateColors) {
			if (m_vectorLine.lineColors.Length == m_mesh.vertexCount) {	// In case line points were erased and SetColor called
				m_mesh.colors32 = m_vectorLine.lineColors;
			}
			m_updateColors = false;
		}
		if (m_updateTris) {
			m_mesh.SetTriangles (m_vectorLine.lineTriangles, 0);
			m_updateTris = false;
		}
		if (m_updateNormals) {
			m_mesh.RecalculateNormals();
			m_updateNormals = false;
		}
		if (m_updateTangents) {
			m_mesh.tangents = m_vectorLine.CalculateTangents (m_mesh.normals);
			m_updateTangents = false;
		}
	}
	
	void SetVerts() {
		m_mesh.vertices = m_vectorLine.lineVertices;
		m_updateVerts = false;
		m_mesh.RecalculateBounds();
	}
	
	public void SetName (string name) {
		if (m_mesh == null) return;
		m_mesh.name = name;
	}
	
	public void UpdateVerts () {
		m_updateVerts = true;
	}
	
	public void UpdateUVs () {
		m_updateUVs = true;
	}
	
	public void UpdateColors () {
		m_updateColors = true;
	}

	public void UpdateNormals () {
		m_updateNormals = true;
	}
	
	public void UpdateTangents () {
		m_updateTangents = true;
	}
	
	public void UpdateTris () {
		m_updateTris = true;
	}
	
	public void UpdateMeshAttributes () {
		m_mesh.Clear();
		m_updateVerts = true;
		m_updateUVs = true;
		m_updateColors = true;
		m_updateTris = true;
	}
	
	public void ClearMesh () {
		if (m_mesh == null) return;
		m_mesh.Clear();
	}
	
	public int VertexCount () {
		return m_mesh.vertexCount;
	}
}

}