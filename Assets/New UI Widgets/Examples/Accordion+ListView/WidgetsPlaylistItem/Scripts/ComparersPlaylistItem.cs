namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// Sort fields for the for the PlaylistItem.
	/// </summary>
	public enum ComparersFieldsPlaylistItem
	{
		/// <summary>
		/// Not sorted list.
		/// </summary>
		None,

		/// <summary>
		/// Title.
		/// </summary>
		Title,

		/// <summary>
		/// PlayStart.
		/// </summary>
		PlayStart,

		/// <summary>
		/// PlayStop.
		/// </summary>
		PlayStop,
	}

	/// <summary>
	/// Comparers functions for the PlaylistItem.
	/// </summary>
	public static class ComparersPlaylistItem
	{
		/// <summary>
		/// Comparers.
		/// </summary>
		public static readonly System.Collections.Generic.Dictionary<int, System.Comparison<UIWidgets.Examples.PlaylistItem>> Comparers = new System.Collections.Generic.Dictionary<int, System.Comparison<UIWidgets.Examples.PlaylistItem>>()
			{
				{ (int)ComparersFieldsPlaylistItem.Title, TitleComparer },
				{ (int)ComparersFieldsPlaylistItem.PlayStart, PlayStartComparer },
				{ (int)ComparersFieldsPlaylistItem.PlayStop, PlayStopComparer },
			};

		#region Items comparers

		/// <summary>
		/// Title comparer.
		/// </summary>
		/// <param name="x">First PlaylistItem.</param>
		/// <param name="y">Second PlaylistItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int TitleComparer(UIWidgets.Examples.PlaylistItem x, UIWidgets.Examples.PlaylistItem y)
		{
			return x.Title.CompareTo(y.Title);
		}

		/// <summary>
		/// PlayStart comparer.
		/// </summary>
		/// <param name="x">First PlaylistItem.</param>
		/// <param name="y">Second PlaylistItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int PlayStartComparer(UIWidgets.Examples.PlaylistItem x, UIWidgets.Examples.PlaylistItem y)
		{
			return x.PlayStart.CompareTo(y.PlayStart);
		}

		/// <summary>
		/// PlayStop comparer.
		/// </summary>
		/// <param name="x">First PlaylistItem.</param>
		/// <param name="y">Second PlaylistItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int PlayStopComparer(UIWidgets.Examples.PlaylistItem x, UIWidgets.Examples.PlaylistItem y)
		{
			return x.PlayStop.CompareTo(y.PlayStop);
		}
		#endregion
	}
}