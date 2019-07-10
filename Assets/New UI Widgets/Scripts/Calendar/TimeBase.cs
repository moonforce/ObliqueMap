namespace UIWidgets
{
	using System;
	using System.Collections;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for Time widget.
	/// </summary>
	[DataBindSupport]
	public abstract class TimeBase : MonoBehaviour, IStylable
	{
		/// <summary>
		/// The time as text.
		/// </summary>
		[SerializeField]
		protected string timeText = DateTime.Now.TimeOfDay.ToString();

		/// <summary>
		/// The minumum time as text.
		/// </summary>
		[SerializeField]
		protected string timeMinText = "00:00:00";

		/// <summary>
		/// The maximum time as text.
		/// </summary>
		[SerializeField]
		protected string timeMaxText = "23:59:59";

		/// <summary>
		/// The time.
		/// </summary>
		[SerializeField]
		protected TimeSpan time = DateTime.Now.TimeOfDay;

		/// <summary>
		/// The time.
		/// </summary>
		[DataBindField]
		public virtual TimeSpan Time
		{
			get
			{
				return time;
			}

			set
			{
				if (time == value)
				{
					return;
				}

				time = value;

				if (time.Ticks < 0)
				{
					time += new TimeSpan(1, 0, 0, 0);
				}

				if (time.Days > 0)
				{
					time -= new TimeSpan(time.Days, 0, 0, 0);
				}

				time = Clamp(time);

				UpdateInputs();
				OnTimeChanged.Invoke(time);
			}
		}

		/// <summary>
		/// The minimum time.
		/// </summary>
		[SerializeField]
		protected TimeSpan timeMin = new TimeSpan(0, 0, 0);

		/// <summary>
		/// The minimum time.
		/// </summary>
		/// <value>The time minimum.</value>
		public TimeSpan TimeMin
		{
			get
			{
				return timeMin;
			}

			set
			{
				timeMin = value;
				Time = Clamp(Time);
			}
		}

		/// <summary>
		/// The maximum time.
		/// </summary>
		[SerializeField]
		protected TimeSpan timeMax = new TimeSpan(23, 59, 59);

		/// <summary>
		/// The maximum time.
		/// </summary>
		/// <value>The time max.</value>
		public TimeSpan TimeMax
		{
			get
			{
				return timeMax;
			}

			set
			{
				timeMax = value;
				Time = Clamp(Time);
			}
		}

		/// <summary>
		/// The input field proxy for the hours.
		/// </summary>
		protected IInputFieldProxy InputProxyHours;

		/// <summary>
		/// The input field proxy for the minutes.
		/// </summary>
		protected IInputFieldProxy InputProxyMinutes;

		/// <summary>
		/// The input field proxy for the seconds.
		/// </summary>
		protected IInputFieldProxy InputProxySeconds;

		/// <summary>
		/// The button to increase hours.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonHoursIncrease;

		/// <summary>
		/// The button to decrease hours.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonHoursDecrease;

		/// <summary>
		/// The button to increase minutes.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonMinutesIncrease;

		/// <summary>
		/// The button to decrease minutes.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonMinutesDecrease;

		/// <summary>
		/// The button to increase seconds.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonSecondsIncrease;

		/// <summary>
		/// The button to decrease seconds.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonSecondsDecrease;

		/// <summary>
		/// Allow changing value during hold.
		/// </summary>
		[SerializeField]
		public bool AllowHold = true;

		/// <summary>
		/// Delay of hold in seconds for permanent increase/descrease value.
		/// </summary>
		[SerializeField]
		public float HoldStartDelay = 0.5f;

		/// <summary>
		/// Delay of hold in seconds between increase/descrease value.
		/// </summary>
		[SerializeField]
		public float HoldChangeDelay = 0.1f;

		IEnumerator HoldCoroutine;

		/// <summary>
		/// The OnTimeChanged event.
		/// </summary>
		[DataBindEvent("Time")]
		public TimeEvent OnTimeChanged = new TimeEvent();

		bool isInited = false;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

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

			if (!TimeSpan.TryParse(timeText, out time))
			{
				Debug.LogWarning("TimeText cannot be parsed.");
			}

			if (!TimeSpan.TryParse(timeMinText, out timeMin))
			{
				Debug.LogWarning("TimeMinText cannot be parsed.");
			}

			if (!TimeSpan.TryParse(timeMaxText, out timeMax))
			{
				Debug.LogWarning("TimeMaxText cannot be parsed.");
			}

			InitInput();

			UpdateInputs();

			AddListeners();
		}

		/// <summary>
		/// Init the input.
		/// </summary>
		protected abstract void InitInput();

		/// <summary>
		/// Clamp the specified time.
		/// </summary>
		/// <param name="t">Time.</param>
		/// <returns>Clamped time.</returns>
		protected virtual TimeSpan Clamp(TimeSpan t)
		{
			if (t < timeMin)
			{
				t = timeMin;
			}

			if (t > timeMax)
			{
				t = timeMax;
			}

			return t;
		}

		/// <summary>
		/// Add the listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			if (InputProxyHours != null)
			{
				InputProxyHours.onEndEdit.AddListener(UpdateHours);
			}

			if (InputProxyMinutes != null)
			{
				InputProxyMinutes.onEndEdit.AddListener(UpdateMinutes);
			}

			if (InputProxySeconds != null)
			{
				InputProxySeconds.onEndEdit.AddListener(UpdateSeconds);
			}

			if (ButtonHoursIncrease != null)
			{
				ButtonHoursIncrease.onClick.AddListener(HoursIncrease);
				ButtonHoursIncrease.onPointerDown.AddListener(ButtonHoursIncreaseDown);
				ButtonHoursIncrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonHoursDecrease != null)
			{
				ButtonHoursDecrease.onClick.AddListener(HoursDecrease);
				ButtonHoursDecrease.onPointerDown.AddListener(ButtonHoursDecreaseDown);
				ButtonHoursDecrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonMinutesIncrease != null)
			{
				ButtonMinutesIncrease.onClick.AddListener(MinutesIncrease);
				ButtonMinutesIncrease.onPointerDown.AddListener(ButtonMinutesIncreaseDown);
				ButtonMinutesIncrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonMinutesDecrease != null)
			{
				ButtonMinutesDecrease.onClick.AddListener(MinutesDecrease);
				ButtonMinutesDecrease.onPointerDown.AddListener(ButtonMinutesDecreaseDown);
				ButtonMinutesDecrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonSecondsIncrease != null)
			{
				ButtonSecondsIncrease.onClick.AddListener(SecondsIncrease);
				ButtonSecondsIncrease.onPointerDown.AddListener(ButtonSecondsIncreaseDown);
				ButtonSecondsIncrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonSecondsDecrease != null)
			{
				ButtonSecondsDecrease.onClick.AddListener(SecondsDecrease);
				ButtonSecondsDecrease.onPointerDown.AddListener(ButtonSecondsDecreaseDown);
				ButtonSecondsDecrease.onPointerUp.AddListener(ButtonUp);
			}
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			if (InputProxyHours != null)
			{
				InputProxyHours.onEndEdit.RemoveListener(UpdateHours);
			}

			if (InputProxyMinutes != null)
			{
				InputProxyMinutes.onEndEdit.RemoveListener(UpdateMinutes);
			}

			if (InputProxySeconds != null)
			{
				InputProxySeconds.onEndEdit.RemoveListener(UpdateSeconds);
			}

			if (ButtonHoursIncrease != null)
			{
				ButtonHoursIncrease.onClick.RemoveListener(HoursIncrease);
				ButtonHoursIncrease.onPointerDown.RemoveListener(ButtonHoursIncreaseDown);
				ButtonHoursIncrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonHoursDecrease != null)
			{
				ButtonHoursDecrease.onClick.RemoveListener(HoursDecrease);
				ButtonHoursDecrease.onPointerDown.RemoveListener(ButtonHoursDecreaseDown);
				ButtonHoursDecrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonMinutesIncrease != null)
			{
				ButtonMinutesIncrease.onClick.RemoveListener(MinutesIncrease);
				ButtonMinutesIncrease.onPointerDown.RemoveListener(ButtonMinutesIncreaseDown);
				ButtonMinutesIncrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonMinutesDecrease != null)
			{
				ButtonMinutesDecrease.onClick.RemoveListener(MinutesDecrease);
				ButtonMinutesDecrease.onPointerDown.RemoveListener(ButtonMinutesDecreaseDown);
				ButtonMinutesDecrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonSecondsIncrease != null)
			{
				ButtonSecondsIncrease.onClick.RemoveListener(SecondsIncrease);
				ButtonSecondsIncrease.onPointerDown.RemoveListener(ButtonSecondsIncreaseDown);
				ButtonSecondsIncrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonSecondsDecrease != null)
			{
				ButtonSecondsDecrease.onClick.RemoveListener(SecondsDecrease);
				ButtonSecondsDecrease.onPointerDown.RemoveListener(ButtonSecondsDecreaseDown);
				ButtonSecondsDecrease.onPointerUp.RemoveListener(ButtonUp);
			}
		}

		/// <summary>
		/// Updates the hours.
		/// </summary>
		/// <param name="hours">Hours.</param>
		protected virtual void UpdateHours(string hours)
		{
			var h = 0;
			if (int.TryParse(hours, out h))
			{
				Time += new TimeSpan(Mathf.Abs(h) - time.Hours, 0, 0);
			}
			else
			{
				UpdateInputs();
			}
		}

		/// <summary>
		/// Updates the minutes.
		/// </summary>
		/// <param name="minutes">Minutes.</param>
		protected virtual void UpdateMinutes(string minutes)
		{
			var m = 0;
			if (int.TryParse(minutes, out m))
			{
				Time += new TimeSpan(0, Mathf.Abs(m) - time.Minutes, 0);
			}
			else
			{
				UpdateInputs();
			}
		}

		/// <summary>
		/// Updates the seconds.
		/// </summary>
		/// <param name="seconds">Seconds.</param>
		protected virtual void UpdateSeconds(string seconds)
		{
			var s = 0;
			if (int.TryParse(seconds, out s))
			{
				Time += new TimeSpan(0, 0, Mathf.Abs(s) - time.Seconds);
			}
			else
			{
				UpdateInputs();
			}
		}

		/// <summary>
		/// Increase the hours.
		/// </summary>
		public void HoursIncrease()
		{
			Time += new TimeSpan(1, 0, 0);
		}

		/// <summary>
		/// Decrease the hours.
		/// </summary>
		public void HoursDecrease()
		{
			Time -= new TimeSpan(1, 0, 0);
		}

		/// <summary>
		/// Increase the minutes.
		/// </summary>
		public void MinutesIncrease()
		{
			Time += new TimeSpan(0, 1, 0);
		}

		/// <summary>
		/// Decrease the minutes.
		/// </summary>
		public void MinutesDecrease()
		{
			Time -= new TimeSpan(0, 1, 0);
		}

		/// <summary>
		/// Increase the seconds.
		/// </summary>
		public void SecondsIncrease()
		{
			Time += new TimeSpan(0, 0, 1);
		}

		/// <summary>
		/// Decrease the seconds.
		/// </summary>
		public void SecondsDecrease()
		{
			Time -= new TimeSpan(0, 0, 1);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		public virtual void UpdateInputs()
		{
			if (InputProxyHours != null)
			{
				InputProxyHours.text = Time.Hours.ToString("D2");
			}

			if (InputProxyMinutes != null)
			{
				InputProxyMinutes.text = Time.Minutes.ToString("D2");
			}

			if (InputProxySeconds != null)
			{
				InputProxySeconds.text = Time.Seconds.ToString("D2");
			}
		}

		#region Hold

		/// <summary>
		/// Create hold coroutine.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <returns>Hold coroutine.</returns>
		protected virtual IEnumerator HoldCreateCoroutine(Action action)
		{
			if (AllowHold)
			{
				yield return new WaitForSeconds(HoldStartDelay);
				while (AllowHold)
				{
					action();
					yield return new WaitForSeconds(HoldChangeDelay);
				}
			}
		}

		/// <summary>
		/// Start hold process.
		/// </summary>
		/// <param name="action">Action.</param>
		protected virtual void HoldStart(Action action)
		{
			HoldStop();
			HoldCoroutine = HoldCreateCoroutine(action);
			StartCoroutine(HoldCoroutine);
		}

		/// <summary>
		/// Stop hold process.
		/// </summary>
		protected virtual void HoldStop()
		{
			if (HoldCoroutine != null)
			{
				StopCoroutine(HoldCoroutine);
			}
		}

		/// <summary>
		/// Process pointer up event on all buttons.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonUp(PointerEventData eventData)
		{
			HoldStop();
		}

		/// <summary>
		/// Process pointer down event on hours increase button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonHoursIncreaseDown(PointerEventData eventData)
		{
			HoldStart(HoursIncrease);
		}

		/// <summary>
		/// Process pointer down event on hours decrease button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonHoursDecreaseDown(PointerEventData eventData)
		{
			HoldStart(HoursDecrease);
		}

		/// <summary>
		/// Process pointer down event on minutes increase button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonMinutesIncreaseDown(PointerEventData eventData)
		{
			HoldStart(MinutesIncrease);
		}

		/// <summary>
		/// Process pointer down event on minutes decrease button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonMinutesDecreaseDown(PointerEventData eventData)
		{
			HoldStart(MinutesDecrease);
		}

		/// <summary>
		/// Process pointer down event on seconds increase button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonSecondsIncreaseDown(PointerEventData eventData)
		{
			HoldStart(SecondsIncrease);
		}

		/// <summary>
		/// Process pointer down event on seconds decrease button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonSecondsDecreaseDown(PointerEventData eventData)
		{
			HoldStart(SecondsDecrease);
		}
		#endregion

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			style.Time.ButtonIncrease.ApplyTo(ButtonHoursIncrease);
			style.Time.ButtonDecrease.ApplyTo(ButtonHoursDecrease);

			style.Time.ButtonIncrease.ApplyTo(ButtonMinutesIncrease);
			style.Time.ButtonDecrease.ApplyTo(ButtonMinutesDecrease);

			style.Time.ButtonIncrease.ApplyTo(ButtonSecondsIncrease);
			style.Time.ButtonDecrease.ApplyTo(ButtonSecondsDecrease);

			return true;
		}
		#endregion
	}
}