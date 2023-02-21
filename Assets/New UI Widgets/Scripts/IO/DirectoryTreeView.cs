namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DirectoryTreeView
	/// </summary>
	public class DirectoryTreeView : TreeViewCustom<DirectoryTreeViewComponent, FileSystemEntry>
	{
		/// <summary>
		/// Root directory, if not specified drives will be used as root.
		/// </summary>
		[SerializeField]
		protected string rootDirectory = string.Empty;

		/// <summary>
		/// Root directory, if not specified drives will be used as root.
		/// </summary>
		public string RootDirectory
		{
			get
			{
				return rootDirectory;
			}

			set
			{
				SetRootDirectory(value);
			}
		}

		/// <summary>
		/// Display IO errors.
		/// </summary>
		[SerializeField]
		public IOExceptionsView ExceptionsView;

		bool isInited = false;

		/// <summary>
		/// Init.
		/// </summary>
		public override void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			base.Init();

			SetRootDirectory(rootDirectory);
		}

		/// <summary>
		/// Set root directory.
		/// </summary>
		/// <param name="root">New root.</param>
		protected virtual void SetRootDirectory(string root)
		{
			rootDirectory = root;

			if (string.IsNullOrEmpty(root))
			{
				var drives = GetDrives();
				LoadNodes(drives);

				Nodes = drives;
			}
			else
			{
				var nodes = GetDirectoriesNodes(root);
				LoadNodes(nodes);

				Nodes = nodes;
			}
		}

		/// <summary>
		/// Get directories list from current to root.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <returns>Directories list from current to root.</returns>
		protected static List<string> GetPaths(string directory)
		{
			var paths = new List<string>();
			var temp = directory;

			do
			{
				paths.Add(temp);
				temp = Path.GetDirectoryName(temp);
			}
			while (!string.IsNullOrEmpty(temp));

			return paths;
		}

		/// <summary>
		/// Get node for specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <returns>Node if directory found; otherwise, null.</returns>
		public virtual TreeNode<FileSystemEntry> GetNodeByPath(string directory)
		{
			if (!Directory.Exists(directory))
			{
				return null;
			}

			var paths = GetPaths(directory);
			var nodes = Nodes;
			TreeNode<FileSystemEntry> node;

			do
			{
				node = nodes.Find(x => paths.Contains(x.Item.FullName));
				if (node == null)
				{
					return null;
				}

				paths.Remove(node.Item.FullName);

				if (node.Nodes == null)
				{
					node.Nodes = GetDirectoriesNodes(node.Item.FullName);
				}

				nodes = node.Nodes;
			}
			while (directory != node.Item.FullName);

			return node;
		}

		/// <summary>
		/// Select node with specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <returns>true if directory found; otherwise, false.</returns>
		public virtual bool SelectDirectory(string directory)
		{
			var node = GetNodeByPath(directory);
			if (node == null)
			{
				return false;
			}

			var parent = node.Parent;
			while (parent != null)
			{
				parent.IsExpanded = true;
				parent = parent.Parent;
			}

			SelectNode(node);
			ScrollTo(Node2Index(node));

			return true;
		}

		/// <summary>
		/// Toggles the node.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void ToggleNode(int index)
		{
			var node = NodesList[index];

			// disable node observation
			node.Node.PauseObservation = true;
			node.Node.IsExpanded = !node.Node.IsExpanded;
			node.Node.PauseObservation = false;

			var nodes = node.Node.Nodes;
			nodes.BeginUpdate();

			// force update because node observation was disabled and if nothing found update will not be called
			nodes.CollectionChanged();

			LoadNodes(nodes);

			nodes.EndUpdate();
		}

		/// <summary>
		/// Ger drives.
		/// </summary>
		/// <returns>Drives list.</returns>
		public virtual ObservableList<TreeNode<FileSystemEntry>> GetDrives()
		{
			var nodes = new ObservableList<TreeNode<FileSystemEntry>>();

			ExceptionsView.Execute(() =>
			{
#if !NETFX_CORE
				var drives = Directory.GetLogicalDrives();
				drives.ForEach(drive =>
				{
					var item = new FileSystemEntry(drive, drive, false);
					nodes.Add(new TreeNode<FileSystemEntry>(item, null));
				});
#endif
			});

			return nodes;
		}

		/// <summary>
		/// Load subnodes data for specified nodes.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		public virtual void LoadNodes(ObservableList<TreeNode<FileSystemEntry>> nodes)
		{
			nodes.BeginUpdate();

			try
			{
				nodes.ForEach(LoadNode);
			}
			finally
			{
				nodes.EndUpdate();
			}
		}

		/// <summary>
		/// Get subnodes for specified directory.
		/// </summary>
		/// <param name="path">Directory.</param>
		/// <returns>Subnodes for specified directory.</returns>
		public virtual ObservableList<TreeNode<FileSystemEntry>> GetDirectoriesNodes(string path)
		{
			var nodes = new ObservableList<TreeNode<FileSystemEntry>>();

			ExceptionsView.Execute(() =>
			{
				var directories = Directory.GetDirectories(path);
				directories.ForEach(directory =>
				{
					var item = new FileSystemEntry(directory, Path.GetFileName(directory), false);
					nodes.Add(new TreeNode<FileSystemEntry>(item, null));
				});
			});
			ExceptionsView.CurrentError = null;

			return nodes;
		}

		/// <summary>
		/// Load subnodes data for specified node.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void LoadNode(TreeNode<FileSystemEntry> node)
		{
			if (node.Nodes != null)
			{
				return;
			}

			node.Nodes = GetDirectoriesNodes(node.Item.FullName);
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public override bool SetStyle(Style style)
		{
			if (ExceptionsView != null)
			{
				ExceptionsView.SetStyle(style);
			}

			return base.SetStyle(style);
		}
		#endregion
	}
}