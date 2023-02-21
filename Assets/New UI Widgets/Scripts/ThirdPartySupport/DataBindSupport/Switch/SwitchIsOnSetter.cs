#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the IsOn of a Switch depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Switch IsOn Setter")]
	public class SwitchIsOnSetter : ComponentSingleSetter<UIWidgets.Switch, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Switch target, System.Boolean value)
		{
			target.IsOn = value;
		}
	}
}
#endif