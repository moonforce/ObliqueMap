namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Test PickerBool.
	/// </summary>
	public class TestPickerBool : MonoBehaviour
	{
		/// <summary>
		/// PickerBool template.
		/// </summary>
		[SerializeField]
		protected PickerBool PickerTemplate;

		/// <summary>
		/// Text component to display selected value.
		/// </summary>
		[SerializeField]
		protected Text Info;

		/// <summary>
		/// Current value.
		/// </summary>
		[NonSerialized]
		[FormerlySerializedAs("currentValue")]
		protected bool CurrentValue;

		/// <summary>
		/// Show picker and log selected value.
		/// </summary>
		public void Test()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			picker.SetMessage("Confirmation text");

			// show picker
			picker.Show(CurrentValue, ValueSelected, Canceled);
		}

		void ValueSelected(bool value)
		{
			CurrentValue = value;
			Debug.Log("value: " + value);
		}

		void Canceled()
		{
			Debug.Log("canceled");
		}

		/// <summary>
		/// Show picker and display selected value.
		/// </summary>
		public void TestShow()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			picker.SetMessage("Confirmation text");

			// show picker
			picker.Show(CurrentValue, ShowValueSelected, ShowCanceled);
		}

		void ShowValueSelected(bool value)
		{
			CurrentValue = value;
			Info.text = "Value: " + value;
		}

		void ShowCanceled()
		{
			Info.text = "Canceled";
		}
	}
}