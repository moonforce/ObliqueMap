namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// CalendarDayOfWeek.
	/// Display day of week.
	/// </summary>
	public class CalendarDayOfWeek : CalendarDayOfWeekBase
	{
		/// <summary>
		/// Text component to display day of week.
		/// </summary>
		[SerializeField]
		protected Text Day;

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
			styleCalendar.DayOfWeekText.ApplyTo(Day);
			styleCalendar.DayOfWeekBackground.ApplyTo(GetComponent<Image>());
		}
	}
}