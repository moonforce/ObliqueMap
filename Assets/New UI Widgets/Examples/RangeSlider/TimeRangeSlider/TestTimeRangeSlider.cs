namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Test TimeRangeSlider.
	/// </summary>
	[RequireComponent(typeof(TimeRangeSlider))]
	public class TestTimeRangeSlider : MonoBehaviour
	{
		/// <summary>
		/// Text component to display start time.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("startText")]
		protected Text StartText;

		/// <summary>
		/// Text component to display end time.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("endText")]
		protected Text EndText;

		/// <summary>
		/// Time format.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("format")]
		protected string Format = "hh:mm tt";

		/// <summary>
		/// Current slider.
		/// </summary>
		[HideInInspector]
		protected TimeRangeSlider Slider;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Slider = GetComponent<TimeRangeSlider>();
			if (Slider != null)
			{
				Slider.OnChange.AddListener(SliderChanged);

				SetRange1();

				Slider.StartTime = Slider.MinTime;
				Slider.EndTime = Slider.MaxTime;

				SliderChanged(Slider.StartTime, Slider.EndTime);
			}
		}

		/// <summary>
		/// Set slider range.
		/// </summary>
		public void SetRange1()
		{
			Slider.MinTime = new DateTime(2017, 1, 1, 9, 0, 0);
			Slider.MaxTime = new DateTime(2017, 1, 1, 18, 0, 0);

			SliderChanged(Slider.StartTime, Slider.EndTime);
		}

		/// <summary>
		/// Set another range.
		/// </summary>
		public void SetRange2()
		{
			Slider.MinTime = new DateTime(2017, 1, 1, 12, 0, 0);
			Slider.MaxTime = new DateTime(2017, 1, 1, 15, 0, 0);

			SliderChanged(Slider.StartTime, Slider.EndTime);
		}

		/// <summary>
		/// Handle slider value changed event.
		/// </summary>
		/// <param name="start">Start time.</param>
		/// <param name="end">End time.</param>
		protected virtual void SliderChanged(DateTime start, DateTime end)
		{
			if (StartText != null)
			{
				StartText.text = start.ToString(Format);
			}

			if (EndText != null)
			{
				EndText.text = end.ToString(Format);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Slider != null)
			{
				Slider.OnChange.RemoveListener(SliderChanged);
			}
		}
	}
}