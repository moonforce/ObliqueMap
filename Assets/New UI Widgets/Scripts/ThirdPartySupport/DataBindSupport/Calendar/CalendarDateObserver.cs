#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Date of an Calendar.
	/// </summary>
	public class CalendarDateObserver : ComponentDataObserver<UIWidgets.Calendar, System.DateTime>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.Calendar target)
		{
			target.OnDateChanged.AddListener(OnDateChangedCalendar);
		}

		/// <inheritdoc />
		protected override System.DateTime GetValue(UIWidgets.Calendar target)
		{
			return target.Date;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.Calendar target)
		{
			target.OnDateChanged.RemoveListener(OnDateChangedCalendar);
		}
		
		void OnDateChangedCalendar(System.DateTime arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif