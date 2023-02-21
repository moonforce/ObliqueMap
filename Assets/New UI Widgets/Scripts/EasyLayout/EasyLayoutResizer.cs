namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// EasyLayoutResizer
	/// </summary>
	public class EasyLayoutResizer
	{
		/// <summary>
		/// Layout.
		/// </summary>
		protected EasyLayout Layout;

		/// <summary>
		/// Changed RectTransform properties tracker.
		/// </summary>
		public DrivenRectTransformTracker PropertiesTracker;

		/// <summary>
		/// Initializes a new instance of the <see cref="EasyLayoutResizer"/> class.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public EasyLayoutResizer(EasyLayout layout)
		{
			Layout = layout;
		}

		/// <summary>
		/// Clear tracker.
		/// </summary>
		public void Clear()
		{
			PropertiesTracker.Clear();
		}

		float maxWidth = -1;
		float maxHeight = -1;

		/// <summary>
		/// Resize elements.
		/// </summary>
		/// <param name="elements">Elements to resize.</param>
		public void ResizeElements(List<LayoutElementInfo> elements)
		{
			Clear();

			if (Layout.ChildrenWidth == ChildrenSize.DoNothing && Layout.ChildrenHeight == ChildrenSize.DoNothing)
			{
				return;
			}

			if (elements == null)
			{
				return;
			}

			if (elements.Count == 0)
			{
				return;
			}

			SetMaxSize(elements);

			foreach (var elem in elements)
			{
				ResizeElement(elem);
			}
		}

		void SetMaxSize(List<LayoutElementInfo> elements)
		{
			var max_width = -1f;
			var max_height = -1f;

			foreach (var elem in elements)
			{
				max_width = Mathf.Max(max_width, elem.PreferredWidth);
				max_height = Mathf.Max(max_height, elem.PreferredHeight);
			}

			maxWidth = (Layout.ChildrenWidth == ChildrenSize.SetMaxFromPreferred) ? max_width : -1f;
			maxHeight = (Layout.ChildrenHeight == ChildrenSize.SetMaxFromPreferred) ? max_height : -1f;
		}

		void ResizeElement(LayoutElementInfo element)
		{
			if (Layout.ChildrenWidth != ChildrenSize.DoNothing)
			{
				element.NewWidth = (maxWidth != -1) ? maxWidth : element.PreferredWidth;
			}

			if (Layout.ChildrenHeight != ChildrenSize.DoNothing)
			{
				element.NewHeight = (maxHeight != -1) ? maxHeight : element.PreferredHeight;
			}
		}

		void ShrinkOnOverflow(List<List<LayoutElementInfo>> group, Vector2 size)
		{
			if (group.Count == 0)
			{
				return;
			}

			var rows = group.Count - 1;
			var columns = EasyLayoutUtilites.MaxCount(group) - 1;
			var size_without_spacing = new Vector2(size.x - (Layout.Spacing.x * columns), size.y - (Layout.Spacing.y * rows));
			var ui_size_without_spacing = new Vector2(Layout.UISize.x - (Layout.Spacing.x * columns), Layout.UISize.y - (Layout.Spacing.y * rows));
			var scale = GetShrinkScale(size_without_spacing, ui_size_without_spacing);
			foreach (var row in group)
			{
				foreach (var elem in row)
				{
					elem.NewWidth = elem.Width * scale;
					elem.NewHeight = elem.Height * scale;
				}
			}
		}

		/// <summary>
		/// Resize group.
		/// </summary>
		/// <param name="group">LayoutElementInfo group.</param>
		public void ResizeGroup(List<List<LayoutElementInfo>> group)
		{
			var rectTransform = Layout.transform as RectTransform;
			var width = rectTransform.rect.width - (Layout.GetMarginLeft() + Layout.GetMarginRight()) - (Layout.PaddingInner.Left + Layout.PaddingInner.Right);
			var height = rectTransform.rect.height - (Layout.GetMarginTop() + Layout.GetMarginBottom()) - (Layout.PaddingInner.Top + Layout.PaddingInner.Bottom);
			var size = new Vector2(width, height);

			if ((Layout.ChildrenWidth == ChildrenSize.ShrinkOnOverflow) && (Layout.ChildrenHeight == ChildrenSize.ShrinkOnOverflow))
			{
				ShrinkOnOverflow(group, size);
			}
			else if (Layout.LayoutType == LayoutTypes.Grid)
			{
				if (Layout.ChildrenWidth == ChildrenSize.FitContainer)
				{
					ResizeColumnWidthToFit(size.x, group);
				}
				else if (Layout.ChildrenWidth == ChildrenSize.ShrinkOnOverflow)
				{
					ShrinkColumnWidthToFit(size.x, group);
				}

				if (Layout.ChildrenHeight == ChildrenSize.FitContainer)
				{
					ResizeRowHeightToFit(size.y, group);
				}
				else if (Layout.ChildrenHeight == ChildrenSize.ShrinkOnOverflow)
				{
					ShrinkRowHeightToFit(size.y, group);
				}
			}
			else
			{
				if (Layout.Stacking == Stackings.Horizontal)
				{
					if (Layout.ChildrenWidth == ChildrenSize.FitContainer)
					{
						ResizeWidthToFit(size.x, group);
					}
					else if (Layout.ChildrenWidth == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkWidthToFit(size.x, group);
					}

					if (Layout.ChildrenHeight == ChildrenSize.FitContainer)
					{
						ResizeRowHeightToFit(size.y, group);
					}
					else if (Layout.ChildrenHeight == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkRowHeightToFit(size.y, group);
					}
				}
				else
				{
					if (Layout.ChildrenWidth == ChildrenSize.FitContainer)
					{
						ResizeColumnWidthToFit(size.x, group);
					}
					else if (Layout.ChildrenWidth == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkColumnWidthToFit(size.x, group);
					}

					if (Layout.ChildrenHeight == ChildrenSize.FitContainer)
					{
						ResizeHeightToFit(size.y, group);
					}
					else if (Layout.ChildrenHeight == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkHeightToFit(size.y, group);
					}
				}
			}
		}

		static float GetShrinkScale(Vector2 requiredSize, Vector2 currentSize)
		{
			var scale = requiredSize.x / currentSize.x;
			if ((scale > 1) || ((currentSize.y * scale) > requiredSize.y))
			{
				return Math.Min(1f, requiredSize.y / currentSize.y);
			}

			return Math.Min(1f, scale);
		}

		static void ResizeToFit(float size, List<LayoutElementInfo> elems, float spacing, RectTransform.Axis axis)
		{
			var sizes = axis == RectTransform.Axis.Horizontal ? SizesInfo.GetWidths(elems) : SizesInfo.GetHeights(elems);

			float free_space = size - sizes.TotalPreferred - ((elems.Count - 1) * spacing);
			var per_flexible = Mathf.Max(0f, free_space / sizes.TotalFlexible);

			var minPrefLerp = 0f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((elems.Count - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			for (int i = 0; i < elems.Count; i++)
			{
				var element_size = Mathf.Lerp(sizes.Sizes[i].Min, sizes.Sizes[i].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[i].Flexible);
				elems[i].SetSize(axis, element_size);
			}
		}

		static void ShrinkToFit(float size, List<LayoutElementInfo> elems, float spacing, RectTransform.Axis axis)
		{
			var sizes = axis == RectTransform.Axis.Horizontal ? SizesInfo.GetWidths(elems) : SizesInfo.GetHeights(elems);

			float free_space = size - sizes.TotalPreferred - ((elems.Count - 1) * spacing);
			if (free_space > 0f)
			{
				return;
			}

			var per_flexible = Mathf.Max(0f, free_space / sizes.TotalFlexible);

			var minPrefLerp = 0f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((elems.Count - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			for (int i = 0; i < elems.Count; i++)
			{
				var element_size = Mathf.Lerp(sizes.Sizes[i].Min, sizes.Sizes[i].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[i].Flexible);
				elems[i].SetSize(axis, element_size);
			}
		}

		void ResizeHeightToFit(float height, List<List<LayoutElementInfo>> group)
		{
			var transposed_group = EasyLayoutUtilites.Transpose(group);
			for (int i = 0; i < transposed_group.Count; i++)
			{
				ResizeToFit(height, transposed_group[i], Layout.Spacing.y, RectTransform.Axis.Vertical);
			}
		}

		void ShrinkHeightToFit(float height, List<List<LayoutElementInfo>> group)
		{
			var transposed_group = EasyLayoutUtilites.Transpose(group);
			for (int i = 0; i < transposed_group.Count; i++)
			{
				ShrinkToFit(height, transposed_group[i], Layout.Spacing.y, RectTransform.Axis.Vertical);
			}
		}

		void ResizeWidthToFit(float width, List<List<LayoutElementInfo>> group)
		{
			for (int i = 0; i < group.Count; i++)
			{
				ResizeToFit(width, group[i], Layout.Spacing.x, RectTransform.Axis.Horizontal);
			}
		}

		void ShrinkWidthToFit(float width, List<List<LayoutElementInfo>> group)
		{
			for (int i = 0; i < group.Count; i++)
			{
				ShrinkToFit(width, group[i], Layout.Spacing.x, RectTransform.Axis.Horizontal);
			}
		}

		void ShrinkRowHeightToFit(float height, List<List<LayoutElementInfo>> group)
		{
			ShrinkToFit(height, group, Layout.Spacing.y, RectTransform.Axis.Vertical);
		}

		void ResizeRowHeightToFit(float height, List<List<LayoutElementInfo>> group)
		{
			ResizeToFit(height, group, Layout.Spacing.y, RectTransform.Axis.Vertical);
		}

		void ShrinkColumnWidthToFit(float width, List<List<LayoutElementInfo>> group)
		{
			var transposed_group = EasyLayoutUtilites.Transpose(group);

			ShrinkToFit(width, transposed_group, Layout.Spacing.x, RectTransform.Axis.Horizontal);
		}

		void ResizeColumnWidthToFit(float width, List<List<LayoutElementInfo>> group)
		{
			var transposed_group = EasyLayoutUtilites.Transpose(group);

			ResizeToFit(width, transposed_group, Layout.Spacing.x, RectTransform.Axis.Horizontal);
		}

		static void ShrinkToFit(float size, List<List<LayoutElementInfo>> elems, float spacing, RectTransform.Axis axis)
		{
			var sizes = axis == RectTransform.Axis.Horizontal ? SizesInfo.GetWidths(elems) : SizesInfo.GetHeights(elems);

			float free_space = size - sizes.TotalPreferred - ((elems.Count - 1) * spacing);
			if (free_space > 0f)
			{
				return;
			}

			var per_flexible = Mathf.Max(0f, free_space / sizes.TotalFlexible);

			var minPrefLerp = 0f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((elems.Count - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			for (int i = 0; i < elems.Count; i++)
			{
				var element_size = Mathf.Lerp(sizes.Sizes[i].Min, sizes.Sizes[i].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[i].Flexible);
				for (int j = 0; j < elems[i].Count; j++)
				{
					elems[i][j].SetSize(axis, element_size);
				}
			}
		}

		static void ResizeToFit(float size, List<List<LayoutElementInfo>> elems, float spacing, RectTransform.Axis axis)
		{
			var sizes = axis == RectTransform.Axis.Horizontal ? SizesInfo.GetWidths(elems) : SizesInfo.GetHeights(elems);

			float free_space = size - sizes.TotalPreferred - ((elems.Count - 1) * spacing);
			var per_flexible = Mathf.Max(0f, free_space / sizes.TotalFlexible);

			var minPrefLerp = 0f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((elems.Count - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			for (int i = 0; i < elems.Count; i++)
			{
				var element_size = Mathf.Lerp(sizes.Sizes[i].Min, sizes.Sizes[i].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[i].Flexible);
				for (int j = 0; j < elems[i].Count; j++)
				{
					elems[i][j].SetSize(axis, element_size);
				}
			}
		}
	}
}