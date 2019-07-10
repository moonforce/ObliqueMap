namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeGraph component with icons.
	/// </summary>
	public class TreeGraphComponentIcons : TreeGraphComponent<TreeViewItem>
	{
		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Name, };
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void SetData(TreeNode<TreeViewItem> node)
		{
			Node = node;

			Name.text = node.Item.LocalizedName ?? node.Item.Name;

			name = node.Item.Name;
		}
	}
}