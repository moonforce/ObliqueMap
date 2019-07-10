#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// List view item component.
	/// </summary>
	public class ListViewStringComponentTMPro : ListViewStringComponent
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
		/// The Text component.
		/// </summary>
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Text.</param>
		public override void SetData(string item)
		{
			TextTMPro.text = item.Replace("\\n", "\n");
		}
	}
}
#endif