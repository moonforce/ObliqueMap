namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// TreeGraph component for the PlaylistItem.
	/// </summary>
	public class TreeGraphComponentPlaylistItem : UIWidgets.TreeGraphComponent<UIWidgets.Examples.PlaylistItem>
	{
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
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void SetData(UIWidgets.TreeNode<UIWidgets.Examples.PlaylistItem> node)
		{
			Node = node;

			if (Title != null)
			{
				Title.text = Node.Item.Title;
			}

			if (PlayStart != null)
			{
				PlayStart.text = Node.Item.PlayStart.ToString();
			}

			if (PlayStop != null)
			{
				PlayStop.text = Node.Item.PlayStop.ToString();
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