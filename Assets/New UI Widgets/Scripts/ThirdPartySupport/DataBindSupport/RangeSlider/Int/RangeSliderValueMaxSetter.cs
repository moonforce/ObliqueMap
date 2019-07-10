#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the ValueMax of a RangeSlider depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSlider ValueMax Setter")]
	public class RangeSliderValueMaxSetter : ComponentSingleSetter<UIWidgets.RangeSlider, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSlider target, System.Int32 value)
		{
			target.ValueMax = value;
		}
	}
}
#endif