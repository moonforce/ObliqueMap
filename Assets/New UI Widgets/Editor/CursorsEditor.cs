namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Cursors editor.
	/// </summary>
	public class CursorsEditor : Editor
	{
		/// <summary>
		/// Serialized properties.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Serialized cursors.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedCursors = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Properies.
		/// </summary>
		protected List<string> Properties = new List<string>();

		/// <summary>
		/// Cursors.
		/// </summary>
		protected List<string> Cursors = new List<string>();

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Properties.Clear();
			SerializedProperties.Clear();
			SerializedCursors.Clear();

			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				AddProperty(property);
			}

			Properties.ForEach(x => SerializedProperties.Add(x, serializedObject.FindProperty(x)));
			Cursors.ForEach(x => SerializedCursors.Add(x, serializedObject.FindProperty(x)));
		}

		void AddProperty(SerializedProperty property)
		{
			if (!Cursors.Contains(property.name))
			{
				Properties.Add(property.name);
			}
		}

		/// <summary>
		/// Toggle cursors block.
		/// </summary>
		protected bool ShowCursors;

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperties.ForEach(x => EditorGUILayout.PropertyField(x.Value, true));

			EditorGUILayout.BeginVertical();

			ShowCursors = GUILayout.Toggle(ShowCursors, "Cursors", "Foldout", GUILayout.ExpandWidth(true));
			if (ShowCursors)
			{
				SerializedCursors.ForEach(x => EditorGUILayout.PropertyField(x.Value, true));
			}

			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}
	}
}