#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	/// <typeparam name="TListViewComponent">Type of ListView.DefaultItem.</typeparam>
	/// <typeparam name="TListView">Type of ListView.</typeparam>
	public abstract class AutocompleteCustomTMPro<TValue, TListViewComponent, TListView> : AutocompleteCustom<TValue, TListViewComponent, TListView>
		where TListView : ListViewCustom<TListViewComponent, TValue>
		where TListViewComponent : ListViewItem
	{
		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("InputFieldTMPro")]
		protected TMP_InputField inputFieldTMPro;

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		public TMP_InputField InputFieldTMPro
		{
			get
			{
				return inputFieldTMPro;
			}

			set
			{
				inputFieldTMPro = value;
				inputFieldProxy = null;
				InitInputField();
			}
		}

		/// <summary>
		/// Gets the InputFieldProxy.
		/// </summary>
		protected override IInputFieldProxy InputFieldProxy
		{
			get
			{
				if ((inputFieldProxy == null) && (InputFieldTMPro != null))
				{
					inputFieldProxy = new InputFieldTMProProxy(InputFieldTMPro);
				}

				return inputFieldProxy;
			}
		}

#region IStylable implementation

		/// <summary>
		/// Set InputField style.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected override void SetStyleInput(Style style)
		{
			if (InputFieldTMPro == null)
			{
				return;
			}

			style.Autocomplete.InputField.ApplyTo(InputFieldTMPro.textComponent.gameObject, true);
			if (InputFieldTMPro.placeholder != null)
			{
				style.Autocomplete.Placeholder.ApplyTo(InputFieldTMPro.placeholder.gameObject);
			}
		}
#endregion
	}
}
#endif