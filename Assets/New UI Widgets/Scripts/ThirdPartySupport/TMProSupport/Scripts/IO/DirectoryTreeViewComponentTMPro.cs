#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UnityEngine.UI;

	/// <summary>
	/// DirectoryTreeView component TMPro.
	/// </summary>
	public class DirectoryTreeViewComponentTMPro : DirectoryTreeViewComponent
	{
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
		/// Text.
		/// </summary>
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected override void UpdateView()
		{
			TextTMPro.text = Node.Item.DisplayName;
		}
	}
}
#endif