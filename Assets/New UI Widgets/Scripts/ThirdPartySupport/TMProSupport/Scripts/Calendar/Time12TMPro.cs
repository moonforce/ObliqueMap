#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using System;
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Time widget with 12 hour format.
	/// </summary>
	public class Time12TMPro : Time24TMPro
	{
		/// <summary>
		/// The AMPM button.
		/// </summary>
		[SerializeField]
		protected Button AMPMButton;

		/// <summary>
		/// The AMPM text.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI AMPMText;

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected override void AddListeners()
		{
			base.AddListeners();

			if (AMPMButton != null)
			{
				AMPMButton.onClick.AddListener(ToggleAMPM);
			}
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();

			if (AMPMButton != null)
			{
				AMPMButton.onClick.RemoveListener(ToggleAMPM);
			}
		}

		/// <summary>
		/// Toggles the AMPM.
		/// </summary>
		public virtual void ToggleAMPM()
		{
			Time += new TimeSpan(12, 0, 0);
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		public override void UpdateInputs()
		{
			if (InputProxyMinutes != null)
			{
				InputProxyMinutes.text = Time.Minutes.ToString("D2");
			}

			if (InputProxySeconds != null)
			{
				InputProxySeconds.text = Time.Seconds.ToString("D2");
			}

			var hours = Time.Hours;

			if (AMPMText != null)
			{
				AMPMText.text = hours < 12 ? "AM" : "PM";
			}

			if (InputProxyHours != null)
			{
				if (hours == 0)
				{
					hours = 12;
				}
				else if (hours >= 13)
				{
					hours -= 12;
				}

				InputProxyHours.text = hours.ToString("D2");
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

			if (AMPMButton != null)
			{
				style.Time.AMPMBackground.ApplyTo(AMPMButton);
			}

			if (AMPMText != null)
			{
				style.Time.AMPMText.ApplyTo(AMPMText.gameObject);
			}

			return true;
		}
		#endregion
	}
}
#endif