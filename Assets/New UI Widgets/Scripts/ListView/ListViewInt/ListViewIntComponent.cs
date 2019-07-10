namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewInt component.
	/// </summary>
	public class ListViewIntComponent : ListViewIntComponentBase, IViewData<int>
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public Text Number;

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Number, };
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(int item)
		{
			Number.text = item.ToString();
		}
	}
}