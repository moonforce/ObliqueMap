#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DrivesListViewComponent TMPro.
	/// Display drive.
	/// </summary>
	public class DrivesListViewComponentTMPro : DrivesListViewComponentBase
	{
		/// <summary>
		/// Text component to display drive name.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI Name;

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(FileSystemEntry item)
		{
			Item = item;

			Name.text = Item.DisplayName;
		}

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public override void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.SetStyle(styleBackground, styleText, style);

			if (Name != null)
			{
				styleText.ApplyTo(Name.gameObject);
			}
		}
	}
}
#endif