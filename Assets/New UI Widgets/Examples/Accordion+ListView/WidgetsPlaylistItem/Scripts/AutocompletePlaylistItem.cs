namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// Autocomplete for the PlaylistItem.
	/// </summary>
	public class AutocompletePlaylistItem : UIWidgets.AutocompleteCustom<UIWidgets.Examples.PlaylistItem, ListViewComponentPlaylistItem, ListViewPlaylistItem>
	{
		/// <summary>
		/// Determines whether the beginnig of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginnig of value matches the Input; otherwise, false.</returns>
		public override bool Startswith(UIWidgets.Examples.PlaylistItem value)
		{
			if (CaseSensitive)
			{
				return value.Title.StartsWith(Query);
			}

			return value.Title.ToLower().StartsWith(Query.ToLower());
		}

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(UIWidgets.Examples.PlaylistItem value)
		{
			if (CaseSensitive)
			{
				return value.Title.Contains(Query);
			}

			return value.Title.ToLower().Contains(Query.ToLower());
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(UIWidgets.Examples.PlaylistItem value)
		{
			return value.Title;
		}
	}
}