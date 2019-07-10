namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using EasyLayoutNS;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Calendar base class.
	/// </summary>
	[DataBindSupport]
	public abstract class CalendarBase : MonoBehaviour, IStylable
	{
		[SerializeField]
		DateTime date = DateTime.Today;

		/// <summary>
		/// Selected date.
		/// </summary>
		[DataBindField]
		public DateTime Date
		{
			get
			{
				return date;
			}

			set
			{
				var is_changed = date != value;

				Init();
				date = Clamp(value);
				dateDisplay = date;

				UpdateCalendar();

				if (is_changed)
				{
					OnDateChanged.Invoke(date);
				}
			}
		}

		[SerializeField]
		DateTime dateMin = DateTime.MinValue;

		/// <summary>
		/// The minimum selectable date.
		/// </summary>
		/// <value>The date minimum.</value>
		public DateTime DateMin
		{
			get
			{
				return dateMin;
			}

			set
			{
				Init();
				dateMin = value;

				date = Clamp(date);
			}
		}

		[SerializeField]
		DateTime dateMax = DateTime.MaxValue;

		/// <summary>
		/// The maximum selectable date.
		/// </summary>
		/// <value>The date max.</value>
		public DateTime DateMax
		{
			get
			{
				return dateMax;
			}

			set
			{
				Init();
				dateMax = value;

				date = Clamp(date);
			}
		}

		[SerializeField]
		DayOfWeek firstDayOfWeek;

		/// <summary>
		/// First day of week.
		/// </summary>
		public DayOfWeek FirstDayOfWeek
		{
			get
			{
				return firstDayOfWeek;
			}

			set
			{
				firstDayOfWeek = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Container for dates.
		/// </summary>
		[SerializeField]
		public RectTransform Container;

		[SerializeField]
		CalendarDateBase calendarDateTemplate;

		/// <summary>
		/// Date template.
		/// </summary>
		public CalendarDateBase CalendarDateTemplate
		{
			get
			{
				return calendarDateTemplate;
			}

			set
			{
				calendarDateTemplate = value;

				Init();
				UpdateCalendar();
			}
		}

		/// <summary>
		/// Container for day of weeks.
		/// </summary>
		[SerializeField]
		public RectTransform HeaderContainer;

		[SerializeField]
		CalendarDayOfWeekBase calendarDayOfWeekTemplate;

		/// <summary>
		/// Day of week template.
		/// </summary>
		public CalendarDayOfWeekBase CalendarDayOfWeekTemplate
		{
			get
			{
				return calendarDayOfWeekTemplate;
			}

			set
			{
				calendarDayOfWeekTemplate = value;

				InitHeader();

				UpdateCalendar();
			}
		}

		DateTime dateDisplay = DateTime.Today;

		/// <summary>
		/// Displayed month in calendar.
		/// </summary>
		public DateTime DateDisplay
		{
			get
			{
				return dateDisplay;
			}

			set
			{
				dateDisplay = value;

				UpdateCalendar();
			}
		}

		CultureInfo culture = CultureInfo.InvariantCulture;

		/// <summary>
		/// Current culture.
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return culture;
			}

			set
			{
				culture = value;

				UpdateCalendar();
			}
		}

		[SerializeField]
		bool currentDateAsDefault = true;

		[SerializeField]
		[FormerlySerializedAs("currentDate")]
		string defaultDate = DateTime.Today.ToString("yyyy-MM-dd");

		[SerializeField]
		[FormerlySerializedAs("currentDateMin")]
		string defaultDateMin = DateTime.MinValue.ToString("yyyy-MM-dd");

		[SerializeField]
		[FormerlySerializedAs("currentDateMax")]
		string defaultDateMax = DateTime.MaxValue.ToString("yyyy-MM-dd");

		[SerializeField]
		string format = "yyyy-MM-dd";

		/// <summary>
		/// Date format.
		/// </summary>
		public string Format
		{
			get
			{
				return format;
			}

			set
			{
				format = value;

				UpdateDate();
			}
		}

		/// <summary>
		/// Event called when date changed.
		/// </summary>
		[DataBindEvent("Date")]
		public CalendarDateEvent OnDateChanged = new CalendarDateEvent();

		/// <summary>
		/// Event called when date clicked.
		/// </summary>
		/// //!added
		public CalendarDateEvent OnDateClick = new CalendarDateEvent();

		/// <summary>
		/// Days in week.
		/// </summary>
		protected int DaysInWeek = 7;

		/// <summary>
		/// Daiplayed weeks count.
		/// </summary>
		protected int DisplayedWeeks = 6;

		bool isInited;

		EasyLayout layout;

		/// <summary>
		/// Container layout.
		/// </summary>
		public EasyLayout Layout
		{
			get
			{
				if (layout == null)
				{
					layout = Container.GetComponent<EasyLayout>();
				}

				return layout;
			}
		}

		/// <summary>
		/// Cache for instatiated date components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<CalendarDateBase> Cache = new List<CalendarDateBase>();

		/// <summary>
		/// Displayed days components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<CalendarDateBase> Days = new List<CalendarDateBase>();

		/// <summary>
		/// Displayed days of week components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<CalendarDayOfWeekBase> Header = new List<CalendarDayOfWeekBase>();

		/// <summary>
		/// Start.
		/// </summary>
		public virtual void Start()
		{
			InitFull();
		}

		/// <summary>
		/// Full initialization of this instance.
		/// </summary>
		public virtual void InitFull()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			dateMin = DateTime.ParseExact(defaultDateMin, format, Culture);
			dateMax = DateTime.ParseExact(defaultDateMax, format, Culture);

			date = currentDateAsDefault
				? Clamp(DateTime.Now)
				: Clamp(DateTime.ParseExact(defaultDate, format, Culture));

			dateDisplay = date;

			Init();
			InitHeader();

			UpdateCalendar();
		}

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void Init()
		{
			calendarDateTemplate.gameObject.SetActive(false);

			Layout.LayoutType = LayoutTypes.Grid;
			Layout.Stacking = Stackings.Horizontal;
			Layout.GridConstraint = GridConstraints.FixedColumnCount;
			Layout.GridConstraintCount = DaysInWeek;

			Cache.ForEach(x => Destroy(x.gameObject));
			Cache.Clear();

			Days.ForEach(x => Destroy(x.gameObject));
			Days.Clear();
		}

		/// <summary>
		/// init header.
		/// </summary>
		protected virtual void InitHeader()
		{
			calendarDayOfWeekTemplate.gameObject.SetActive(false);

			Header.ForEach(x => Destroy(x.gameObject));
			Header.Clear();
		}

		/// <summary>
		/// Clamp specified date.
		/// </summary>
		/// <param name="d">Date.</param>
		/// <returns>Date</returns>
		protected virtual DateTime Clamp(DateTime d)
		{
			if (d < dateMin)
			{
				d = dateMin;
			}

			if (d > dateMax)
			{
				d = dateMax;
			}

			return d;
		}

		/// <summary>
		/// Displayed days count for specified month.
		/// </summary>
		/// <param name="displayedMonth">Displayed month.</param>
		/// <returns>Displayed days count.</returns>
		protected virtual int GetMaxDisplayedDays(DateTime displayedMonth)
		{
			return DaysInWeek * DisplayedWeeks;
		}

		/// <summary>
		/// Updates the calendar.
		/// Should be called only after changing culture settings.
		/// </summary>
		public virtual void UpdateCalendar()
		{
			InitFull();

			UpdateHeader();
			UpdateDays();
			UpdateDate();
		}

		/// <summary>
		/// Update displayed date and month.
		/// </summary>
		protected virtual void UpdateDate()
		{
		}

		/// <summary>
		/// Update header.
		/// </summary>
		protected virtual void UpdateHeader()
		{
			if (CalendarDayOfWeekTemplate == null)
			{
				return;
			}

			GenerateHeader();

			var firstDay = GetFirstBlockDay(dateDisplay);
			for (int i = 0; i < DaysInWeek; i++)
			{
				Header[i].SetDate(firstDay.AddDays(i));
			}
		}

		/// <summary>
		/// Update days.
		/// </summary>
		protected virtual void UpdateDays()
		{
			GenerateDays(dateDisplay);

			var n = GetMaxDisplayedDays(dateDisplay);
			var firstDay = GetFirstBlockDay(dateDisplay);
			for (int i = 0; i < n; i++)
			{
				Days[i].SetDate(firstDay.AddDays(i));
			}
		}

		/// <summary>
		/// Get first day for displayed month.
		/// </summary>
		/// <param name="day">Displayed month.</param>
		/// <returns>First day for displayed month.</returns>
		protected virtual DateTime GetFirstBlockDay(DateTime day)
		{
			var first = day.AddDays(-culture.DateTimeFormat.Calendar.GetDayOfMonth(day) + 1);

			while (culture.DateTimeFormat.Calendar.GetDayOfWeek(first) != FirstDayOfWeek)
			{
				first = first.AddDays(-1);
			}

			return first;
		}

		/// <summary>
		/// Instantiated date component from template.
		/// </summary>
		/// <returns>Date component.</returns>
		protected virtual CalendarDateBase GenerateDay()
		{
			CalendarDateBase day;

			if (Cache.Count > 0)
			{
				day = Cache.Pop();
			}
			else
			{
				day = Compatibility.Instantiate(CalendarDateTemplate);
				day.transform.SetParent(Container, false);
				Utilites.FixInstantiated(CalendarDateTemplate, day);

				day.Calendar = this;
			}

			day.gameObject.SetActive(true);
			day.transform.SetAsLastSibling();

			return day;
		}

		/// <summary>
		/// Create header from day of week template.
		/// </summary>
		protected virtual void GenerateHeader()
		{
			if (CalendarDayOfWeekTemplate == null)
			{
				return;
			}

			for (int i = Header.Count; i < DaysInWeek; i++)
			{
				var day_of_week = Compatibility.Instantiate(CalendarDayOfWeekTemplate);
				day_of_week.transform.SetParent(HeaderContainer, false);
				Utilites.FixInstantiated(CalendarDayOfWeekTemplate, day_of_week);

				day_of_week.Calendar = this;

				day_of_week.gameObject.SetActive(true);

				Header.Add(day_of_week);
			}
		}

		/// <summary>
		/// Create days for displayed month.
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		protected virtual void GenerateDays(DateTime displayedDate)
		{
			var n = GetMaxDisplayedDays(displayedDate);

			if (n > Days.Count)
			{
				for (int i = Days.Count; i < n; i++)
				{
					Days.Add(GenerateDay());
				}
			}
			else if (n < Days.Count)
			{
				for (int i = n; i < Days.Count - n; i++)
				{
					Cache.Add(Days[i]);
				}

				Cache.ForEach(x => x.gameObject.SetActive(false));
				Days.RemoveRange(n, Days.Count - n);
			}
		}

		/// <summary>
		/// Display next month.
		/// </summary>
		public virtual void NextMonth()
		{
			DateDisplay = DateDisplay.AddMonths(1);
		}

		/// <summary>
		/// Display previous month.
		/// </summary>
		public virtual void PrevMonth()
		{
			DateDisplay = DateDisplay.AddMonths(-1);
		}

		/// <summary>
		/// Display next year.
		/// </summary>
		public virtual void NextYear()
		{
			DateDisplay = DateDisplay.AddYears(1);
		}

		/// <summary>
		/// Display previous year.
		/// </summary>
		public virtual void PrevYear()
		{
			DateDisplay = DateDisplay.AddYears(-1);
		}

		/// <summary>
		/// Is specified date is weekend?
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		/// <returns>true if specified date is weekend; otherwise, false.</returns>
		public virtual bool IsWeekend(DateTime displayedDate)
		{
			var day_of_week = culture.DateTimeFormat.Calendar.GetDayOfWeek(displayedDate);
			return day_of_week == DayOfWeek.Saturday || day_of_week == DayOfWeek.Sunday;
		}

		/// <summary>
		/// Is specified date is holiday?
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		/// <returns>true if specified date is holiday; otherwise, false.</returns>
		public virtual bool IsHoliday(DateTime displayedDate)
		{
			return false;
		}

		/// <summary>
		/// Is dates belongs to same month?
		/// </summary>
		/// <param name="date1">First date.</param>
		/// <param name="date2">Second date.</param>
		/// <returns>true if dates belongs to same month; otherwise, false.</returns>
		public virtual bool IsSameMonth(DateTime date1, DateTime date2)
		{
			return culture.DateTimeFormat.Calendar.GetYear(date1) == culture.DateTimeFormat.Calendar.GetYear(date2)
				&& culture.DateTimeFormat.Calendar.GetMonth(date1) == culture.DateTimeFormat.Calendar.GetMonth(date2);
		}

		/// <summary>
		/// Is dates belongs to same day?
		/// </summary>
		/// <param name="date1">First date.</param>
		/// <param name="date2">Second date.</param>
		/// <returns>true if dates is same day; otherwise, false.</returns>
		public virtual bool IsSameDay(DateTime date1, DateTime date2)
		{
			return date1.Date == date2.Date;
		}

		/// <summary>
		/// Set calendar type.
		/// </summary>
		/// <param name="calendar">Calendar type.</param>
		public void SetCalendar(System.Globalization.Calendar calendar)
		{
			culture.DateTimeFormat.Calendar = calendar;
			UpdateCalendar();
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			calendarDateTemplate.SetStyle(style.Calendar, style);
			Days.ForEach(x => x.SetStyle(style.Calendar, style));
			Cache.ForEach(x => x.SetStyle(style.Calendar, style));

			calendarDayOfWeekTemplate.SetStyle(style.Calendar, style);
			Header.ForEach(x => x.SetStyle(style.Calendar, style));

			style.Calendar.PrevMonth.ApplyTo(transform.Find("MonthToggle/PrevMonth"));
			style.Calendar.NextMonth.ApplyTo(transform.Find("MonthToggle/NextMonth"));

			return true;
		}
		#endregion
	}
}