namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// ListView drop support for the PlaylistItem.
	/// </summary>
	[UnityEngine.RequireComponent(typeof(ListViewPlaylistItem))]
	public class ListViewDropSupportPlaylistItem : UIWidgets.ListViewCustomDropSupport<ListViewPlaylistItem, ListViewComponentPlaylistItem, UIWidgets.Examples.PlaylistItem>
	{
	}
}