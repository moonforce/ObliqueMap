namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// DateTime widget.
	/// </summary>
	[DataBindSupport]
	public class DateTimeWidget : MonoBehaviour
	{
		/// <summary>
		/// The calendar widget.
		/// </summary>
		[SerializeField]
		protected CalendarBase calendar;

		/// <summary>
		/// The calendar widget.
		/// </summary>
		/// <value>The calendar.</value>
		public CalendarBase Calendar
		{
			get
			{
				return calendar;
			}

			set
			{
				RemoveListeners();
				calendar = value;
				AddListeners();
			}
		}

		/// <summary>
		/// The time widget.
		/// </summary>
		[SerializeField]
		protected TimeBase time;

		/// <summary>
		/// The time widget.
		/// </summary>
		/// <value>The time.</value>
		public TimeBase Time
		{
			get
			{
				return time;
			}

			set
			{
				RemoveListeners();
				time = value;
				AddListeners();
			}
		}

		/// <summary>
		/// The current DateTime.
		/// </summary>
		[SerializeField]
		protected DateTime dateTime;

		/// <summary>
		/// The current DateTime.
		/// </summary>
		/// <value>The date time.</value>
		[DataBindField]
		public DateTime DateTime
		{
			get
			{
				return dateTime;
			}

			set
			{
				dateTime = value;

				if (dateTime != value)
				{
					UpdateWidgets();

					OnDateTimeChanged.Invoke(dateTime);
				}
			}
		}

		/// <summary>
		/// The current date time.
		/// </summary>
		[SerializeField]
		protected string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

		/// <summary>
		/// The format.
		/// </summary>
		[SerializeField]
		protected string format = "yyyy-MM-dd HH:mm:ss";

		/// <summary>
		/// Event called when datetime changed.
		/// </summary>
		[DataBindEvent("DateTime")]
		public CalendarDateEvent OnDateTimeChanged = new CalendarDateEvent();

		bool isInited = false;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			dateTime = DateTime.ParseExact(currentDateTime, format, calendar.Culture);

			UpdateWidgets();

			AddListeners();
		}

		/// <summary>
		/// Updates the widgets.
		/// </summary>
		protected virtual void UpdateWidgets()
		{
			calendar.Date = dateTime;
			time.Time = dateTime.TimeOfDay;
		}

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			calendar.OnDateChanged.AddListener(DateChanged);
			time.OnTimeChanged.AddListener(TimeChanged);
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			calendar.OnDateChanged.RemoveListener(DateChanged);
			time.OnTimeChanged.RemoveListener(TimeChanged);
		}

		/// <summary>
		/// Process changed date.
		/// </summary>
		/// <param name="d">Date.</param>
		protected virtual void DateChanged(DateTime d)
		{
			var dt = d;
			dt += time.Time - dt.TimeOfDay;
			DateTime = dt;
		}

		/// <summary>
		/// Process changed time.
		/// </summary>
		/// <param name="t">Time.</param>
		protected virtual void TimeChanged(TimeSpan t)
		{
			var dt = calendar.Date;
			dt += t - dt.TimeOfDay;
			DateTime = dt;
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}
	}
}