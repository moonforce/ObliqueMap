namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// Test PlaylistItem.
	/// </summary>
	public partial class TestPlaylistItem : UIWidgets.WidgetGeneration.TestBase<UIWidgets.Examples.PlaylistItem>
	{
		/// <summary>
		/// Generate item.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override UIWidgets.Examples.PlaylistItem GenerateItem(int index)
		{
			return new UIWidgets.Examples.PlaylistItem()
			{
				Title = "Title " + index.ToString("0000"),
				PlayStart = UnityEngine.Random.Range(0, 100000),
				PlayStop = UnityEngine.Random.Range(0, 100000),
			};
		}

		/// <summary>
		/// Generate item with the specified name.
		/// </summary>
		/// <param name="name">Item name.</param>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override UIWidgets.Examples.PlaylistItem GenerateItem(string name, int index)
		{
			var item = GenerateItem(index);

			item.Title = name;

			return item;
		}
	}
}