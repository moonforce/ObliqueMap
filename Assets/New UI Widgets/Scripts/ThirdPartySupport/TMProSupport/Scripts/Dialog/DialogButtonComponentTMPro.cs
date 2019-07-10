#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// DialogButtonComponent.
	/// Control how button name will be displayed.
	/// </summary>
	public class DialogButtonComponentTMPro : DialogButtonComponentBase
	{
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Name;

		/// <summary>
		/// Sets the button name.
		/// </summary>
		/// <param name="name">Name.</param>
		public override void SetButtonName(string name)
		{
			Name.text = name;
		}
	}
}
#endif