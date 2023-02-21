namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// ListView with disable coloring.
	/// </summary>
	public class ListViewNoColoring : ListView
	{
		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewStringComponent component)
		{
			// do nothing
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(ListViewStringComponent component)
		{
			// do nothing
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(ListViewStringComponent component)
		{
			// do nothing
		}
	}
}