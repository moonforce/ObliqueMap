#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	public class AutocompleteTMPro : Autocomplete
	{
		/// <summary>
		/// TMPro InputField.
		/// </summary>
		[SerializeField]
		protected TMP_InputField InputFieldTMPro;

		/// <summary>
		/// Gets the InputFieldProxy.
		/// </summary>
		protected override IInputFieldProxy InputFieldProxy
		{
			get
			{
				if (inputFieldProxy == null)
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