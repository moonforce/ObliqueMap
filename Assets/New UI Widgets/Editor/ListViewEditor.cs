namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// ListView editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ListView), true)]
	public class ListViewEditor : Editor
	{
		Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		string[] properties = new string[]
		{
			"Source",
			"strings",
			"file",
			"CommentsStartWith",
			"sort",
			"Unique",
			"AllowEmptyItems",
			"multipleSelect",
			"selectedIndex",
			"direction",
			"backgroundColor",
			"textColor",
			"HighlightedBackgroundColor",
			"HighlightedTextColor",
			"selectedBackgroundColor",
			"selectedTextColor",
			"FadeDuration",
			"Container",
			"defaultItem",
			"scrollRect",
			"OnSelectString",
			"OnDeselectString",
			"setContentSizeFitter",
			"Navigation",
			"centerTheItems",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Array.ForEach(properties, x => serializedProperties.Add(x, serializedObject.FindProperty(x)));
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedProperties["Source"]);

			EditorGUI.indentLevel++;
			if (serializedProperties["Source"].enumValueIndex == 0)
			{
				var options = new GUILayoutOption[] { };
				EditorGUILayout.PropertyField(serializedProperties["strings"], new GUIContent("Items"), true, options);
			}
			else
			{
				EditorGUILayout.PropertyField(serializedProperties["file"]);
				EditorGUILayout.PropertyField(serializedProperties["CommentsStartWith"], true);
				EditorGUILayout.PropertyField(serializedProperties["AllowEmptyItems"]);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(serializedProperties["Unique"], new GUIContent("Only unique items"));
			EditorGUILayout.PropertyField(serializedProperties["sort"]);
			EditorGUILayout.PropertyField(serializedProperties["multipleSelect"]);
			EditorGUILayout.PropertyField(serializedProperties["selectedIndex"]);
			EditorGUILayout.PropertyField(serializedProperties["direction"]);

			EditorGUILayout.PropertyField(serializedProperties["backgroundColor"]);
			EditorGUILayout.PropertyField(serializedProperties["textColor"]);
			EditorGUILayout.PropertyField(serializedProperties["HighlightedBackgroundColor"]);
			EditorGUILayout.PropertyField(serializedProperties["HighlightedTextColor"]);
			EditorGUILayout.PropertyField(serializedProperties["selectedBackgroundColor"]);
			EditorGUILayout.PropertyField(serializedProperties["selectedTextColor"]);
			EditorGUILayout.PropertyField(serializedProperties["FadeDuration"]);

			EditorGUILayout.PropertyField(serializedProperties["defaultItem"]);
			EditorGUILayout.PropertyField(serializedProperties["Container"]);
			EditorGUILayout.PropertyField(serializedProperties["scrollRect"]);

			EditorGUILayout.PropertyField(serializedProperties["OnSelectString"]);
			EditorGUILayout.PropertyField(serializedProperties["OnDeselectString"]);

			EditorGUILayout.PropertyField(serializedProperties["setContentSizeFitter"]);
			EditorGUILayout.PropertyField(serializedProperties["Navigation"]);
			EditorGUILayout.PropertyField(serializedProperties["centerTheItems"]);

			serializedObject.ApplyModifiedProperties();
		}
	}
}