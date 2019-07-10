namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// KeyValueListView item.
	/// </summary>
	public class KeyValueListViewItem : ListViewItem, IViewData<KeyValuePair<string, string>>
	{
		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Text, };
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(KeyValuePair<string, string> item)
		{
			Text.text = item.Value;
		}
	}
}