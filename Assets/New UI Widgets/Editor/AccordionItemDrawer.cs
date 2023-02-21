namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// AccordionItem drawer.
	/// </summary>
	[CustomPropertyDrawer(typeof(AccordionItem))]
	public class AccordionItemDrawer : PropertyDrawer
	{
		/// <summary>
		/// Draw property.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Calculate rects
			var width = (position.width - 30) / 2;
			var openRect = new Rect(position.x, position.y, 30, position.height);
			var togglGORect = new Rect(position.x + 31, position.y, width, position.height);
			var contentGORect = new Rect(position.x + 30 + width, position.y, width, position.height);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(openRect, property.FindPropertyRelative("Open"), GUIContent.none);
			EditorGUI.LabelField(openRect, new GUIContent(string.Empty, "Is open on start?"));

			EditorGUI.PropertyField(togglGORect, property.FindPropertyRelative("ToggleObject"), GUIContent.none);
			EditorGUI.LabelField(togglGORect, new GUIContent(string.Empty, "Toggle object. Click on this object show or hide Content object."));

			EditorGUI.PropertyField(contentGORect, property.FindPropertyRelative("ContentObject"), GUIContent.none);
			EditorGUI.LabelField(contentGORect, new GUIContent(string.Empty, "Content object."));

			EditorGUI.EndProperty();
		}
	}
}