namespace UIWidgets
{
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// FileListViewPathComponent.
	/// </summary>
	public class FileListViewPathComponent : FileListViewPathComponentBase
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected Text Name;

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="path">Path.</param>
		public override void SetPath(string path)
		{
			FullName = path;
			var dir = Path.GetFileName(path);
			Name.text = !string.IsNullOrEmpty(dir) ? dir : path;
		}

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public override void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.SetStyle(styleBackground, styleText, style);

			styleText.ApplyTo(Name);
		}
	}
}