namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Autocomplete test.
	/// </summary>
	public class AutocompleteTest : MonoBehaviour
	{
		/// <summary>
		/// Autocomplete InputField.
		/// </summary>
		[SerializeField]
		public InputField AutocompleteInputField;

		/// <summary>
		/// Get InputField text.
		/// </summary>
		public void GetText()
		{
			var text = AutocompleteInputField.text;

			// do something with text
			Debug.Log("AutocompleteInputField.text = " + text);
		}

		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public Autocomplete Autocomplete;

		/// <summary>
		/// Adds listeners.
		/// </summary>
		protected virtual void Start()
		{
			// OptionSelected will be called when user select value
			Autocomplete.OnOptionSelected.AddListener(OptionSelected);
		}

		/// <summary>
		/// Process selected text.
		/// </summary>
		/// <param name="text">Selected text.</param>
		protected void OptionSelected(string text)
		{
			// do something with text
			Debug.Log("Autocomplete selected value = " + text);
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Autocomplete.OnOptionSelected.RemoveListener(OptionSelected);
		}
	}
}