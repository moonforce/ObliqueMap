namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DatePicker.
	/// </summary>
	public class DatePicker : Picker<DateTime, DatePicker>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public CalendarBase Calendar;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(DateTime defaultValue)
		{
			Calendar.Date = defaultValue;

			Calendar.OnDateChanged.AddListener(DateSelected);
		}

		void DateSelected(DateTime date)
		{
			Selected(date);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			Calendar.OnDateChanged.RemoveListener(DateSelected);
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			Calendar.SetStyle(style);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}