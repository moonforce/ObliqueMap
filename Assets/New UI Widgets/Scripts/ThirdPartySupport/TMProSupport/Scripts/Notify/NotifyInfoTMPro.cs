#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Notify info.
	/// </summary>
	public class NotifyInfoTMPro : NotifyInfoBase
	{
		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Message;

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="message">Message.</param>
		public override void SetInfo(string message)
		{
			if ((Message != null) && (message != null))
			{
				Message.text = message;
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public override bool SetStyle(Style style)
		{
			if (Message != null)
			{
				style.Notify.Text.ApplyTo(Message.gameObject);
			}

			return true;
		}
		#endregion
	}
}
#endif