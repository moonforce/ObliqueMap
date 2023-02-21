#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;
	
	/// <summary>
	/// Provides the SelectedItem of an ListViewInt.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewInt SelectedItem Provider")]
	public class ListViewIntSelectedItemProvider : ComponentDataProvider<UIWidgets.ListViewInt, System.Int32>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewInt target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectListViewInt);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewInt);
		}

		/// <inheritdoc />
		protected override System.Int32 GetValue(UIWidgets.ListViewInt target)
		{
			return target.SelectedItem;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewInt target)
		{
			target.OnSelectObject.RemoveListener(OnSelectObjectListViewInt);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewInt);
		}
		
		void OnSelectObjectListViewInt(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewInt(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif