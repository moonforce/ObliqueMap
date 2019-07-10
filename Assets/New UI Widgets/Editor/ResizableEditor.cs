namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Resizable editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Resizable), true)]
	public class ResizableEditor : CursorsEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResizableEditor"/> class.
		/// </summary>
		public ResizableEditor()
		{
			Cursors = new List<string>()
			{
				"CurrentCamera",
				"CursorEWTexture",
				"CursorEWHotSpot",
				"CursorNSTexture",
				"CursorNSHotSpot",
				"CursorNESWTexture",
				"CursorNESWHotSpot",
				"CursorNWSETexture",
				"CursorNWSEHotSpot",
				"DefaultCursorTexture",
				"DefaultCursorHotSpot",
			};
		}
	}
}