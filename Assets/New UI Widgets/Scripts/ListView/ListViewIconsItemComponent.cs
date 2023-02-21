namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class ListViewIconsItemComponent : ListViewItem, IViewData<ListViewIconsItemDescription>, IViewData<TreeViewItem>
	{
		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				return new GameObject[] { Icon.transform.parent.gameObject, Text.gameObject, };
			}
		}

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
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public ListViewIconsItemDescription Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ListViewIconsItemDescription item)
		{
			Item = item;
			name = item == null ? "DefaultItem (Clone)" : item.Name;
			if (Item == null)
			{
				if (Icon != null)
				{
					Icon.sprite = null;
				}

				Text.text = string.Empty;
			}
			else
			{
				if (Icon != null)
				{
					Icon.sprite = Item.Icon;
				}

				Text.text = Item.LocalizedName ?? Item.Name;
			}

			if (SetNativeSize && (Icon != null))
			{
				Icon.SetNativeSize();
			}

			// set transparent color if no icon
			if (Icon != null)
			{
				Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
			}
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(TreeViewItem item)
		{
			SetData(new ListViewIconsItemDescription()
			{
				Name = item.Name,
				LocalizedName = item.LocalizedName,
				Icon = item.Icon,
				Value = item.Value,
			});
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
			if (Icon != null)
			{
				Icon.sprite = null;
			}
		}
	}
}