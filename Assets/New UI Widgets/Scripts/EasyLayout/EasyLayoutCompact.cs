namespace EasyLayoutNS
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// EasyLayout compact layout.
	/// </summary>
	public static class EasyLayoutCompact
	{
		/// <summary>
		/// Group the specified elements.
		/// </summary>
		/// <param name="elements">Elements.</param>
		/// <param name="baseLength">Base length (width or height).</param>
		/// <param name="layout">Layout.</param>
		/// <param name="resultedGroup">Result</param>
		public static void Group(List<LayoutElementInfo> elements, float baseLength, EasyLayout layout, List<List<LayoutElementInfo>> resultedGroup)
		{
			if (elements.Count == 0)
			{
				return;
			}

			if (layout.Stacking == Stackings.Horizontal)
			{
				GroupHorizontal(elements, baseLength, layout, resultedGroup);
			}
			else
			{
				GroupVertical(elements, baseLength, layout, resultedGroup);
			}

			var rows = resultedGroup.Count;
			var columns = EasyLayoutUtilites.MaxCount(resultedGroup);

			if ((layout.CompactConstraint == CompactConstraints.MaxRowCount) && (rows > layout.ConstraintCount))
			{
				resultedGroup.Clear();
				EasyLayoutGrid.GroupByRows(elements, layout, resultedGroup);
			}
			else if ((layout.CompactConstraint == CompactConstraints.MaxColumnCount) && (columns > layout.ConstraintCount))
			{
				resultedGroup.Clear();
				EasyLayoutGrid.GroupByColumns(elements, layout, resultedGroup);
			}
		}

		static void GroupHorizontal(List<LayoutElementInfo> elements, float baseLength, EasyLayout layout, List<List<LayoutElementInfo>> resultedGroup)
		{
			var length = baseLength;
			var spacing = layout.Spacing.x;

			var row = new List<LayoutElementInfo>();
			resultedGroup.Add(row);

			foreach (var element in elements)
			{
				if (row.Count == 0)
				{
					length -= element.Size;
					row.Add(element);
					continue;
				}

				if (length >= (element.Size + spacing))
				{
					length -= element.Size + spacing;
					row.Add(element);
				}
				else
				{
					length = baseLength - element.Size;

					row = new List<LayoutElementInfo>();
					resultedGroup.Add(row);

					row.Add(element);
				}
			}
		}

		static void GroupVertical(List<LayoutElementInfo> elements, float baseLength, EasyLayout layout, List<List<LayoutElementInfo>> group)
		{
			var length = baseLength;

			var spacing = layout.Spacing.y;
			var block = 0;
			group.Add(new List<LayoutElementInfo>());
			foreach (var element in elements)
			{
				if (group[block].Count == 0)
				{
					length -= element.Size;
					group[block].Add(element);
					continue;
				}

				if (length >= (element.Size + spacing))
				{
					length -= element.Size + spacing;

					block += 1;
					if (group.Count == block)
					{
						group.Add(new List<LayoutElementInfo>());
					}

					group[block].Add(element);
				}
				else
				{
					length = baseLength - element.Size;
					group[0].Add(element);
					block = 0;
				}
			}
		}
	}
}