#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the DataSource of a ListView depending on the UIWidgets.ObservableList<System.String> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListView DataSource Setter")]
	public class ListViewDataSourceSetter : ComponentSingleSetter<UIWidgets.ListView, UIWidgets.ObservableList<System.String>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListView target, UIWidgets.ObservableList<System.String> value)
		{
			target.DataSource = value;
		}
	}
}
#endif