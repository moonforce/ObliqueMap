namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TableRow component.
	/// </summary>
	public class TableRowComponent : ListViewItem, IViewData<TableRow>
	{
		/// <summary>
		/// Cell01Text.
		/// </summary>
		[SerializeField]
		public Text Cell01Text;

		/// <summary>
		/// Cell02Text.
		/// </summary>
		[SerializeField]
		public Text Cell02Text;

		TableRow Item;

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Cell01Text, Cell02Text};
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
				return new GameObject[]
				{
					Cell01Text.transform.parent.gameObject,
					Cell02Text.transform.parent.gameObject
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(TableRow item)
		{
			Item = item;

			Cell01Text.text = Item.Cell01;
			Cell02Text.text = Item.Cell02;
		}

		/// <summary>
		/// Handle cell clicked event.
		/// </summary>
		/// <param name="cellName">Cell name.</param>
		public void CellClicked(string cellName)
		{
			Debug.Log(string.Format("clicked row {0}, cell {1}", Index, cellName));
			switch (cellName)
			{
				case "Cell01":
					Debug.Log("cell value: " + Item.Cell01);
					break;
				case "Cell02":
					Debug.Log("cell value: " + Item.Cell02);
					break;
				default:
					Debug.Log("cell value: <unknown cell>");
					break;
			}
		}
	}
}