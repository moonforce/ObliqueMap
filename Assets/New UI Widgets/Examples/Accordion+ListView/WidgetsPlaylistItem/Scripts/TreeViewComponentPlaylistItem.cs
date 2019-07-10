namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// TreeView component for the PlaylistItem.
	/// </summary>
	public class TreeViewComponentPlaylistItem : UIWidgets.TreeViewComponentBase<UIWidgets.Examples.PlaylistItem>
	{
		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override UnityEngine.UI.Graphic[] GraphicsForeground
		{
			get
			{
				return new UnityEngine.UI.Graphic[] { Title, PlayStart, PlayStop,  };
			}
		}

		/// <summary>
		/// The Title.
		/// </summary>
		public UnityEngine.UI.Text Title;

		/// <summary>
		/// The PlayStart.
		/// </summary>
		public UnityEngine.UI.Text PlayStart;

		/// <summary>
		/// The PlayStop.
		/// </summary>
		public UnityEngine.UI.Text PlayStop;

		UIWidgets.Examples.PlaylistItem item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public UIWidgets.Examples.PlaylistItem Item
		{
			get
			{
				return item;
			}

			set
			{
				item = value;

				UpdateView();
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(UIWidgets.TreeNode<UIWidgets.Examples.PlaylistItem> node, int depth)
		{
			Node = node;

			base.SetData(Node, depth);

			Item = (Node == null) ? null : Node.Item;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (Title != null)
			{
				Title.text = Item.Title;
			}

			if (PlayStart != null)
			{
				PlayStart.text = Item.PlayStart.ToString();
			}

			if (PlayStop != null)
			{
				PlayStop.text = Item.PlayStop.ToString();
			}
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
		}
	}
}