#if UIWIDGETS_TMPRO_SUPPORT && UNITY_EDITOR
namespace UIWidgets.TMProSupport
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// ProgressbarTMPro Editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ProgressbarTMPro), true)]
	public class ProgressbarTMProEditor : Editor
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
			"EmptyBarTextTMPro",
			"fullBar",
			"FullBarTextTMPro",
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
			Array.ForEach(properties, x => serializedProperties.Add(x, serializedObject.FindProperty(x)));
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
				EditorGUILayout.PropertyField(serializedProperties["EmptyBarTextTMPro"]);
				EditorGUILayout.PropertyField(serializedProperties["fullBar"]);
				EditorGUILayout.PropertyField(serializedProperties["FullBarTextTMPro"]);
				EditorGUILayout.PropertyField(serializedProperties["textType"]);
			}
			else
			{
				EditorGUILayout.PropertyField(serializedProperties["IndeterminateBar"]);
				EditorGUILayout.PropertyField(serializedProperties["UnscaledTime"]);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(serializedProperties["Direction"]);
			EditorGUILayout.PropertyField(serializedProperties["Speed"]);

			serializedObject.ApplyModifiedProperties();

			Array.ForEach(targets, x => ((Progressbar)x).Refresh());
		}
	}
}
#endif