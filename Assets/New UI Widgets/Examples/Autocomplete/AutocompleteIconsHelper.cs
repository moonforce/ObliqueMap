namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// AutocompleteIconsHelper.
	/// How to use AutocompleteIcons to select one item.
	/// </summary>
	public class AutocompleteIconsHelper : MonoBehaviour
	{
		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public AutocompleteIcons Autocomplete;

		/// <summary>
		/// InputField.
		/// </summary>
		[SerializeField]
		public InputField Input;

		[SerializeField]
		ListViewIconsItemDescription item;

		/// <summary>
		/// Selected item.
		/// </summary>
		public ListViewIconsItemDescription Item
		{
			get
			{
				return item;
			}

			set
			{
				Input.text = item.Name;
				item = value;
			}
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		protected virtual void Start()
		{
			Input.onEndEdit.AddListener(ResetItem);
			Autocomplete.OnOptionSelectedItem.AddListener(SetItem);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Input.onEndEdit.RemoveListener(ResetItem);
			Autocomplete.OnOptionSelectedItem.RemoveListener(SetItem);
		}

		/// <summary>
		/// Reset item.
		/// </summary>
		/// <param name="str">Input string.</param>
		protected virtual void ResetItem(string str)
		{
			item = null;
		}

		/// <summary>
		/// Set item.
		/// </summary>
		/// <param name="it">Item.</param>
		protected virtual void SetItem(ListViewIconsItemDescription it)
		{
			item = Autocomplete.TargetListView.DataSource[0];
			Autocomplete.TargetListView.DataSource.Clear();
		}
	}
}