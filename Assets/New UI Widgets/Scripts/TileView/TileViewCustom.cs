namespace UIWidgets
{
	/// <summary>
	/// Base class for TileView's.
	/// </summary>
	/// <typeparam name="TComponent">Component class.</typeparam>
	/// <typeparam name="TItem">Item class.</typeparam>
	public class TileViewCustom<TComponent, TItem> : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		bool isTileViewCustomInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isTileViewCustomInited)
			{
				return;
			}

			isTileViewCustomInited = true;

			listType = ListViewType.TileViewWithFixedSize;

			base.Init();
		}
	}
}