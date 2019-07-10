namespace UIWidgets.Examples.Widgets
{
	/// <summary>
	/// ListView component for the PlaylistItem.
	/// </summary>
	public class ListViewComponentPlaylistItem : UIWidgets.ListViewItem, UIWidgets.IViewData<UIWidgets.Examples.PlaylistItem>
	{
		UnityEngine.UI.Graphic[] cellsBackground;

		/// <summary>
		/// Cells backgrounds.
		/// </summary>
		public virtual UnityEngine.UI.Graphic[] CellsBackground
		{
			get
			{
				if (cellsBackground == null)
				{
					var result = new System.Collections.Generic.List<UnityEngine.UI.Graphic>();

					foreach (UnityEngine.Transform child in transform)
					{
						var graphic = child.GetComponent<UnityEngine.UI.Graphic>();
						if (graphic != null)
						{
							result.Add(graphic);
						}
					}

					cellsBackground = result.ToArray();
				}

				return cellsBackground;
			}
		}

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override UnityEngine.UI.Graphic[] GraphicsBackground
		{
			get
			{
				var is_table = (Owner != null) && Owner.IsTable;
				return is_table ? CellsBackground : base.GraphicsBackground;
			}
		}

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

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public UIWidgets.Examples.PlaylistItem Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Remove current item.
		/// </summary>
		public void Remove()
		{
			(Owner as ListViewPlaylistItem).DataSource.RemoveAt(Index);
		}

		/// <summary>
		/// Deselect current item.
		/// </summary>
		public void Deselect()
		{
			Owner.Deselect(Index);
		}

		#region Added Code

		/// <summary>
		/// RangeSlider.
		/// </summary>
		public UIWidgets.RangeSlider PlaySlider;

		/// <summary>
		/// Accordion.
		/// </summary>
		public UIWidgets.Accordion Accordion;

		/// <summary>
		/// Test text.
		/// </summary>
		public UnityEngine.UI.Text TestText;

		#region Current

		/// <summary>
		/// Playlist current.
		/// </summary>
		public PlaylistCurrent Current;

		/// <summary>
		/// Playlist indicator.
		/// </summary>
		public UnityEngine.UI.Text Indicator;

		/// <summary>
		/// Process changed Current value.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void CurrentChanged(int index)
		{
			if (Indicator != null)
			{
				Indicator.text = Index == index ? "play" : "stop";
			}
		}

		/// <summary>
		/// Play.
		/// </summary>
		public void Play()
		{
			if (Current != null)
			{
				Current.Index = Index;
			}
		}
		#endregion

		/// <summary>
		/// Add callback.
		/// </summary>
		protected override void Start()
		{
			#region Current
			if (Current != null)
			{
				Current.OnChange.AddListener(CurrentChanged);
			}
			#endregion

			if (PlaySlider != null)
			{
				PlaySlider.OnValuesChange.AddListener(PlaySliderChanged);
			}

			if (Accordion != null)
			{
				Accordion.OnToggleItem.AddListener(AccordionChanged);
			}

			base.Start();
		}

		/// <summary>
		/// Remove callback.
		/// </summary>
		protected override void OnDestroy()
		{
			if (PlaySlider != null)
			{
				PlaySlider.OnValuesChange.RemoveListener(PlaySliderChanged);
			}

			if (Accordion != null)
			{
				Accordion.OnToggleItem.RemoveListener(AccordionChanged);
			}

			base.OnDestroy();
		}

		void PlaySliderChanged(int min, int max)
		{
			if (Item != null)
			{
				Item.PlayStart = PlaySlider.ValueMin;
				Item.PlayStop = PlaySlider.ValueMax;
			}
		}

		void AccordionChanged(UIWidgets.AccordionItem accordionItem)
		{
			if (Item != null)
			{
				Item.IsOpened = accordionItem.Open;
			}
		}
		#endregion

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(UIWidgets.Examples.PlaylistItem item)
		{
			Item = item;

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

			#region Added Code
			if (PlaySlider != null)
			{
				PlaySlider.SetLimit(0, Item.Track.samples);

				PlaySlider.SetValue(Item.PlayStart, Item.PlayStop);
			}

			if (Accordion != null)
			{
				if (TestText != null)
				{
					TestText.text = (Index % 2) == 0 ? Item.Title : Item.Title + "\r\n" + Item.Title;
				}

				if (item.IsOpened)
				{
					Accordion.ForceOpen(Accordion.DataSource[0]);
				}
				else
				{
					Accordion.ForceClose(Accordion.DataSource[0]);
				}
			}
			#endregion

			#region Current
			if (Current != null)
			{
				CurrentChanged(Current.Index);
			}
			#endregion
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
			#region Added Code
			Item = null;
			#endregion
		}
	}
}