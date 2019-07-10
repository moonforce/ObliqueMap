namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// ListView drag support for the PlaylistItem.
	/// </summary>
	[UnityEngine.RequireComponent(typeof(ListViewComponentPlaylistItem))]
	public class ListViewDragSupportPlaylistItem : UIWidgets.ListViewCustomDragSupport<ListViewPlaylistItem, ListViewComponentPlaylistItem, UIWidgets.Examples.PlaylistItem>
	{
		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected override UIWidgets.Examples.PlaylistItem GetData(ListViewComponentPlaylistItem component)
		{
			return component.Item;
		}

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected override void SetDragInfoData(UIWidgets.Examples.PlaylistItem data)
		{
			DragInfo.SetData(data);
		}

		/// <summary>
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		public override bool CanDrag(UnityEngine.EventSystems.PointerEventData eventData)
		{
			var component = GetComponent<ListViewComponentPlaylistItem>();
			return !component.Accordion.DataSource[0].Open;
		}
	}
}