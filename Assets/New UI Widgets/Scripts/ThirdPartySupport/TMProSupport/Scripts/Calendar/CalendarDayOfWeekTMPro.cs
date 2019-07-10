#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using System;
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// CalendarDayOfWeek TMPro.
	/// Display day of week.
	/// </summary>
	public class CalendarDayOfWeekTMPro : CalendarDayOfWeekBase
	{
		/// <summary>
		/// Text component to display day of week.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI Day;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public override void SetDate(DateTime currentDate)
		{
			Day.text = currentDate.ToString("ddd", Calendar.Culture);
		}

		/// <summary>
		/// Apply specified style.
		/// </summary>
		/// <param name="styleCalendar">Style for the calendar.</param>
		/// <param name="style">Full style data.</param>
		public override void SetStyle(StyleCalendar styleCalendar, Style style)
		{
			if (Day != null)
			{
				styleCalendar.DayOfWeekText.ApplyTo(Day.gameObject);
			}

			styleCalendar.DayOfWeekBackground.ApplyTo(GetComponent<Image>());
		}
	}
}
#endif