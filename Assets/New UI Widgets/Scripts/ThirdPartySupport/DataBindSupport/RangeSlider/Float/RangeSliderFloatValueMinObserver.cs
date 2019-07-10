#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the ValueMin of an RangeSliderFloat.
	/// </summary>
	public class RangeSliderFloatValueMinObserver : ComponentDataObserver<UIWidgets.RangeSliderFloat, System.Single>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.RangeSliderFloat target)
		{
			target.OnValuesChange.AddListener(OnValuesChangeRangeSliderFloat);
		}

		/// <inheritdoc />
		protected override System.Single GetValue(UIWidgets.RangeSliderFloat target)
		{
			return target.ValueMin;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.RangeSliderFloat target)
		{
			target.OnValuesChange.RemoveListener(OnValuesChangeRangeSliderFloat);
		}
		
		void OnValuesChangeRangeSliderFloat(System.Single arg0, System.Single arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif