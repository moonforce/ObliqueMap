#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using System.IO;
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// FileListViewPath component TMPro.
	/// </summary>
	public class FileListViewPathComponentTMPro : FileListViewPathComponentBase
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI Name;

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
	}
}
#endif