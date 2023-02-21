#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the LimitMin of a RangeSliderFloat depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSliderFloat LimitMin Setter")]
	public class RangeSliderFloatLimitMinSetter : ComponentSingleSetter<UIWidgets.RangeSliderFloat, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSliderFloat target, System.Single value)
		{
			target.LimitMin = value;
		}
	}
}
#endif