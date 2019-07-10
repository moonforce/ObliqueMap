namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TreeViewCustomNode drop support.
	/// </summary>
	/// <typeparam name="TTreeView">Type of the TreeView.</typeparam>
	/// <typeparam name="TTreeViewComponent">Type of the TreeView component.</typeparam>
	/// <typeparam name="TTreeViewItem">Type of the TreeView item.</typeparam>
	public class TreeViewCustomNodeDropSupport<TTreeView, TTreeViewComponent, TTreeViewItem> : MonoBehaviour, IDropSupport<TreeNode<TTreeViewItem>>, IDropSupport<TTreeViewItem>
		where TTreeView : TreeViewCustom<TTreeViewComponent, TTreeViewItem>
		where TTreeViewComponent : TreeViewComponentBase<TTreeViewItem>
	{
		TTreeViewComponent source;

		/// <summary>
		/// Gets the current TreeViewComponent.
		/// </summary>
		/// <value>Current TreeViewComponent.</value>
		public TTreeViewComponent Source
		{
			get
			{
				if (source == null)
				{
					source = GetComponent<TTreeViewComponent>();
				}

				return source;
			}
		}

		#region IDropSupport<TreeNode<TItem>>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TreeNode<TTreeViewItem> data, PointerEventData eventData)
		{
			return data.CanBeParent(Source.Node);
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TreeNode<TTreeViewItem> data, PointerEventData eventData)
		{
			var tree = Source.Owner as TTreeView;

			tree.Nodes.BeginUpdate();

			Source.Node.IsExpanded = true;
			data.Parent = Source.Node;

			tree.Nodes.EndUpdate();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TreeNode<TTreeViewItem> data, PointerEventData eventData)
		{
		}
		#endregion

		#region IDropSupport<TTreeViewItem>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TTreeViewItem data, PointerEventData eventData)
		{
			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TTreeViewItem data, PointerEventData eventData)
		{
			var node = Source.Node;
			if (node.Nodes == null)
			{
				node.Nodes = new ObservableList<TreeNode<TTreeViewItem>>();
			}

			var newNode = new TreeNode<TTreeViewItem>(data, null, true, true);
			node.Nodes.Add(newNode);
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TTreeViewItem data, PointerEventData eventData)
		{
		}
		#endregion
	}
}