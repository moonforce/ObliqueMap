namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Calendar.
	/// </summary>
	public class Calendar : CalendarBase
	{
		[SerializeField]
		Text dateText;

		/// <summary>
		/// Text to display current date.
		/// </summary>
		public Text DateText
		{
			get
			{
				return dateText;
			}

			set
			{
				dateText = value;

				UpdateDate();
			}
		}

		[SerializeField]
		Text monthText;

		/// <summary>
		/// Text to display current month.
		/// </summary>
		public Text MonthText
		{
			get
			{
				return monthText;
			}

			set
			{
				monthText = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Update displayed date and month.
		/// </summary>
		protected override void UpdateDate()
		{
			if (dateText != null)
			{
				dateText.text = Date.ToString("D", Culture);
			}

			if (monthText != null)
			{
				monthText.text = DateDisplay.ToString("Y", Culture);
			}
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

			style.Calendar.CurrentDate.ApplyTo(dateText);
			style.Calendar.CurrentMonth.ApplyTo(monthText);

			return true;
		}
		#endregion
	}
}