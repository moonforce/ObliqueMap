namespace UIWidgets
{
	/// <summary>
	/// ListViewInt base component.
	/// </summary>
	public abstract class ListViewIntComponentBase : ListViewItem, IViewData<int>
	{
		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public abstract void SetData(int item);
	}
}