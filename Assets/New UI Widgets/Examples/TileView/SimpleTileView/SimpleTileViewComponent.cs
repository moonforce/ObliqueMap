namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine.UI;

	/// <summary>
	/// SimpleTileView component.
	/// </summary>
	public class SimpleTileViewComponent : ListViewItem, IViewData<SimpleTileViewItem>
	{
		/// <summary>
		/// The name of the item.
		/// </summary>
		public Text ItemName;

		/// <summary>
		/// The text of the item.
		/// </summary>
		public Text ItemText;

		#region IViewData implementation

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SimpleTileViewItem item)
		{
			ItemName.text = item.Name;

			ItemText.text = item.Text.Replace("\\n", "\n");
		}
		#endregion
	}
}