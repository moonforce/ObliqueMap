#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItems of an ListView.
	/// </summary>
	public class ListViewSelectedItemsObserver : ComponentDataObserver<UIWidgets.ListView, System.Collections.Generic.List<System.String>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListView target)
		{
			target.OnSelectString.AddListener(OnSelectStringListView);
			target.OnDeselectString.AddListener(OnDeselectStringListView);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<System.String> GetValue(UIWidgets.ListView target)
		{
			return target.SelectedItems;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListView target)
		{
			target.OnSelectString.RemoveListener(OnSelectStringListView);
			target.OnDeselectString.RemoveListener(OnDeselectStringListView);
		}
		
		void OnSelectStringListView(System.Int32 arg0, System.String arg1)
		{
			OnTargetValueChanged();
		}

		void OnDeselectStringListView(System.Int32 arg0, System.String arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif