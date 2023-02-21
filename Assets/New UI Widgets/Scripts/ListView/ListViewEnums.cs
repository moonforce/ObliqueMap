namespace UIWidgets
{
	/// <summary>
	/// ListView direction.
	/// </summary>
	public enum ListViewDirection
	{
		/// <summary>
		/// Horizontal scroll direction.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical scroll direction.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// ListView type.
	/// </summary>
	public enum ListViewType
	{
		/// <summary>
		/// ListView with items of fixed size.
		/// </summary>
		ListViewWithFixedSize = 0,

		/// <summary>
		/// ListView with items of variable size.
		/// </summary>
		ListViewWithVariableSize = 1,

		/// <summary>
		/// TileView with items of fixed size.
		/// </summary>
		TileViewWithFixedSize = 2,

		/// <summary>
		/// TileView with items of variable size.
		/// </summary>
		TileViewWithVariableSize = 3,
	}
}