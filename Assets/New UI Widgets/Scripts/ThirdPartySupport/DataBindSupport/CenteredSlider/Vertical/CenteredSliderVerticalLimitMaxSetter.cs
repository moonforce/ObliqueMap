#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the LimitMax of a CenteredSliderVertical depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CenteredSliderVertical LimitMax Setter")]
	public class CenteredSliderVerticalLimitMaxSetter : ComponentSingleSetter<UIWidgets.CenteredSliderVertical, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSliderVertical target, System.Int32 value)
		{
			target.LimitMax = value;
		}
	}
}
#endif