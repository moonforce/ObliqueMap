namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView for items with variable size.
	/// </summary>
	/// <typeparam name="TComponent">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public class ListViewCustomSize<TComponent, TItem> : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		bool isListViewCustomSizeInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewCustomSizeInited)
			{
				return;
			}

			isListViewCustomSizeInited = true;

			listType = ListViewType.ListViewWithVariableSize;

			base.Init();
		}
	}
}