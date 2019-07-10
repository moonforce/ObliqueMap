namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for Calendar date.
	/// </summary>
	public abstract class CalendarDateBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Current date to display.
		/// </summary>
		protected DateTime CurrentDate;

		/// <summary>
		/// Date belongs to this calendar.
		/// </summary>
		[HideInInspector]
		public CalendarBase Calendar;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public virtual void SetDate(DateTime currentDate)
		{
			CurrentDate = currentDate;

			DateChanged();
		}

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public abstract void DateChanged();

		/// <summary>
		/// OnPoiterDown event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerUp event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// PointerClick event.
		/// Change calendar date to clicked date.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (CurrentDate < Calendar.DateMin)
			{
				return;
			}

			if (CurrentDate > Calendar.DateMax)
			{
				return;
			}

			Calendar.Date = CurrentDate;
			Calendar.OnDateClick.Invoke(CurrentDate);
		}

		/// <summary>
		/// Apply specified style.
		/// </summary>
		/// <param name="styleCalendar">Style for the calendar.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleCalendar styleCalendar, Style style)
		{
			if (Calendar != null)
			{
				DateChanged();
			}
		}
	}
}