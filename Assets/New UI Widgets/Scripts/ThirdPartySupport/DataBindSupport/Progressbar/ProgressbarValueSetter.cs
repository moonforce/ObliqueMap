#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the Value of a Progressbar depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Progressbar Value Setter")]
	public class ProgressbarValueSetter : ComponentSingleSetter<UIWidgets.Progressbar, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Progressbar target, System.Int32 value)
		{
			target.Value = value;
		}
	}
}
#endif