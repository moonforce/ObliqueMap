namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// FileListViewComponentBase.
	/// </summary>
	public abstract class FileListViewComponentBase : ListViewItem, IViewData<FileSystemEntry>
	{
		/// <summary>
		/// Icon.
		/// </summary>
		[SerializeField]
		protected Image Icon;

		/// <summary>
		/// Directory icon.
		/// </summary>
		[SerializeField]
		protected Sprite DirectoryIcon;

		/// <summary>
		/// Current item.
		/// </summary>
		protected FileSystemEntry Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(FileSystemEntry item)
		{
			Item = item;

			Icon.sprite = GetIcon(item);
			Icon.color = Icon.sprite == null ? Color.clear : Color.white;
		}

		/// <summary>
		/// Get icon for specified FileSystemEntry.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Icon for specified FileSystemEntry.</returns>
		public virtual Sprite GetIcon(FileSystemEntry item)
		{
			if (item.IsDirectory)
			{
				return DirectoryIcon;
			}

			return null;
		}

		/// <summary>
		/// Handle double click event.
		/// </summary>
		public virtual void DoubleClick()
		{
			if (Item.IsDirectory)
			{
				(Owner as FileListView).CurrentDirectory = Item.FullName;
			}
		}
	}
}