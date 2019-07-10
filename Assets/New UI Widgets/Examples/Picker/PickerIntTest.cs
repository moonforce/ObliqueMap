namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test PickerInt.
	/// </summary>
	public class PickerIntTest : MonoBehaviour
	{
		/// <summary>
		/// PickerInt template.
		/// </summary>
		[SerializeField]
		protected PickerInt PickerTemplate;

		/// <summary>
		/// Text component to display selected value.
		/// </summary>
		[SerializeField]
		protected Text Info;

		int currentValue = 0;

		/// <summary>
		/// Run test.
		/// </summary>
		public void Test()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// set values from template
			picker.ListView.DataSource = PickerTemplate.ListView.DataSource.ToObservableList();

			// or set new values
			// picker.ListView.DataSource = Utilites.CreateList(100, x => x);

			// show picker
			picker.Show(currentValue, ValueSelected, Canceled);
		}

		void ValueSelected(int value)
		{
			currentValue = value;
			Debug.Log("value: " + value);
		}

		void Canceled()
		{
			Debug.Log("canceled");
		}

		/// <summary>
		/// Run test.
		/// </summary>
		public void TestShow()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// set values from template
			// picker.ListView.DataSource = PickerTemplate.ListView.DataSource.ToObservableList();
			// or set new values
			picker.ListView.DataSource = Utilites.CreateList(100, x => x);

			// show picker
			picker.Show(currentValue, ShowValueSelected, ShowCanceled);
		}

		void ShowValueSelected(int value)
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