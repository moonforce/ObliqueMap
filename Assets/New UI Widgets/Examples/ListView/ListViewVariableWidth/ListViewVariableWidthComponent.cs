namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewVariableWidth component.
	/// </summary>
	public class ListViewVariableWidthComponent : ListViewItem, IViewData<ListViewVariableWidthItemDescription>
	{
		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Name, Text, };
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewVariableWidthItemDescription item)
		{
			Name.text = item.Name;
			Text.text = item.Text.Replace("\\n", "\n");
		}
	}
}