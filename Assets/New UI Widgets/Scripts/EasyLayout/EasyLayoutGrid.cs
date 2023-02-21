namespace EasyLayoutNS
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Easy layout grid layout.
	/// </summary>
	public static class EasyLayoutGrid
	{
		/// <summary>
		/// Gets the max columns count.
		/// </summary>
		/// <returns>The max columns count.</returns>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="baseLength">Base length.</param>
		/// <param name="layout">Layout.</param>
		/// <param name="maxColumns">Max columns.</param>
		static int GetMaxColumnsCount(List<LayoutElementInfo> uiElements, float baseLength, EasyLayout layout, int maxColumns)
		{
			var length = baseLength;
			var spacing = (layout.Stacking == Stackings.Horizontal) ? layout.Spacing.x : layout.Spacing.y;

			bool min_columns_setted = false;
			int min_columns = maxColumns;
			int current_columns = 0;

			for (int i = 0; i < uiElements.Count; i++)
			{
				if (current_columns == maxColumns)
				{
					min_columns_setted = true;
					min_columns = Mathf.Min(min_columns, current_columns);

					current_columns = 1;
					length = baseLength - uiElements[i].Size;
					continue;
				}

				if (current_columns == 0)
				{
					current_columns = 1;
					length = baseLength - uiElements[i].Size;
					continue;
				}

				if (length >= (uiElements[i].Size + spacing))
				{
					length -= uiElements[i].Size + spacing;
					current_columns++;
				}
				else
				{
					min_columns_setted = true;
					min_columns = Mathf.Min(min_columns, current_columns);

					current_columns = 1;
					length = baseLength - uiElements[i].Size;
				}
			}

			if (!min_columns_setted)
			{
				min_columns = current_columns;
			}

			return min_columns;
		}

		static void GroupByColumnsVertical(List<LayoutElementInfo> uiElements, int maxColumns, List<List<LayoutElementInfo>> resultedGroup)
		{
			int i = 0;
			for (int column = 0; column < maxColumns; column++)
			{
				int max_rows = Mathf.CeilToInt(((float)(uiElements.Count - i)) / ((float)(maxColumns - column)));
				for (int row = 0; row < max_rows; row++)
				{
					if (row == resultedGroup.Count)
					{
						resultedGroup.Add(new List<LayoutElementInfo>());
					}

					resultedGroup[row].Add(uiElements[i]);
					i++;
				}
			}
		}

		static void GroupByColumnsHorizontal(List<LayoutElementInfo> uiElements, int maxColumns, List<List<LayoutElementInfo>> resultedGroup)
		{
			for (int i = 0; i < uiElements.Count; i += maxColumns)
			{
				var column = new List<LayoutElementInfo>();
				var end = Mathf.Min(i + maxColumns, uiElements.Count);
				for (int j = i; j < end; j++)
				{
					column.Add(uiElements[j]);
				}

				resultedGroup.Add(column);
			}
		}

		static void GroupByRowsVertical(List<LayoutElementInfo> uiElements, int maxRows, List<List<LayoutElementInfo>> resultedGroup)
		{
			for (int i = 0; i < uiElements.Count; i++)
			{
				int row = i % maxRows;
				if (resultedGroup.Count == row)
				{
					resultedGroup.Add(new List<LayoutElementInfo>());
				}

				resultedGroup[row].Add(uiElements[i]);
			}
		}

		static void GroupByRowsHorizontal(List<LayoutElementInfo> uiElements, int maxRows, List<List<LayoutElementInfo>> resultedGroup)
		{
			int i = 0;
			for (int row = 0; row < maxRows; row++)
			{
				resultedGroup.Add(new List<LayoutElementInfo>());

				int max_columns = Mathf.CeilToInt((float)(uiElements.Count - i) / (float)(maxRows - row));
				for (int column = 0; column < max_columns; column++)
				{
					resultedGroup[row].Add(uiElements[i]);
					i++;
				}
			}
		}

		/// <summary>
		/// Group the specified uiElements by columns.
		/// </summary>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="layout">Layout.</param>
		/// <param name="resultedGroup">Result</param>
		public static void GroupByColumns(List<LayoutElementInfo> uiElements, EasyLayout layout, List<List<LayoutElementInfo>> resultedGroup)
		{
			if (layout.Stacking == Stackings.Horizontal)
			{
				GroupByColumnsHorizontal(uiElements, layout.ConstraintCount, resultedGroup);
			}
			else
			{
				GroupByColumnsVertical(uiElements, layout.ConstraintCount, resultedGroup);
			}
		}

		/// <summary>
		/// Group the specified uiElements by rows.
		/// </summary>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="layout">Layout.</param>
		/// <param name="resultedGroup">Result</param>
		public static void GroupByRows(List<LayoutElementInfo> uiElements, EasyLayout layout, List<List<LayoutElementInfo>> resultedGroup)
		{
			if (layout.Stacking == Stackings.Horizontal)
			{
				GroupByRowsHorizontal(uiElements, layout.ConstraintCount, resultedGroup);
			}
			else
			{
				GroupByRowsVertical(uiElements, layout.ConstraintCount, resultedGroup);
			}
		}

		/// <summary>
		/// Group the specified uiElements.
		/// </summary>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="baseLength">Base length (width or size).</param>
		/// <param name="layout">Layout.</param>
		/// <param name="resultedGroup">Result</param>
		public static void Group(List<LayoutElementInfo> uiElements, float baseLength, EasyLayout layout, List<List<LayoutElementInfo>> resultedGroup)
		{
			if (layout.GridConstraint == GridConstraints.Flexible)
			{
				GroupFlexible(uiElements, baseLength, layout, resultedGroup);
			}
			else if (layout.GridConstraint == GridConstraints.FixedRowCount)
			{
				GroupByRows(uiElements, layout, resultedGroup);
			}
			else if (layout.GridConstraint == GridConstraints.FixedColumnCount)
			{
				GroupByColumns(uiElements, layout, resultedGroup);
			}
		}

		/// <summary>
		/// Group the specified uiElements.
		/// </summary>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="baseLength">Base length (width or size).</param>
		/// <param name="layout">Layout.</param>
		/// <param name="resultedGroup">Result</param>
		public static void GroupFlexible(List<LayoutElementInfo> uiElements, float baseLength, EasyLayout layout, List<List<LayoutElementInfo>> resultedGroup)
		{
			int max_columns = 999999;
			while (true)
			{
				var new_max_columns = GetMaxColumnsCount(uiElements, baseLength, layout, max_columns);

				if ((max_columns == new_max_columns) || (new_max_columns == 1))
				{
					break;
				}

				max_columns = new_max_columns;
			}

			if (layout.Stacking == Stackings.Horizontal)
			{
				GroupByColumnsHorizontal(uiElements, max_columns, resultedGroup);
			}
			else
			{
				GroupByRowsVertical(uiElements, max_columns, resultedGroup);
			}
		}
	}
}