namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test FileDialog.
	/// </summary>
	public class TestFileDialog : MonoBehaviour
	{
		/// <summary>
		/// FileDialog template.
		/// </summary>
		[SerializeField]
		protected FileDialog PickerTemplate;

		/// <summary>
		/// Text component to display selected value.
		/// </summary>
		[SerializeField]
		protected Text Info;

		string currentValue = string.Empty;

		/// <summary>
		/// Show picker and log selected value.
		/// </summary>
		public void Test()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// show picker
			picker.Show(currentValue, ValueSelected, Canceled);
		}

		void ValueSelected(string value)
		{
			currentValue = value;
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

			// show picker
			picker.Show(currentValue, ShowValueSelected, ShowCanceled);
		}

		void ShowValueSelected(string value)
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