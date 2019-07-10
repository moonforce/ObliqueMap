namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Progressbar editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Progressbar), true)]
	public class ProgressbarEditor : Editor
	{
		Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		string[] properties = new string[]
		{
			"Max",
			"progressValue",
			"type",
			"Direction",
			"IndeterminateBar",
			"DeterminateBar",
			"EmptyBar",
			"EmptyBarText",
			"fullBar",
			"FullBarText",
			"BarMask",
			"textType",
			"Speed",
			"SpeedType",
			"UnscaledTime",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected void OnEnable()
		{
			Array.ForEach(properties, x =>
			{
				var p = serializedObject.FindProperty(x);
				serializedProperties.Add(x, p);
			});
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedProperties["Max"]);
			EditorGUILayout.PropertyField(serializedProperties["progressValue"]);
			EditorGUILayout.PropertyField(serializedProperties["type"]);

			EditorGUI.indentLevel++;
			if (serializedProperties["type"].enumValueIndex == 0)
			{
				EditorGUILayout.PropertyField(serializedProperties["DeterminateBar"]);
				EditorGUILayout.PropertyField(serializedProperties["BarMask"]);
				EditorGUILayout.PropertyField(serializedProperties["EmptyBar"]);
				EditorGUILayout.PropertyField(serializedProperties["EmptyBarText"]);
				EditorGUILayout.PropertyField(serializedProperties["fullBar"]);
				EditorGUILayout.PropertyField(serializedProperties["FullBarText"]);
				EditorGUILayout.PropertyField(serializedProperties["textType"]);
			}
			else
			{
				EditorGUILayout.PropertyField(serializedProperties["IndeterminateBar"]);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(serializedProperties["Direction"]);
			EditorGUILayout.PropertyField(serializedProperties["Speed"]);
			EditorGUILayout.PropertyField(serializedProperties["SpeedType"]);
			EditorGUILayout.PropertyField(serializedProperties["UnscaledTime"]);

			serializedObject.ApplyModifiedProperties();

			Array.ForEach(targets, x => ((Progressbar)x).Refresh());
		}
	}
}