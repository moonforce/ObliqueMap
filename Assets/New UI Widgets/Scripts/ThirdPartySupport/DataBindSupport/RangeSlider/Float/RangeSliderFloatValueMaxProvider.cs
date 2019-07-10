#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;
	
	/// <summary>
	/// Provides the ValueMax of an RangeSliderFloat.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] RangeSliderFloat ValueMax Provider")]
	public class RangeSliderFloatValueMaxProvider : ComponentDataProvider<UIWidgets.RangeSliderFloat, System.Single>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.RangeSliderFloat target)
		{
			target.OnValuesChange.AddListener(OnValuesChangeRangeSliderFloat);
		}

		/// <inheritdoc />
		protected override System.Single GetValue(UIWidgets.RangeSliderFloat target)
		{
			return target.ValueMax;
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