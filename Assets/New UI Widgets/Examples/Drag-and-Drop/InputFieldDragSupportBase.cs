namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Drag support for the InputField.
	/// </summary>
	[RequireComponent(typeof(InputField))]
	public class InputFieldDragSupportBase : DragSupport<string>
	{
		/// <summary>
		/// Set Data, which will be passed to the Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = GetComponent<InputField>().text;
		}
	}
}