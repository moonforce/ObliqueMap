namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// EasyLayout.
	/// Warning: using RectTransform relative size with positive size delta (like 100% + 10) with ContentSizeFitter can lead to infinite increased size.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/New UI Widgets/EasyLayout")]
	public class EasyLayout : LayoutGroup, INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (x, y) => { };

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		[SerializeField]
		public UnityEvent SettingsChanged = new UnityEvent();

		[SerializeField]
		[FormerlySerializedAs("GroupPosition")]
		Anchors groupPosition = Anchors.UpperLeft;

		/// <summary>
		/// The group position.
		/// </summary>
		public Anchors GroupPosition
		{
			get
			{
				return groupPosition;
			}

			set
			{
				groupPosition = value;
				Changed("GroupPosition");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Stacking")]
		Stackings stacking = Stackings.Horizontal;

		/// <summary>
		/// The stacking type.
		/// </summary>
		public Stackings Stacking
		{
			get
			{
				return stacking;
			}

			set
			{
				stacking = value;
				Changed("Stacking");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("LayoutType")]
		LayoutTypes layoutType = LayoutTypes.Compact;

		/// <summary>
		/// The type of the layout.
		/// </summary>
		public LayoutTypes LayoutType
		{
			get
			{
				return layoutType;
			}

			set
			{
				layoutType = value;
				Changed("LayoutType");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CompactConstraint")]
		CompactConstraints compactConstraint = CompactConstraints.Flexible;

		/// <summary>
		/// Which constraint to use for the Grid layout.
		/// </summary>
		public CompactConstraints CompactConstraint
		{
			get
			{
				return compactConstraint;
			}

			set
			{
				compactConstraint = value;
				Changed("CompactConstraint");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CompactConstraintCount")]
		int compactConstraintCount = 1;

		/// <summary>
		/// How many elements there should be along the constrained axis.
		/// </summary>
		public int CompactConstraintCount
		{
			get
			{
				return Mathf.Max(1, compactConstraintCount);
			}

			set
			{
				compactConstraintCount = value;
				Changed("CompactConstraintCount");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("GridConstraint")]
		GridConstraints gridConstraint = GridConstraints.Flexible;

		/// <summary>
		/// Which constraint to use for the Grid layout.
		/// </summary>
		public GridConstraints GridConstraint
		{
			get
			{
				return gridConstraint;
			}

			set
			{
				gridConstraint = value;
				Changed("GridConstraint");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("GridConstraintCount")]
		int gridConstraintCount = 1;

		/// <summary>
		/// How many cells there should be along the constrained axis.
		/// </summary>
		public int GridConstraintCount
		{
			get
			{
				return Mathf.Max(1, gridConstraintCount);
			}

			set
			{
				gridConstraintCount = value;
				Changed("GridConstraintCount");
			}
		}

		/// <summary>
		/// Constraint count.
		/// </summary>
		public int ConstraintCount
		{
			get
			{
				if (LayoutType == LayoutTypes.Compact)
				{
					return CompactConstraintCount;
				}
				else
				{
					return GridConstraintCount;
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("RowAlign")]
		HorizontalAligns rowAlign = HorizontalAligns.Left;

		/// <summary>
		/// The row align.
		/// </summary>
		public HorizontalAligns RowAlign
		{
			get
			{
				return rowAlign;
			}

			set
			{
				rowAlign = value;
				Changed("RowAlign");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("InnerAlign")]
		InnerAligns innerAlign = InnerAligns.Top;

		/// <summary>
		/// The inner align.
		/// </summary>
		public InnerAligns InnerAlign
		{
			get
			{
				return innerAlign;
			}

			set
			{
				innerAlign = value;
				Changed("InnerAlign");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CellAlign")]
		Anchors cellAlign = Anchors.UpperLeft;

		/// <summary>
		/// The cell align.
		/// </summary>
		public Anchors CellAlign
		{
			get
			{
				return cellAlign;
			}

			set
			{
				cellAlign = value;
				Changed("CellAlign");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Spacing")]
		Vector2 spacing = new Vector2(5, 5);

		/// <summary>
		/// The spacing.
		/// </summary>
		public Vector2 Spacing
		{
			get
			{
				return spacing;
			}

			set
			{
				spacing = value;
				Changed("Spacing");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Symmetric")]
		bool symmetric = true;

		/// <summary>
		/// Symmetric margin.
		/// </summary>
		public bool Symmetric
		{
			get
			{
				return symmetric;
			}

			set
			{
				symmetric = value;
				Changed("Symmetric");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Margin")]
		Vector2 margin = new Vector2(5, 5);

		/// <summary>
		/// The margin.
		/// </summary>
		public Vector2 Margin
		{
			get
			{
				return margin;
			}

			set
			{
				margin = value;
				Changed("Margin");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("PaddingInner")]
		Padding paddingInner;

		/// <summary>
		/// The padding.
		/// </summary>
		public Padding PaddingInner
		{
			get
			{
				return paddingInner;
			}

			set
			{
				paddingInner = value;
				Changed("PaddingInner");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginTop")]
		float marginTop = 5f;

		/// <summary>
		/// The margin top.
		/// </summary>
		public float MarginTop
		{
			get
			{
				return marginTop;
			}

			set
			{
				marginTop = value;
				Changed("MarginTop");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginBottom")]
		float marginBottom = 5f;

		/// <summary>
		/// The margin bottom.
		/// </summary>
		public float MarginBottom
		{
			get
			{
				return marginBottom;
			}

			set
			{
				marginBottom = value;
				Changed("MarginBottom");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginLeft")]
		float marginLeft = 5f;

		/// <summary>
		/// The margin left.
		/// </summary>
		public float MarginLeft
		{
			get
			{
				return marginLeft;
			}

			set
			{
				marginLeft = value;
				Changed("MarginLeft");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginRight")]
		float marginRight = 5f;

		/// <summary>
		/// The margin right.
		/// </summary>
		public float MarginRight
		{
			get
			{
				return marginRight;
			}

			set
			{
				marginRight = value;
				Changed("MarginRight");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("RightToLeft")]
		bool rightToLeft = false;

		/// <summary>
		/// The right to left stacking.
		/// </summary>
		public bool RightToLeft
		{
			get
			{
				return rightToLeft;
			}

			set
			{
				rightToLeft = value;
				Changed("RightToLeft");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("TopToBottom")]
		bool topToBottom = true;

		/// <summary>
		/// The top to bottom stacking.
		/// </summary>
		public bool TopToBottom
		{
			get
			{
				return topToBottom;
			}

			set
			{
				topToBottom = value;
				Changed("TopToBottom");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("SkipInactive")]
		bool skipInactive = true;

		/// <summary>
		/// The skip inactive.
		/// </summary>
		public bool SkipInactive
		{
			get
			{
				return skipInactive;
			}

			set
			{
				skipInactive = value;
				Changed("SkipInactive");
			}
		}

		Func<IEnumerable<GameObject>, IEnumerable<GameObject>> filter = null;

		/// <summary>
		/// The filter.
		/// </summary>
		public Func<IEnumerable<GameObject>, IEnumerable<GameObject>> Filter
		{
			get
			{
				return filter;
			}

			set
			{
				filter = value;
				Changed("Filter");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ChildrenWidth")]
		ChildrenSize childrenWidth;

		/// <summary>
		/// How to control width of the children.
		/// </summary>
		public ChildrenSize ChildrenWidth
		{
			get
			{
				return childrenWidth;
			}

			set
			{
				childrenWidth = value;
				Changed("ChildrenWidth");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ChildrenHeight")]
		ChildrenSize childrenHeight;

		/// <summary>
		/// How to control height of the children.
		/// </summary>
		public ChildrenSize ChildrenHeight
		{
			get
			{
				return childrenHeight;
			}

			set
			{
				childrenHeight = value;
				Changed("ChildrenHeight");
			}
		}

		/// <summary>
		/// Control width of children.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetPreferred instead.")]
		public bool ControlWidth;

		/// <summary>
		/// Control height of children.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetPreferred instead.")]
		[FormerlySerializedAs("ControlHeight")]
		public bool ControlHeight;

		/// <summary>
		/// Sets width of the chidren to maximum width from them.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetMaxFromPreferred instead.")]
		[FormerlySerializedAs("MaxWidth")]
		public bool MaxWidth;

		/// <summary>
		/// Sets height of the chidren to maximum height from them.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetMaxFromPreferred instead.")]
		[FormerlySerializedAs("MaxHeight")]
		public bool MaxHeight;

		Vector2 blockSize;

		/// <summary>
		/// Gets or sets the size of the inner block.
		/// </summary>
		/// <value>The size of the inner block.</value>
		public Vector2 BlockSize
		{
			get
			{
				return blockSize;
			}

			protected set
			{
				blockSize = value;
			}
		}

		Vector2 uiSize;

		/// <summary>
		/// Gets or sets the UI size.
		/// </summary>
		/// <value>The UI size.</value>
		public Vector2 UISize
		{
			get
			{
				return uiSize;
			}

			protected set
			{
				uiSize = value;
			}
		}

		/// <summary>
		/// Size in elements.
		/// </summary>
		public Vector2 Size
		{
			get;
			protected set;
		}
		
		/// <summary>
		/// Gets the minimum height.
		/// </summary>
		/// <value>The minimum height.</value>
		public override float minHeight
		{
			get
			{
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the minimum width.
		/// </summary>
		/// <value>The minimum width.</value>
		public override float minWidth
		{
			get
			{
				return BlockSize[0];
			}
		}

		/// <summary>
		/// Gets the preferred height.
		/// </summary>
		/// <value>The preferred height.</value>
		public override float preferredHeight
		{
			get
			{
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the preferred width.
		/// </summary>
		/// <value>The preferred width.</value>
		public override float preferredWidth
		{
			get
			{
				return BlockSize[0];
			}
		}

		/// <summary>
		/// Property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void Changed(string propertyName)
		{
			SetDirty();

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

			SettingsChanged.Invoke();
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			Resizer.Clear();
			base.OnDisable();
		}

		/// <summary>
		/// Raises the rect transform removed event.
		/// </summary>
		protected virtual void OnRectTransformRemoved()
		{
			SetDirty();
		}

		/// <summary>
		/// Sets the layout horizontal.
		/// </summary>
		public override void SetLayoutHorizontal()
		{
			RepositionUIElements();
		}

		/// <summary>
		/// Sets the layout vertical.
		/// </summary>
		public override void SetLayoutVertical()
		{
			RepositionUIElements();
		}

		/// <summary>
		/// Calculates the layout input horizontal.
		/// </summary>
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			CalculateLayoutSize();
		}

		/// <summary>
		/// Calculates the layout input vertical.
		/// </summary>
		public override void CalculateLayoutInputVertical()
		{
			CalculateLayoutSize();
		}

		/// <summary>
		/// Calculates the size.
		/// </summary>
		void CalculateSize()
		{
			if (LayoutElementsGroup.Count == 0)
			{
				UISize = Vector2.zero;
			}
			else
			{
				UISize = new Vector2(GetWidth(), GetHeight());
			}

			if (Symmetric)
			{
				BlockSize = new Vector2(UISize.x + (Margin.x * 2), UISize.y + (Margin.y * 2));
			}
			else
			{
				BlockSize = new Vector2(UISize.x + MarginLeft + MarginRight, UISize.y + MarginTop + MarginBottom);
			}
		}

		float GetHeight()
		{
			float height = Spacing.y * (LayoutElementsGroup.Count - 1);
			foreach (var row in LayoutElementsGroup)
			{
				float row_height = 0f;
				foreach (var elem in row)
				{
					row_height = Mathf.Max(row_height, elem.Height);
				}

				height += row_height;
			}

			return height + PaddingInner.Top + PaddingInner.Bottom;
		}

		List<float> widths = new List<float>();

		float GetWidth()
		{
			foreach (var block in LayoutElementsGroup)
			{
				for (var j = 0; j < block.Count; j++)
				{
					if (widths.Count == j)
					{
						widths.Add(0);
					}

					widths[j] = Mathf.Max(widths[j], block[j].Width);
				}
			}

			var width = (widths.Count * Spacing.x) - Spacing.x + PaddingInner.Left + PaddingInner.Right;
			foreach (var w in widths)
			{
				width += w;
			}

			widths.Clear();

			return width;
		}

		/// <summary>
		/// Marks layout to update.
		/// </summary>
		public void NeedUpdateLayout()
		{
			UpdateLayout();
		}

		/// <summary>
		/// Calculates the size of the layout.
		/// </summary>
		public void CalculateLayoutSize()
		{
			GroupLayoutElements();
		}

		/// <summary>
		/// Repositions the user interface elements.
		/// </summary>
		void RepositionUIElements()
		{
			GroupLayoutElements();

			Size = Positioner.SetPositions(LayoutElementsGroup);
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			CalculateLayoutInputHorizontal();
			SetLayoutHorizontal();
			CalculateLayoutInputVertical();
			SetLayoutVertical();
		}

		EasyLayoutPositioner positioner;

		/// <summary>
		/// EasyLayout Positioner.
		/// </summary>
		protected EasyLayoutPositioner Positioner
		{
			get
			{
				if (positioner == null)
				{
					positioner = new EasyLayoutPositioner(this);
				}

				return positioner;
			}
		}

		EasyLayoutResizer resizer;

		/// <summary>
		/// EasyLayout Resizer.
		/// </summary>
		protected EasyLayoutResizer Resizer
		{
			get
			{
				if (resizer == null)
				{
					resizer = new EasyLayoutResizer(this);
				}

				return resizer;
			}
		}

		/// <summary>
		/// Is IgnoreLayout enabled?
		/// </summary>
		/// <param name="rect">RectTransform</param>
		/// <returns>true if IgnoreLayout enabled; otherwise, false.</returns>
		protected static bool IsIgnoreLayout(Transform rect)
		{
			#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			var ignorer = rect.GetComponent<ILayoutIgnorer>();
			#else
			var ignorer = rect.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;
			#endif
			return (ignorer != null) && ignorer.ignoreLayout;
		}

		/// <summary>
		/// Layout elements list.
		/// </summary>
		protected List<LayoutElementInfo> ElementsInfo = new List<LayoutElementInfo>();

		/// <summary>
		/// Update LayoutElements.
		/// </summary>
		protected void ElementsInfoUpdate()
		{
			var elements = rectChildren;

			if (!SkipInactive)
			{
				elements = new List<RectTransform>();
				foreach (Transform child in transform)
				{
					if (!IsIgnoreLayout(child))
					{
						elements.Add(child as RectTransform);
					}
				}
			}

			if (Filter != null)
			{
				elements = ApplyFilter(elements);
			}

			ElementInfoReset();
			foreach (var elem in elements)
			{
				ElementsInfo.Add(ElementInfoCreate(elem));
			}

			Resizer.ResizeElements(ElementsInfo);
		}

		Stack<LayoutElementInfo> layoutElementInfoCache = new Stack<LayoutElementInfo>();

		/// <summary>
		/// Reset layout elements list.
		/// </summary>
		protected void ElementInfoReset()
		{
			foreach (var elem in ElementsInfo)
			{
				layoutElementInfoCache.Push(elem);
			}

			ElementsInfo.Clear();
		}

		/// <summary>
		/// Create layout element.
		/// </summary>
		/// <param name="elem">Element.</param>
		/// <returns>Element data.</returns>
		protected LayoutElementInfo ElementInfoCreate(RectTransform elem)
		{
			var info = (layoutElementInfoCache.Count > 0) ? layoutElementInfoCache.Pop() : new LayoutElementInfo();
			info.SetElement(elem, Resizer, this);

			return info;
		}

		/// <summary>
		/// Apply filter.
		/// </summary>
		/// <param name="input">Original list.</param>
		/// <returns>Filtered list.</returns>
		protected List<RectTransform> ApplyFilter(List<RectTransform> input)
		{
			var objects = new List<GameObject>(input.Count);

			foreach (var elem in input)
			{
				objects.Add(elem.gameObject);
			}

			var result = new List<RectTransform>();
			foreach (var elem in Filter(objects))
			{
				result.Add(elem.transform as RectTransform);
			}

			return result;
		}

		/// <summary>
		/// Gets the margin top.
		/// </summary>
		/// <returns>Top margin.</returns>
		public float GetMarginTop()
		{
			return Symmetric ? Margin.y : MarginTop;
		}

		/// <summary>
		/// Gets the margin bottom.
		/// </summary>
		/// <returns>Bottom margin.</returns>
		public float GetMarginBottom()
		{
			return Symmetric ? Margin.y : MarginBottom;
		}

		/// <summary>
		/// Gets the margin left.
		/// </summary>
		/// <returns>Left margin.</returns>
		public float GetMarginLeft()
		{
			return Symmetric ? Margin.x : MarginLeft;
		}

		/// <summary>
		/// Gets the margin right.
		/// </summary>
		/// <returns>Right margin.</returns>
		public float GetMarginRight()
		{
			return Symmetric ? Margin.y : MarginRight;
		}

		static void ReverseList(List<LayoutElementInfo> list)
		{
			list.Reverse();
		}

		/// <summary>
		/// Layout elements group.
		/// </summary>
		protected List<List<LayoutElementInfo>> LayoutElementsGroup = new List<List<LayoutElementInfo>>();

		/// <summary>
		/// Convert layout elements list to group.
		/// </summary>
		protected void GroupLayoutElements()
		{
			LayoutElementsGroup.Clear();

			var base_length = Stacking == Stackings.Horizontal ? rectTransform.rect.width : rectTransform.rect.height;
			base_length -= (Stacking == Stackings.Horizontal) ? (GetMarginLeft() + GetMarginRight()) : (GetMarginTop() + GetMarginBottom());

			ElementsInfoUpdate();

			if (LayoutType == LayoutTypes.Compact)
			{
				EasyLayoutCompact.Group(ElementsInfo, base_length, this, LayoutElementsGroup);
			}
			else
			{
				EasyLayoutGrid.Group(ElementsInfo, base_length, this, LayoutElementsGroup);
			}

			if (!TopToBottom)
			{
				LayoutElementsGroup.Reverse();
			}

			if (RightToLeft)
			{
				LayoutElementsGroup.ForEach(ReverseList);
			}

			CalculateSize();

			Resizer.ResizeGroup(LayoutElementsGroup);

			for (int i = 0; i < LayoutElementsGroup.Count; i++)
			{
				var block = LayoutElementsGroup[i];
				for (int j = 0; j < block.Count; j++)
				{
					block[j].ApplyResize();
				}
			}
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			Upgrade();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected override void OnValidate()
		{
			SetDirty();
		}
		#endif

		[SerializeField]
		int version = 0;

		#pragma warning disable 0618
		/// <summary>
		/// Upgrade to keep compatibility between versions.
		/// </summary>
		public virtual void Upgrade()
		{
			// upgrade to 1.6
			if (version == 0)
			{
				if (ControlWidth)
				{
					ChildrenWidth = MaxWidth ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}

				if (ControlHeight)
				{
					ChildrenHeight = MaxHeight ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}
			}

			version = 1;
		}
		#pragma warning restore 0618
	}
}