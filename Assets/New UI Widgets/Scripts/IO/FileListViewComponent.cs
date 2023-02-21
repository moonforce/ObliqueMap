namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// FileListViewComponent
	/// </summary>
	public class FileListViewComponent : FileListViewComponentBase
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected Text Name;

		/// <summary>
		/// Foreground graphics.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Name };
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(FileSystemEntry item)
		{
			base.SetData(item);

			Name.text = item.DisplayName;
		}
	}
}