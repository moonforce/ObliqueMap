#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// InputField with exposed functions to validate input.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/TMPro Input Field Proxy")]
	public class InputFieldTMProExtended : TMP_InputField, IInputFieldExtended
	{
		/// <summary>
		/// Function to validate input.
		/// </summary>
		public UnityEngine.UI.InputField.OnValidateInput Validation
		{
			get;
			set;
		}

		/// <summary>
		/// Function to process changed value.
		/// </summary>
		public UnityAction<string> ValueChanged
		{
			get;
			set;
		}

		/// <summary>
		/// Function to process end edit.
		/// </summary>
		public UnityAction<string> ValueEndEdit
		{
			get;
			set;
		}

		/// <summary>
		/// Current value.
		/// </summary>
		public string Value
		{
			get
			{
				return text;
			}

			set
			{
#if UNITY_5_3_4 || UNITY_5_3_OR_NEWER
				if (m_Text != value)
				{
					m_Text = value;
					UpdateLabel();
				}
#else
				text = value;
#endif
			}
		}

		/// <summary>
		/// Start selection position.
		/// </summary>
		public int SelectionStart
		{
			get
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				return Mathf.Min(selectionAnchorPosition, selectionFocusPosition);
#else
				return Mathf.Min(caretSelectPos, caretPosition);
#endif
			}
		}

		/// <summary>
		/// End selection position.
		/// </summary>
		public int SelectionEnd
		{
			get
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				return Mathf.Max(selectionAnchorPosition, selectionFocusPosition);
#else
				return Mathf.Max(caretSelectPos, caretPosition);
#endif
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (Validation == null)
			{
				Validation = (x, y, z) => z;
			}

			if (ValueChanged == null)
			{
				ValueChanged = x => { };
			}

			if (ValueEndEdit == null)
			{
				ValueEndEdit = x => { };
			}

			onValidateInput = ProcessValidation;

			onValueChanged.AddListener(ProcessValueChanged);
			onEndEdit.AddListener(ProcessValueEndEdit);
		}

		char ProcessValidation(string validateText, int charIndex, char addedChar)
		{
			return Validation(validateText, charIndex, addedChar);
		}

		void ProcessValueChanged(string text)
		{
			ValueChanged(text);
		}

		void ProcessValueEndEdit(string text)
		{
			ValueEndEdit(text);
		}

		/// <summary>
		/// Ons the destroy.
		/// </summary>
		protected virtual void onDestroy()
		{
			onValueChanged.RemoveListener(ProcessValueChanged);
			onEndEdit.RemoveListener(ProcessValueEndEdit);
		}

		/// <summary>
		/// Set the specified style for specified spinner.
		/// </summary>
		/// <param name="styleSpinner">Spinner style data.</param>
		/// <param name="style">Style data.</param>
		public virtual void SetStyle(StyleSpinner styleSpinner, Style style)
		{
			styleSpinner.InputText.ApplyTo(textComponent.gameObject, true);

			if (placeholder != null)
			{
				styleSpinner.InputPlaceholder.ApplyTo(placeholder.gameObject, true);
			}
		}
	}
}
#endif