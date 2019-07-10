namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// List view item component.
	/// </summary>
	public class ListViewStringComponent : ListViewItem, IViewData<string>
	{
		/// <summary>
		/// The Text component.
		/// </summary>
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
		/// Sets the data.
		/// </summary>
		/// <param name="item">Text.</param>
		public virtual void SetData(string item)
		{
			Text.text = item.Replace("\\n", "\n");
		}
	}
}