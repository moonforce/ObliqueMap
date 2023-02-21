namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// RangeSlider with label.
	/// </summary>
	[RequireComponent(typeof(RangeSlider))]
	public class RangleSliderLabel : MonoBehaviour
	{
		/// <summary>
		/// Text component to display min value.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("minLabel")]
		protected Text MinLabel;

		/// <summary>
		/// Text component to display max value.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("maxLabel")]
		protected Text MaxLabel;

		RangeSlider slider;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init and adds listeners.
		/// </summary>
		protected virtual void Init()
		{
			slider = GetComponent<RangeSlider>();
			if (slider != null)
			{
				slider.OnValuesChange.AddListener(ValueChanged);
				ValueChanged(slider.ValueMin, slider.ValueMax);
			}
		}

		/// <summary>
		/// Callback when slider value changed.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		protected virtual void ValueChanged(int min, int max)
		{
			MinLabel.text = min.ToString();
			MaxLabel.text = max.ToString();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (slider != null)
			{
				slider.OnValuesChange.RemoveListener(ValueChanged);
			}
		}
	}
}