namespace UIWidgets.Examples.Shops
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// TraderTreeView component.
	/// </summary>
	public class TraderTreeViewComponent : TreeViewComponentBase<JRPGOrderLine>
	{
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// The price.
		/// </summary>
		[SerializeField]
		public Text Price;

		/// <summary>
		/// The available count.
		/// </summary>
		[SerializeField]
		public Text AvailableCount;

		/// <summary>
		/// Count spinner.
		/// </summary>
		[SerializeField]
		protected Spinner Count;

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Name, Price, AvailableCount };
			}
		}

		/// <summary>
		/// Gets or sets the OrderLine.
		/// </summary>
		/// <value>The OrderLine.</value>
		public JRPGOrderLine OrderLine
		{
			get;
			protected set;
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(TreeNode<JRPGOrderLine> node, int depth)
		{
			Node = node;
			base.SetData(Node, depth);

			OrderLine = Node.Item;

			UpdateView();
		}

		/// <summary>
		/// Delete node from TreeView.
		/// </summary>
		public void Delete()
		{
			Node.Parent = null;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (OrderLine.IsPlaylist)
			{
				Name.text = OrderLine.Item.Name;

				// disable unused gameobjects
				Price.transform.parent.gameObject.SetActive(false);
				AvailableCount.gameObject.SetActive(false);
				Count.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				// enable unused gameobjects
				Price.transform.parent.gameObject.SetActive(true);
				AvailableCount.gameObject.SetActive(true);
				Count.transform.parent.gameObject.SetActive(true);

				Name.text = OrderLine.Item.Name;
				Price.text = OrderLine.Price.ToString();
				AvailableCount.text = (OrderLine.Item.Count == -1) ? "∞" : OrderLine.Item.Count.ToString();

				Count.Min = 0;
				Count.Max = (OrderLine.Item.Count == -1) ? 9999 : OrderLine.Item.Count;
				Count.Value = OrderLine.Count;
			}
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		protected override void Start()
		{
			Count.onValueChangeInt.AddListener(ChangeCount);
			base.Start();
		}

		/// <summary>
		/// Change count on left and right movements.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					Count.Value -= 1;
					break;
				case MoveDirection.Right:
					Count.Value += 1;
					break;
				default:
					base.OnMove(eventData);
					break;
			}
		}

		void ChangeCount(int count)
		{
			OrderLine.Count = count;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Count != null)
			{
				Count.onValueChangeInt.RemoveListener(ChangeCount);
			}
		}

		#region edit playlist name

		/// <summary>
		/// EditPlaylistDialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("editPlaylistDialog")]
		protected Dialog EditPlaylistDialogTemplate;

		/// <summary>
		/// Open EditPlaylistDialog.
		/// </summary>
		public void EditPlaylistDialog()
		{
			// if not playlist do nothing
			if (!OrderLine.IsPlaylist)
			{
				return;
			}

			// create dialog from template
			var dialog = EditPlaylistDialogTemplate.Clone();

			// helper component with references to input fields
			var helper = dialog.GetComponent<PlaylistDialogHelper>();

			// reset input fields to default
			helper.Name.text = OrderLine.Item.Name;

			var actions = new DialogActions()
			{
				{ "Change", () => CheckPlaylistDialog(helper) },

				// on click close dialog
				{ "Cancel", Dialog.Close },
			};

			// open dialog
			dialog.Show(
				title: "Change playlist name",
				buttons: actions,
				focusButton: "Sign in",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}

		/// <summary>
		/// Check EditPlaylistDialog and change dialog name if valid.
		/// </summary>
		/// <param name="helper">Dialog helper to check values.</param>
		/// <returns>true if values valid; otherwise false.</returns>
		protected bool CheckPlaylistDialog(PlaylistDialogHelper helper)
		{
			if (!helper.Validate())
			{
				// return false to keep dialog open
				return false;
			}

			// change name in node
			OrderLine.Item.Name = helper.Name.text;

			// change displayed name
			Name.text = helper.Name.text;

			// return true to close dialog
			return true;
		}
		#endregion
	}
}