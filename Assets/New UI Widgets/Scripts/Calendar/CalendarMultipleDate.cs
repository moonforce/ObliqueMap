namespace UIWidgets
{
	/// <summary>
	/// CalendarMultipleDate.
	/// Display date.
	/// </summary>
	public class CalendarMultipleDate : CalendarDate
	{
		/// <summary>
		/// CalendarMultipleDates.
		/// </summary>
		public CalendarMultipleDates Dates;

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public override void DateChanged()
		{
			Day.text = CurrentDate.ToString("dd", Calendar.Culture);

			if (Dates.IsSelected(CurrentDate))
			{
				Day.color = SelectedDay;
				DayImage.sprite = SelectedDayBackground;
			}
			else
			{
				DayImage.sprite = DefaultDayBackground;

				if (Calendar.IsSameMonth(Calendar.DateDisplay, CurrentDate))
				{
					if (Calendar.IsWeekend(CurrentDate) ||
						Calendar.IsHoliday(CurrentDate))
					{
						Day.color = Weekend;
					}
					else
					{
						Day.color = CurrentMonth;
					}
				}
				else
				{
					if (Calendar.IsWeekend(CurrentDate) ||
						Calendar.IsHoliday(CurrentDate))
					{
						Day.color = Weekend * OtherMonth;
					}
					else
					{
						Day.color = OtherMonth;
					}
				}

				if (CurrentDate < Calendar.DateMin)
				{
					Day.color *= OtherMonth;
				}
				else if (CurrentDate > Calendar.DateMax)
				{
					Day.color *= OtherMonth;
				}
			}
		}
	}
}