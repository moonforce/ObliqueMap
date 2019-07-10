#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// PickerBool.
	/// </summary>
	public class PickerBoolTMPro : PickerBool
	{
		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI MessageTMPro;

		/// <summary>
		/// Set message.
		/// </summary>
		/// <param name="message">Message text.</param>
		public override void SetMessage(string message)
		{
			MessageTMPro.text = message;
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			if (MessageTMPro != null)
			{
				style.Dialog.ContentText.ApplyTo(MessageTMPro.gameObject);
			}

			return true;
		}
		#endregion
	}
}
#endif