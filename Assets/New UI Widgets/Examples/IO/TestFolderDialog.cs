namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Test FolderDialog.
	/// </summary>
	public class TestFolderDialog : MonoBehaviour
	{
		/// <summary>
		/// FolderDialog template.
		/// </summary>
		[SerializeField]
		protected FolderDialog PickerTemplate;

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
		protected string CurrentValue = string.Empty;

		/// <summary>
		/// Show picker and log selected value.
		/// </summary>
		public void Test()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// show picker
			picker.Show(CurrentValue, ValueSelected, Canceled);
		}

		void ValueSelected(string value)
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

			// show picker
			picker.Show(CurrentValue, ShowValueSelected, ShowCanceled);
		}

		void ShowValueSelected(string value)
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