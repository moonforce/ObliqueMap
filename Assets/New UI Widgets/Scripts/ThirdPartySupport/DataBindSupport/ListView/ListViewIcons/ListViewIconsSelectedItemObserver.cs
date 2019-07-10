#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an ListViewIcons.
	/// </summary>
	public class ListViewIconsSelectedItemObserver : ComponentDataObserver<UIWidgets.ListViewIcons, UIWidgets.ListViewIconsItemDescription>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewIcons target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectListViewIcons);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewIcons);
		}

		/// <inheritdoc />
		protected override UIWidgets.ListViewIconsItemDescription GetValue(UIWidgets.ListViewIcons target)
		{
			return target.SelectedItem;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewIcons target)
		{
			target.OnSelectObject.RemoveListener(OnSelectObjectListViewIcons);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewIcons);
		}
		
		void OnSelectObjectListViewIcons(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewIcons(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif