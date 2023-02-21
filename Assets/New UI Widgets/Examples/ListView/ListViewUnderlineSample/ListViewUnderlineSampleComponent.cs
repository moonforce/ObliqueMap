namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewUnderline sample component.
	/// </summary>
	public class ListViewUnderlineSampleComponent : ListViewItem, IViewData<ListViewUnderlineSampleItemDescription>
	{
		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Text, Underline, };
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsBackground
		{
			get
			{
				return new Graphic[] { };
			}
		}

		/// <summary>
		/// Icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Underline.
		/// </summary>
		[SerializeField]
		public Image Underline;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewUnderlineSampleItemDescription item)
		{
			Icon.sprite = item.Icon;
			Text.text = item.Name;

			Icon.SetNativeSize();

			Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
		}
	}
}