#if UIWIDGETS_TMPRO_SUPPORT && UNITY_EDITOR
namespace UIWidgets.TMProSupport
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.UI;
	using UnityEngine;

	/// <summary>
	/// TabIconButtonTMPro editor.
	/// </summary>
	[CustomEditor(typeof(TabIconButtonTMPro), true)]
	[CanEditMultipleObjects]
	public class TabIconButtonTMProEditor : ButtonEditor
	{
		Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		string[] properties = new string[]
		{
			"Name",
			"Icon",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected override void OnEnable()
		{
			Array.ForEach(properties, x => serializedProperties.Add(x, serializedObject.FindProperty(x)));

			base.OnEnable();
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedProperties.ForEach(x => EditorGUILayout.PropertyField(x.Value));
			serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();
		}
	}
}
#endif