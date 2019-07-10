namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// Test script for the PlaylistItem.
	/// </summary>
	public partial class TestPlaylistItem : UIWidgets.WidgetGeneration.TestBase<UIWidgets.Examples.PlaylistItem>
	{
		/// <summary>
		/// Left ListView.
		/// </summary>
		public ListViewPlaylistItem LeftListView;

		/// <summary>
		/// Right ListView.
		/// </summary>
		public ListViewPlaylistItem RightListView;

		/// <summary>
		/// TileView.
		/// </summary>
		public ListViewPlaylistItem TileView;

		/// <summary>
		/// TreeView.
		/// </summary>
		public TreeViewPlaylistItem TreeView;

		/// <summary>
		/// Table.
		/// </summary>
		public ListViewPlaylistItem Table;

		/// <summary>
		/// TreeGraph.
		/// </summary>
		public TreeGraphPlaylistItem TreeGraph;

		/// <summary>
		/// Autocomplete.
		/// </summary>
		public AutocompletePlaylistItem Autocomplete;

		/// <summary>
		/// Combobox.
		/// </summary>
		public ComboboxPlaylistItem Combobox;

		/// <summary>
		/// ComboboxMultiselect.
		/// </summary>
		public ComboboxPlaylistItem ComboboxMultiselect;

		/// <summary>
		/// ListView picker.
		/// </summary>
		public PickerListViewPlaylistItem PickerListView;

		/// <summary>
		/// TreeView picker.
		/// </summary>
		public PickerTreeViewPlaylistItem PickerTreeView;

		UIWidgets.ObservableList<UIWidgets.Examples.PlaylistItem> pickerListViewData;

		UIWidgets.ObservableList<UIWidgets.TreeNode<UIWidgets.Examples.PlaylistItem>> pickerTreeViewNodes;

		/// <summary>
		/// Init.
		/// </summary>
		public void Start()
		{
			var list = GenerateList(4);

			LeftListView.DataSource = list;
			TileView.DataSource = list;

			RightListView.DataSource = GenerateList(15);

			Table.DataSource = GenerateList(50);

			TreeView.Nodes = GenerateNodes(new System.Collections.Generic.List<int>() { 10, 5, 5, });

			TreeGraph.Nodes = GenerateNodes(new System.Collections.Generic.List<int>() { 2, 3, 2, });

			Autocomplete.DataSource = new System.Collections.Generic.List<UIWidgets.Examples.PlaylistItem>(GenerateList(50));
			Combobox.ListView.DataSource = GenerateList(20);
			ComboboxMultiselect.ListView.DataSource = GenerateList(20);

			pickerListViewData = GenerateList(20);

			pickerTreeViewNodes = GenerateNodes(new System.Collections.Generic.List<int>() { 10, 5, 3, });
		}

		/// <summary>
		/// Show ListView picker.
		/// </summary>
		public void ShowPickerListView()
		{
			var picker = PickerListView.Clone();
			picker.ListView.DataSource = pickerListViewData;
			picker.Show(null, LeftListView.DataSource.Add);
		}

		/// <summary>
		/// Show TreeView picker.
		/// </summary>
		public void ShowPickerTreeView()
		{
			var picker = PickerTreeView.Clone();
			picker.TreeView.Nodes = pickerTreeViewNodes;
			picker.Show(null, TreeView.Nodes.Add);
		}
	}
}