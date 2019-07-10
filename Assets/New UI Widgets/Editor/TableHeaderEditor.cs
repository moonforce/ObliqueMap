namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// TableHeader editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TableHeader), true)]
	public class TableHeaderEditor : CursorsEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TableHeaderEditor"/> class.
		/// </summary>
		public TableHeaderEditor()
		{
			Cursors = new List<string>()
			{
				"CurrentCamera",
				"CursorTexture",
				"CursorHotSpot",
				"AllowDropCursor",
				"AllowDropCursorHotSpot",
				"DeniedDropCursor",
				"DeniedDropCursorHotSpot",
				"DefaultCursorTexture",
				"DefaultCursorHotSpot",
			};
		}
	}
}