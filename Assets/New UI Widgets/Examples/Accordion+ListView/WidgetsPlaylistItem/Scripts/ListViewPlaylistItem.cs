namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// ListView for the PlaylistItem.
	/// </summary>
	public class ListViewPlaylistItem : UIWidgets.ListViewCustom<ListViewComponentPlaylistItem, UIWidgets.Examples.PlaylistItem>
	{
		bool isListViewPlaylistItemInited;

		ComparersFieldsPlaylistItem currentSortField = ComparersFieldsPlaylistItem.None;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewPlaylistItemInited)
			{
				return;
			}

			isListViewPlaylistItemInited = true;

			Sort = false;

			base.Init();
		}

		/// <summary>
		/// Toggle sort.
		/// </summary>
		/// <param name="field">Sort field.</param>
		public void ToggleSort(ComparersFieldsPlaylistItem field)
		{
			if (field == currentSortField)
			{
				DataSource.Reverse();
			}
			else if (ComparersPlaylistItem.Comparers.ContainsKey((int)field))
			{
				currentSortField = field;

				DataSource.Sort(ComparersPlaylistItem.Comparers[(int)field]);
			}
		}

		#region used in Button.OnClick()

		/// <summary>
		/// Sort by Title.
		/// </summary>
		public void SortByTitle()
		{
			ToggleSort(ComparersFieldsPlaylistItem.Title);
		}

		/// <summary>
		/// Sort by PlayStart.
		/// </summary>
		public void SortByPlayStart()
		{
			ToggleSort(ComparersFieldsPlaylistItem.PlayStart);
		}

		/// <summary>
		/// Sort by PlayStop.
		/// </summary>
		public void SortByPlayStop()
		{
			ToggleSort(ComparersFieldsPlaylistItem.PlayStop);
		}
		#endregion
	}
}