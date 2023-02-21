#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Scroll rect page with number.
	/// </summary>
	public class ScrollRectPageWithNumberTMPro : ScrollRectPage
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Number;

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
			if (Number != null)
			{
				styleText.ApplyTo(Number.gameObject);
			}
		}
	}
}
#endif