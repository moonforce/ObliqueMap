#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the ValueMax of an RangeSlider.
	/// </summary>
	public class RangeSliderValueMaxObserver : ComponentDataObserver<UIWidgets.RangeSlider, System.Int32>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.RangeSlider target)
		{
			target.OnValuesChange.AddListener(OnValuesChangeRangeSlider);
		}

		/// <inheritdoc />
		protected override System.Int32 GetValue(UIWidgets.RangeSlider target)
		{
			return target.ValueMax;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.RangeSlider target)
		{
			target.OnValuesChange.RemoveListener(OnValuesChangeRangeSlider);
		}
		
		void OnValuesChangeRangeSlider(System.Int32 arg0, System.Int32 arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif