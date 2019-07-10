#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker Hex block.
	/// </summary>
	public class ColorPickerHexBlockTMPro : ColorPickerHexBlockBase
	{
		/// <summary>
		/// The input field for hex.
		/// </summary>
		[SerializeField]
		protected TMP_InputField InputHex;

		/// <summary>
		/// Init the input.
		/// </summary>
		protected override void InitInput()
		{
			if (InputHex != null)
			{
				InputProxyHex = new InputFieldTMProProxy(InputHex);
			}
		}

#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public override void SetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if (InputHex != null)
			{
				styleColorPicker.HexInputBackground.ApplyTo(InputHex);
				styleColorPicker.HexInputText.ApplyTo(InputHex.textComponent.gameObject);
				if (InputHex.placeholder != null)
				{
					styleColorPicker.HexInputPlaceholder.ApplyTo(InputHex.placeholder.gameObject);
				}
			}
		}
#endregion
	}
}
#endif