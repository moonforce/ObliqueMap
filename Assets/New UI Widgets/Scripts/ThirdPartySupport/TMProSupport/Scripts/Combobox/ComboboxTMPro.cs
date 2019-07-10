#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;

	/// <summary>
	/// Combobox with TMPro support.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Combobox.png
	/// </summary>
	public class ComboboxTMPro : Combobox
	{
		/// <summary>
		/// Init InputFieldProxy.
		/// </summary>
		protected override void InitInputFieldProxy()
		{
			InputProxy = new InputFieldTMProProxy(GetComponent<TMP_InputField>());
			InputProxy.interactable = editable;
			InputProxy.onEndEdit.AddListener(InputItem);
		}
	}
}
#endif