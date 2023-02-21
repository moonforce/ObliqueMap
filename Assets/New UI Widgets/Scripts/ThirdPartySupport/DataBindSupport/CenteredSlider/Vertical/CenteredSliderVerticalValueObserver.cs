#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Value of an CenteredSliderVertical.
	/// </summary>
	public class CenteredSliderVerticalValueObserver : ComponentDataObserver<UIWidgets.CenteredSliderVertical, System.Int32>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.CenteredSliderVertical target)
		{
			target.OnValuesChange.AddListener(OnValuesChangeCenteredSliderVertical);
		}

		/// <inheritdoc />
		protected override System.Int32 GetValue(UIWidgets.CenteredSliderVertical target)
		{
			return target.Value;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.CenteredSliderVertical target)
		{
			target.OnValuesChange.RemoveListener(OnValuesChangeCenteredSliderVertical);
		}
		
		void OnValuesChangeCenteredSliderVertical(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif