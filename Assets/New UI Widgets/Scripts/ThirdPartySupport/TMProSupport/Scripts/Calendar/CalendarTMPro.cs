#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Calendar TMPro.
	/// </summary>
	public class CalendarTMPro : CalendarBase
	{
		[SerializeField]
		TextMeshProUGUI dateText;

		/// <summary>
		/// Text to display current date.
		/// </summary>
		public TextMeshProUGUI DateText
		{
			get
			{
				return dateText;
			}

			set
			{
				dateText = value;

				UpdateDate();
			}
		}

		[SerializeField]
		TextMeshProUGUI monthText;

		/// <summary>
		/// Text to display current month.
		/// </summary>
		public TextMeshProUGUI MonthText
		{
			get
			{
				return monthText;
			}

			set
			{
				monthText = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Update displayed date and month.
		/// </summary>
		protected override void UpdateDate()
		{
			if (dateText != null)
			{
				dateText.text = Date.ToString("D", Culture);
			}

			if (monthText != null)
			{
				monthText.text = DateDisplay.ToString("Y", Culture);
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

			if (dateText != null)
			{
				style.Calendar.CurrentDate.ApplyTo(dateText.gameObject);
			}

			if (monthText != null)
			{
				style.Calendar.CurrentMonth.ApplyTo(monthText.gameObject);
			}

			return true;
		}
		#endregion
	}
}
#endif