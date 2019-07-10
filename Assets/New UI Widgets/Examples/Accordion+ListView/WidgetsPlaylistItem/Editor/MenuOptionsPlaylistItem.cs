#if UNITY_EDITOR
namespace UIWidgets.Examples.Widgets
{
	using UIWidgets;
	using UnityEditor;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class MenuOptionsPlaylistItem
	{
		/// <summary>
		/// Create Autocomplete.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/Autocomplete", false, 2000)]
		public static void CreateAutocomplete()
		{
			Utilites.CreateWidgetFromAsset("GeneratedAutocompletePlaylistItem");
		}

		/// <summary>
		/// Create Combobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/Combobox", false, 2010)]
		public static void CreateCombobox()
		{
			Utilites.CreateWidgetFromAsset("GeneratedComboboxPlaylistItem");
		}

		/// <summary>
		/// Create ComboboxMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/ComboboxMultiselect", false, 2020)]
		public static void CreateComboboxMultiselect()
		{
			Utilites.CreateWidgetFromAsset("GeneratedComboboxMultiselectPlaylistItem");
		}

		/// <summary>
		/// Create DragInfo.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/DragInfo", false, 2030)]
		public static void CreateDragInfo()
		{
			Utilites.CreateWidgetFromAsset("GeneratedDragInfoPlaylistItem");
		}

		/// <summary>
		/// Create ListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/ListView", false, 2040)]
		public static void CreateListView()
		{
			Utilites.CreateWidgetFromAsset("GeneratedListViewPlaylistItem");
		}

		/// <summary>
		/// Create Table.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/Table", false, 2050)]
		public static void CreateTable()
		{
			Utilites.CreateWidgetFromAsset("GeneratedTablePlaylistItem");
		}

		/// <summary>
		/// Create TileView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/TileView", false, 2060)]
		public static void CreateTileView()
		{
			Utilites.CreateWidgetFromAsset("GeneratedTileViewPlaylistItem");
		}

		/// <summary>
		/// Create TreeGraph.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/TreeGraph", false, 2070)]
		public static void CreateTreeGraph()
		{
			Utilites.CreateWidgetFromAsset("GeneratedTreeGraphPlaylistItem");
		}

		/// <summary>
		/// Create TreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/TreeView", false, 2080)]
		public static void CreateTreeView()
		{
			Utilites.CreateWidgetFromAsset("GeneratedTreeViewPlaylistItem");
		}

		/// <summary>
		/// Create PickerListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/PickerListView", false, 2090)]
		public static void CreatePickerListView()
		{
			Utilites.CreateWidgetFromAsset("GeneratedPickerListViewPlaylistItem");
		}

		/// <summary>
		/// Create PickerTreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - PlaylistItem/PickerTreeView", false, 2100)]
		public static void CreatePickerTreeView()
		{
			Utilites.CreateWidgetFromAsset("GeneratedPickerTreeViewPlaylistItem");
		}
	}
}
#endif