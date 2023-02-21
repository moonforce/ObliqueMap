namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Base class for Calendar day of week.
	/// </summary>
	public abstract class CalendarDayOfWeekBase : MonoBehaviour
	{
		/// <summary>
		/// Date belongs to this calendar.
		/// </summary>
		[HideInInspector]
		public CalendarBase Calendar;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public abstract void SetDate(DateTime currentDate);

		/// <summary>
		/// Apply specified style.
		/// </summary>
		/// <param name="styleCalendar">Style for the calendar.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleCalendar styleCalendar, Style style)
		{
		}
	}
}