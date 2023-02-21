namespace EasyLayoutNS
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// EasyLayoutPositioner
	/// </summary>
	public class EasyLayoutPositioner
	{
		/// <summary>
		/// Layout.
		/// </summary>
		protected EasyLayout Layout;

		static readonly List<Vector2> GroupPositions = new List<Vector2>()
		{
			new Vector2(0.0f, 1.0f), // Anchors.UpperLeft
			new Vector2(0.5f, 1.0f), // Anchors.UpperCenter
			new Vector2(1.0f, 1.0f), // Anchors.UpperRight

			new Vector2(0.0f, 0.5f), // Anchors.MiddleLeft
			new Vector2(0.5f, 0.5f), // Anchors.MiddleCenter
			new Vector2(1.0f, 0.5f), // Anchors.MiddleRight

			new Vector2(0.0f, 0.0f), // Anchors.LowerLeft
			new Vector2(0.5f, 0.0f), // Anchors.LowerCenter
			new Vector2(1.0f, 0.0f), // Anchors.LowerRight
		};

		static readonly List<float> RowAligns = new List<float>()
		{
			0.0f, // HorizontalAligns.Left
			0.5f, // HorizontalAligns.Center
			1.0f, // HorizontalAligns.Right
		};

		static readonly List<float> InnerAligns = new List<float>()
		{
			0.0f, // InnerAligns.Top
			0.5f, // InnerAligns.Middle
			1.0f, // InnerAligns.Bottom
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="EasyLayoutPositioner"/> class.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public EasyLayoutPositioner(EasyLayout layout)
		{
			Layout = layout;
		}

		static float SumWidth(List<LayoutElementInfo> list)
		{
			var result = 0f;

			for (int i = 0; i < list.Count; i++)
			{
				result += list[i].Width;
			}

			return result;
		}

		void GetRowsWidths(List<List<LayoutElementInfo>> group)
		{
			RowsWidths.Clear();

			foreach (var row in group)
			{
				var width = SumWidth(row) + ((row.Count - 1) * Layout.Spacing.x);
				RowsWidths.Add(width);
			}
		}

		void GetMaxColumnsWidths(List<List<LayoutElementInfo>> group)
		{
			MaxColumnsWidths.Clear();

			for (var i = 0; i < group.Count; i++)
			{
				for (var j = 0; j < group[i].Count; j++)
				{
					if (MaxColumnsWidths.Count == j)
					{
						MaxColumnsWidths.Add(0);
					}

					MaxColumnsWidths[j] = Mathf.Max(MaxColumnsWidths[j], group[i][j].Width);
				}
			}
		}

		void GetColumnsHeights(List<List<LayoutElementInfo>> group)
		{
			ColumnsHeights.Clear();

			for (int i = 0; i < group.Count; i++)
			{
				float height = 0;
				for (int j = 0; j < group[i].Count; j++)
				{
					height += group[i][j].Height;
				}

				ColumnsHeights.Add(height + ((group[i].Count - 1) * Layout.Spacing.y));
			}
		}

		static float GetMaxRowHeight(List<LayoutElementInfo> row)
		{
			float height = 0;
			for (int i = 0; i < row.Count; i++)
			{
				height = Mathf.Max(height, row[i].Height);
			}

			return height;
		}

		void GetMaxRowsHeights(List<List<LayoutElementInfo>> group)
		{
			MaxRowsHeights.Clear();

			for (int i = 0; i < group.Count; i++)
			{
				MaxRowsHeights.Add(GetMaxRowHeight(group[i]));
			}
		}

		static Vector2 GetMaxCellSize(List<LayoutElementInfo> row)
		{
			float x = 0f;
			float y = 0f;
			for (int i = 0; i < row.Count; i++)
			{
				x = Mathf.Max(x, row[i].Width);
				y = Mathf.Max(y, row[i].Height);
			}

			return new Vector2(x, y);
		}

		Vector2 GetAlignByWidth(LayoutElementInfo ui, float maxWidth, Vector2 cellMaxSize, float emptyWidth)
		{
			if (Layout.LayoutType == LayoutTypes.Compact)
			{
				return new Vector2(
					emptyWidth * RowAligns[(int)Layout.RowAlign],
					(cellMaxSize.y - ui.Height) * InnerAligns[(int)Layout.InnerAlign]);
			}
			else
			{
				var cell_align = GroupPositions[(int)Layout.CellAlign];

				return new Vector2(
					(maxWidth - ui.Width) * cell_align.x,
					(cellMaxSize.y - ui.Height) * (1 - cell_align.y));
			}
		}

		Vector2 GetAlignByHeight(LayoutElementInfo ui, float maxHeight, Vector2 cellMaxSize, float emptyHeight)
		{
			if (Layout.LayoutType == LayoutTypes.Compact)
			{
				return new Vector2(
					(cellMaxSize.x - ui.Width) * InnerAligns[(int)Layout.InnerAlign],
					emptyHeight * RowAligns[(int)Layout.RowAlign]);
			}
			else
			{
				var cell_align = GroupPositions[(int)Layout.CellAlign];

				return new Vector2(
					(cellMaxSize.x - ui.Width) * cell_align.x,
					(maxHeight - ui.Height) * (1 - cell_align.y));
			}
		}

		List<float> RowsWidths = new List<float>();
		List<float> MaxColumnsWidths = new List<float>();

		List<float> ColumnsHeights = new List<float>();
		List<float> MaxRowsHeights = new List<float>();

		LayoutElementInfo element;

		Vector2 GetStartPosition()
		{
			var rectTransform = Layout.transform as RectTransform;

			var anchor_position = GroupPositions[(int)Layout.GroupPosition];
			var start_position = new Vector2(
				rectTransform.rect.width * (anchor_position.x - rectTransform.pivot.x),
				rectTransform.rect.height * (anchor_position.y - rectTransform.pivot.y));

			start_position.x -= anchor_position.x * Layout.UISize.x;
			start_position.y += (1 - anchor_position.y) * Layout.UISize.y;

			start_position.x += Layout.GetMarginLeft() * (((1 - anchor_position.x) * 2) - 1);
			start_position.y += Layout.GetMarginTop() * (((1 - anchor_position.y) * 2) - 1);

			start_position.x += Layout.PaddingInner.Left;
			start_position.y -= Layout.PaddingInner.Top;

			return start_position;
		}

		/// <summary>
		/// Set positions.
		/// </summary>
		/// <param name="group">LayoutElementInfo group.</param>
		public Vector2 SetPositions(List<List<LayoutElementInfo>> group)
		{
			if (Layout.Stacking == Stackings.Horizontal)
			{
				return SetPositionsHorizontal(group);
			}
			else
			{
				return SetPositionsVertical(group);
			}
		}

		Vector2 SetPositionsHorizontal(List<List<LayoutElementInfo>> group)
		{
			var size = Vector2.zero;
			var startPosition = GetStartPosition();
			var position = startPosition;
			var width = Layout.UISize.x;

			GetRowsWidths(group);
			GetMaxColumnsWidths(group);

			var align = new Vector2(0, 0);

			size.y = group.Count;
			for (int coord_x = 0; coord_x < group.Count; coord_x++)
			{
				var row_cell_max_size = GetMaxCellSize(group[coord_x]);

				size.x = Mathf.Max(size.x, group[coord_x].Count);
				for (int coord_y = 0; coord_y < group[coord_x].Count; coord_y++)
				{
					element = group[coord_x][coord_y];
					align = GetAlignByWidth(element, MaxColumnsWidths[coord_y], row_cell_max_size, width - RowsWidths[coord_x]);

					var new_position = GetUIPosition(element, position, align);
					if (element.Rect.localPosition.x != new_position.x || element.Rect.localPosition.y != new_position.y)
					{
						element.Rect.localPosition = new_position;
					}

					position.x += ((Layout.LayoutType == LayoutTypes.Compact)
						? element.Width
						: MaxColumnsWidths[coord_y]) + Layout.Spacing.x;
				}

				position.x = startPosition.x;
				position.y -= row_cell_max_size.y + Layout.Spacing.y;
			}

			return size;
		}

		Vector2 SetPositionsVertical(List<List<LayoutElementInfo>> group)
		{
			var size = Vector2.zero;
			var start_position = GetStartPosition();
			var position = start_position;
			var height = Layout.UISize.y;

			GetMaxRowsHeights(group);

			group = EasyLayoutUtilites.Transpose(group);
			GetColumnsHeights(group);

			var align = new Vector2(0, 0);

			size.x = group.Count;
			for (int coord_y = 0; coord_y < group.Count; coord_y++)
			{
				var column_cell_max_size = GetMaxCellSize(group[coord_y]);

				size.y = Mathf.Max(size.y, group[coord_y].Count);
				for (int coord_x = 0; coord_x < group[coord_y].Count; coord_x++)
				{
					element = group[coord_y][coord_x];

					align = GetAlignByHeight(element, MaxRowsHeights[coord_x], column_cell_max_size, height - ColumnsHeights[coord_y]);

					var new_position = GetUIPosition(element, position, align);
					if (element.Rect.localPosition.x != new_position.x || element.Rect.localPosition.y != new_position.y)
					{
						element.Rect.localPosition = new_position;
					}

					position.y -= ((Layout.LayoutType == LayoutTypes.Compact)
						? element.Height
						: MaxRowsHeights[coord_x]) + Layout.Spacing.y;
				}

				position.y = start_position.y;
				position.x += column_cell_max_size.x + Layout.Spacing.x;
			}

			return size;
		}

		/// <summary>
		/// Gets the user interface element position.
		/// </summary>
		/// <returns>The user interface position.</returns>
		/// <param name="ui">User interface.</param>
		/// <param name="position">Position.</param>
		/// <param name="align">Align.</param>
		static Vector2 GetUIPosition(LayoutElementInfo ui, Vector2 position, Vector2 align)
		{
			var pivot_fix_x = ui.Width * ui.Rect.pivot.x;
			var pivox_fix_y = ui.Height * ui.Rect.pivot.y;
			var new_x = position.x + pivot_fix_x + align.x;
			var new_y = position.y - ui.Height + pivox_fix_y - align.y;

			return new Vector2(new_x, new_y);
		}
	}
}