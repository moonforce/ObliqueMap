namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// PickerBool.
	/// </summary>
	public class PickerBool : Picker<bool, PickerBool>
	{
		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		protected Text Message;

		/// <summary>
		/// Set message.
		/// </summary>
		/// <param name="message">Message text.</param>
		public virtual void SetMessage(string message)
		{
			Message.text = message;
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(bool defaultValue)
		{
		}

		/// <summary>
		/// Select true value.
		/// </summary>
		public void Yes()
		{
			Selected(true);
		}

		/// <summary>
		/// Select false value.
		/// </summary>
		public void No()
		{
			Selected(false);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
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

			style.Dialog.ContentText.ApplyTo(Message);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Yes"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/No"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}