namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// How to use RangeSliderFloat.
	/// </summary>
	[RequireComponent(typeof(RangeSliderFloat))]
	public class RangeSliderFloatSample : MonoBehaviour
	{
		/// <summary>
		/// Text component to display RangeSlder values.
		/// </summary>
		[SerializeField]
		protected Text Text;

		RangeSliderFloat slider;

		/// <summary>
		/// Adds listeners.
		/// </summary>
		protected virtual void Start()
		{
			slider = GetComponent<RangeSliderFloat>();
			if (slider != null)
			{
				slider.OnValuesChange.AddListener(SliderChanged);
				SliderChanged(slider.ValueMin, slider.ValueMax);
			}
		}

		/// <summary>
		/// Handle changed values.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		protected virtual void SliderChanged(float min, float max)
		{
			if (Text != null)
			{
				if (slider.WholeNumberOfSteps)
				{
					Text.text = string.Format("Range: {0:000.00} - {1:000.00}; Step: {2:0.00}", min, max, slider.Step);
				}
				else
				{
					Text.text = string.Format("Range: {0:000.00} - {1:000.00}", min, max);
				}
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (slider != null)
			{
				slider.OnValuesChange.RemoveListener(SliderChanged);
			}
		}
	}
}