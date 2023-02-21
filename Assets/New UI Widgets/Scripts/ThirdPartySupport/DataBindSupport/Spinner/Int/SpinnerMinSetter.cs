#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the Min of a Spinner depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Spinner Min Setter")]
	public class SpinnerMinSetter : ComponentSingleSetter<UIWidgets.Spinner, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Spinner target, System.Int32 value)
		{
			target.Min = value;
		}
	}
}
#endif