namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class TileViewComponentSample : ListViewItem, IViewData<TileViewItemSample>
	{
		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Name, Capital, Area, Population, Density };
			}
		}

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// The capital.
		/// </summary>
		[SerializeField]
		public Text Capital;

		/// <summary>
		/// The area.
		/// </summary>
		[SerializeField]
		public Text Area;

		/// <summary>
		/// The population.
		/// </summary>
		[SerializeField]
		public Text Population;

		/// <summary>
		/// The density.
		/// </summary>
		[SerializeField]
		public Text Density;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		/// <summary>
		/// TileView.
		/// </summary>
		public TileViewSample Tiles;

		/// <summary>
		/// Current item.
		/// </summary>
		public TileViewItemSample Item;

		/// <summary>
		/// Duplicate current item in TileView.DataSource.
		/// </summary>
		public void Duplicate()
		{
			Tiles.DataSource.Add(Item);
		}

		/// <summary>
		/// Remove current item from TileView.DataSource.
		/// </summary>
		public void Remove()
		{
			Tiles.DataSource.RemoveAt(Index);
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(TileViewItemSample item)
		{
			Item = item;
			if (Item == null)
			{
				if (Icon != null)
				{
					Icon.sprite = null;
				}

				if (Name != null)
				{
					Name.text = string.Empty;
				}

				if (Capital != null)
				{
					Capital.text = string.Empty;
				}

				if (Area != null)
				{
					Area.text = string.Empty;
				}

				if (Population != null)
				{
					Population.text = string.Empty;
				}

				if (Density != null)
				{
					Density.text = string.Empty;
				}
			}
			else
			{
				name = item.Name;

				if (Icon != null)
				{
					Icon.sprite = Item.Icon;
				}

				if (Name != null)
				{
					Name.text = Item.Name;
				}

				if (Capital != null)
				{
					Capital.text = "Capital: " + Item.Capital;
				}

				if (Area != null)
				{
					Area.text = "Area: " + Item.Area.ToString("N0") + " sq. km";
				}

				if (Population != null)
				{
					Population.text = "Population: " + Item.Population.ToString("N0");
				}

				if (Density != null)
				{
					var density = Item.Area == 0 ? "n/a" : Mathf.CeilToInt(((float)Item.Population) / Item.Area).ToString("N") + " / sq. km";
					Density.text = "Density: " + density;
				}
			}

			if (Icon != null)
			{
				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				// set transparent color if no icon
				Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
			}
		}
	}
}