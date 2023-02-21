namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TableList component.
	/// </summary>
	public class TableListComponent : ListViewItem, IViewData<List<int>>
	{
		/// <summary>
		/// The text components.
		/// </summary>
		[SerializeField]
		public List<Text> TextComponents = new List<Text>();

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				var result = new Graphic[TextComponents.Count];

				for (int i = 0; i < TextComponents.Count; i++)
				{
					result[i] = TextComponents[i] as Graphic;
				}

				return result;
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
				var result = new GameObject[TextComponents.Count];

				for (int i = 0; i < TextComponents.Count; i++)
				{
					result[i] = TextComponents[i].transform.parent.gameObject;
				}

				return result;
			}
		}

		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		protected List<int> Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(List<int> item)
		{
			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update text components text.
		/// </summary>
		public void UpdateView()
		{
			TextComponents.ForEach(SetData);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="index">Index.</param>
		protected void SetData(Text text, int index)
		{
			text.text = Item.Count > index ? Item[index].ToString() : "none";
		}
	}
}