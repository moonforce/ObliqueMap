namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Scroll rect page with number.
	/// </summary>
	public class ScrollRectPageWithNumber : ScrollRectPage
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public Text Number;

		/// <summary>
		/// Sets the page number.
		/// </summary>
		/// <param name="page">Page.</param>
		public override void SetPage(int page)
		{
			base.SetPage(page);
			if (Number != null)
			{
				Number.text = (page + 1).ToString();
			}
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public override void SetStyle(StyleText styleText, Style style)
		{
			styleText.ApplyTo(Number);
		}
	}
}