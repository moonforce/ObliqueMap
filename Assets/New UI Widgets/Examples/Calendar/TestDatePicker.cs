namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test DatePicker.
	/// </summary>
	public class TestDatePicker : MonoBehaviour
	{
		/// <summary>
		/// DatePicker template.
		/// </summary>
		[SerializeField]
		protected DatePicker PickerTemplate;

		/// <summary>
		/// Text component to display selected value.
		/// </summary>
		[SerializeField]
		protected Text Info;

		DateTime currentValue = DateTime.Today;

		/// <summary>
		/// Open picker and log selected value.
		/// </summary>
		public void Test()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// show picker
			picker.Show(currentValue, ValueSelected, Canceled);
		}

		void ValueSelected(DateTime value)
		{
			currentValue = value;
			Debug.Log("value: " + value);
		}

		void Canceled()
		{
			Debug.Log("canceled");
		}

		/// <summary>
		/// Open picker and display selected value.
		/// </summary>
		public void TestShow()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// show picker
			picker.Show(currentValue, ShowValueSelected, ShowCanceled);
		}

		void ShowValueSelected(DateTime value)
		{
			currentValue = value;
			Info.text = "Value: " + value;
		}

		void ShowCanceled()
		{
			Info.text = "Canceled";
		}
	}
}