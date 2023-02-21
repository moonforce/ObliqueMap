namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Time widget for 24 hour format.
	/// </summary>
	public class Time24 : TimeBase
	{
		/// <summary>
		/// The input field for the hours.
		/// </summary>
		[SerializeField]
		protected InputField InputHours;

		/// <summary>
		/// The input field for the minutes.
		/// </summary>
		[SerializeField]
		protected InputField InputMinutes;

		/// <summary>
		/// The input field for the seconds.
		/// </summary>
		[SerializeField]
		protected InputField InputSeconds;

		/// <summary>
		/// Init the input.
		/// </summary>
		protected override void InitInput()
		{
			if (InputHours != null)
			{
				InputProxyHours = new InputFieldProxy(InputHours);
			}

			if (InputMinutes != null)
			{
				InputProxyMinutes = new InputFieldProxy(InputMinutes);
			}

			if (InputSeconds != null)
			{
				InputProxySeconds = new InputFieldProxy(InputSeconds);
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
			base.SetStyle(style);

			if (InputHours != null)
			{
				style.Time.InputBackground.ApplyTo(InputHours);
				style.Time.InputText.ApplyTo(InputHours.textComponent);
			}

			if (InputMinutes)
			{
				style.Time.InputBackground.ApplyTo(InputMinutes);
				style.Time.InputText.ApplyTo(InputMinutes.textComponent);
			}

			if (InputSeconds)
			{
				style.Time.InputBackground.ApplyTo(InputSeconds);
				style.Time.InputText.ApplyTo(InputSeconds.textComponent);
			}

			return true;
		}
		#endregion
	}
}