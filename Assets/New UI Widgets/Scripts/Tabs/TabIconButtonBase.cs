namespace UIWidgets
{
	using UnityEngine.UI;

	/// <summary>
	/// TabIconButton.
	/// </summary>
	public abstract class TabIconButtonBase : TabButton
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public abstract void SetData(TabIcons tab);
	}
}