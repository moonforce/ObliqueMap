namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SimpleTable component.
	/// </summary>
	public class SimpleTableComponent : ListViewItem, IViewData<SimpleTableItem>
	{
		/// <summary>
		/// Field1.
		/// </summary>
		[SerializeField]
		public Text Field1;

		/// <summary>
		/// Field2.
		/// </summary>
		[SerializeField]
		public Text Field2;

		/// <summary>
		/// Field3.
		/// </summary>
		[SerializeField]
		public Text Field3;

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Field1, Field2, Field3, };
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
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				return new[]
				{
					Field1.transform.parent.gameObject,
					Field2.transform.parent.gameObject,
					Field3.transform.parent.gameObject,
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SimpleTableItem item)
		{
			Field1.text = item.Field1;
			Field2.text = item.Field2;
			Field3.text = item.Field3;
		}
	}
}