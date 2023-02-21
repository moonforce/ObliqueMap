namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drop support for the InputField.
	/// </summary>
	[RequireComponent(typeof(InputField))]
	public class InputFieldDropSupport : MonoBehaviour, IDropSupport<TreeNode<TreeViewItem>>, IDropSupport<string>
	{
		#region IDropSupport<TreeNode<TreeViewItem>>

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			GetComponent<InputField>().text = data.Item.Name;
		}

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			return data.Item.Name.EndsWith("1");
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
		}

		#endregion

		#region IDropSupport<string>

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(string data, PointerEventData eventData)
		{
			GetComponent<InputField>().text = data;
		}

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(string data, PointerEventData eventData)
		{
			return true;
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(string data, PointerEventData eventData)
		{
		}

		#endregion
	}
}