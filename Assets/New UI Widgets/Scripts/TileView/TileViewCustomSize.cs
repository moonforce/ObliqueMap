namespace UIWidgets
{
	/// <summary>
	/// Base class for TileView's with items with different widths or heights.
	/// </summary>
	/// <typeparam name="TComponent">Component class.</typeparam>
	/// <typeparam name="TItem">Item class.</typeparam>
	public class TileViewCustomSize<TComponent, TItem> : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		bool isTileViewCustomSizeInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isTileViewCustomSizeInited)
			{
				return;
			}

			isTileViewCustomSizeInited = true;

			listType = ListViewType.TileViewWithVariableSize;

			base.Init();
		}
	}
}