#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using System.ComponentModel;
	using TMPro;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeView component TMPro.
	/// </summary>
	public class TreeViewComponentTMPro : TreeViewComponent
	{
		/// <summary>
		/// Text.
		/// </summary>
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { TextTMPro, };
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="ev">Event data.</param>
		protected override void UpdateView(object sender = null, PropertyChangedEventArgs ev = null)
		{
			if ((Icon == null) || (TextTMPro == null))
			{
				return;
			}

			if (Item == null)
			{
				Icon.sprite = null;
				TextTMPro.text = string.Empty;
			}
			else
			{
				Icon.sprite = Item.Icon;
				TextTMPro.text = Item.LocalizedName ?? Item.Name;
			}

			if (SetNativeSize)
			{
				Icon.SetNativeSize();
			}

			// set transparent color if no icon
			Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
		}
	}
}
#endif