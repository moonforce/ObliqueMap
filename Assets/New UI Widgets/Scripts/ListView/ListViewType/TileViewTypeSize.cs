namespace UIWidgets
{
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewBase, IStylable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListView renderer with items of variable size.
		/// </summary>
		protected class TileViewTypeSize : ListViewTypeSize
		{
			/// <summary>
			/// Blocks sizes.
			/// </summary>
			protected readonly List<float> BlockSizes = new List<float>();

			/// <summary>
			/// Items per row.
			/// </summary>
			protected int ItemsPerRow;

			/// <summary>
			/// Items per column.
			/// </summary>
			protected int ItemsPerColumn;

			/// <summary>
			/// Initializes a new instance of the <see cref="TileViewTypeSize"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public TileViewTypeSize(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
			}

			/// <summary>
			/// Is looped list allowed?
			/// </summary>
			/// <returns>True if looped list allowed; otherwise false.</returns>
			public override bool IsTileView
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Determines whether this instance can be virtialized.
			/// </summary>
			/// <returns><c>true</c> if this instance can be virtialized; otherwise, <c>false</c>.</returns>
			public override bool CanVirtualize()
			{
				var scrollRectSpecified = Owner.scrollRect != null;
				var containerSpecified = Owner.Container != null;
				var currentLayout = containerSpecified ? ((Owner.layout != null) ? Owner.layout : Owner.Container.GetComponent<LayoutGroup>()) : null;
				var validLayout = currentLayout is EasyLayout;

				return scrollRectSpecified && validLayout;
			}

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			/// <returns>Maximum count of the visible items.</returns>
			public override int GetMaxVisibleItems()
			{
				CalculateItemsSizes(Owner.DataSource, false);

				return GetMaxVisibleItems(Owner.DataSource);
			}

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <returns>Maximum count of the visible items.</returns>
			protected override int GetMaxVisibleItems(ObservableList<TItem> items)
			{
				var spacing_x = Owner.GetItemSpacingX();
				var spacing_y = Owner.GetItemSpacingY();

				var height = Owner.ScrollRectSize.y + spacing_y - Owner.LayoutBridge.GetFullMarginY();
				var width = Owner.ScrollRectSize.x + spacing_x - Owner.LayoutBridge.GetFullMarginX();

				if (Owner.IsHorizontal())
				{
					ItemsPerColumn = Mathf.FloorToInt(height / (LowestHeight() + spacing_y));
					ItemsPerColumn = Mathf.Max(1, ItemsPerColumn);
					ItemsPerColumn = Owner.LayoutBridge.ColumnsConstraint(ItemsPerColumn);

					CalculateBlockSizes(ItemsPerColumn);

					ItemsPerRow = RequiredBlocksCount(width);
					ItemsPerRow = Mathf.Max(2, ItemsPerRow);
				}
				else
				{
					ItemsPerRow = Mathf.FloorToInt(width / (LowestWidth() + spacing_x));
					ItemsPerRow = Mathf.Max(1, ItemsPerRow);
					ItemsPerRow = Owner.LayoutBridge.RowsConstraint(ItemsPerRow);

					CalculateBlockSizes(ItemsPerRow);

					ItemsPerColumn = RequiredBlocksCount(height);
					ItemsPerColumn = Mathf.Max(2, ItemsPerColumn);
				}

				return ItemsPerRow * ItemsPerColumn;
			}

			float LowestWidth()
			{
				if (ItemSizes.Count == 0)
				{
					return 1f;
				}

				var result = 0f;
				var is_first = true;

				foreach (var size in ItemSizes.Values)
				{
					if (is_first)
					{
						result = size.x;
						is_first = false;
					}
					else
					{
						result = Mathf.Min(result, size.x);
					}
				}

				return result;
			}

			float LowestHeight()
			{
				if (ItemSizes.Count == 0)
				{
					return 1f;
				}

				var result = 0f;
				var is_first = true;

				foreach (var size in ItemSizes.Values)
				{
					if (is_first)
					{
						result = size.y;
						is_first = false;
					}
					else
					{
						result = Mathf.Min(result, size.y);
					}
				}

				return result;
			}

			/// <summary>
			/// Get required blocks count.
			/// </summary>
			/// <param name="total">Total size.</param>
			/// <returns>Required blocks count.</returns>
			protected int RequiredBlocksCount(float total)
			{
				var spacing = Owner.LayoutBridge.GetSpacing();

				var result = 0;
				var min = MinBlockSize();
				for (; ;)
				{
					total -= min;
					if (result > 0)
					{
						total -= spacing;
					}

					if (total < 0)
					{
						break;
					}

					result += 1;
				}

				return result + 2;
			}

			/// <summary>
			/// Get minimal size of the blocks.
			/// </summary>
			/// <returns>Minimal size.</returns>
			protected float MinBlockSize()
			{
				if (BlockSizes.Count == 0)
				{
					return 0f;
				}

				var result = BlockSizes[0];

				for (int i = 1; i < BlockSizes.Count; i++)
				{
					result = Mathf.Min(result, BlockSizes[i]);
				}

				return result;
			}

			/// <summary>
			/// Calculate block sizes.
			/// </summary>
			/// <param name="perBlock">Per block.</param>
			protected void CalculateBlockSizes(int perBlock)
			{
				BlockSizes.Clear();
				var blocks = Mathf.CeilToInt((float)Owner.DataSource.Count / (float)perBlock);
				for (int i = 0; i < blocks; i++)
				{
					var size = GetItemSize(Owner.DataSource[i * perBlock]);
					for (int j = (i * perBlock) + 1; j < (i + 1) * perBlock; j++)
					{
						if (j == Owner.DataSource.Count)
						{
							break;
						}

						size = Mathf.Max(size, GetItemSize(Owner.DataSource[j]));
					}

					BlockSizes.Add(size);
				}
			}

			/// <summary>
			/// Calculates the size of the top filler.
			/// </summary>
			/// <returns>The top filler size.</returns>
			public override float CalculateTopFillerSize()
			{
				return GetBlocksSize(0, Owner.topHiddenItems);
			}

			/// <summary>
			/// Calculates the size of the bottom filler.
			/// </summary>
			/// <returns>The bottom filler size.</returns>
			public override float CalculateBottomFillerSize()
			{
				var last = Owner.DisplayedIndicesLast + 1;
				return last <= 0 ? 0 : GetBlocksSize(last, Owner.DataSource.Count - last);
			}

			/// <summary>
			/// Gets the blocks count.
			/// </summary>
			/// <returns>The blocks count.</returns>
			/// <param name="items">Items.</param>
			protected int GetBlocksCount(int items)
			{
				return items < 0
					? Mathf.FloorToInt((float)items / (float)GetItemsPerBlock())
					: Mathf.CeilToInt((float)items / (float)GetItemsPerBlock());
			}

			/// <summary>
			/// Convert visible block index to item block index.
			/// </summary>
			/// <returns>Block index.</returns>
			/// <param name="index">Visible block index.</param>
			protected virtual int VisibleBlockIndex2BlockIndex(int index)
			{
				return index % BlockSizes.Count;
			}

			/// <summary>
			/// Gets the size of the blocks.
			/// </summary>
			/// <returns>The blocks size.</returns>
			/// <param name="start">Start.</param>
			/// <param name="count">Count.</param>
			protected float GetBlocksSize(int start, int count)
			{
				int start_block = 0;
				int end_block = 0;
				if (count < 0)
				{
					start_block = GetBlocksCount(count);
					end_block = 0;
				}
				else
				{
					start_block = GetBlocksCount(start);
					end_block = GetBlocksCount(start + count);
				}

				var block_count = end_block - start_block;

				var size = 0f;
				for (int i = start_block; i < end_block; i++)
				{
					size += BlockSizes[VisibleBlockIndex2BlockIndex(i)];
				}

				size += Owner.LayoutBridge.GetSpacing() * (block_count - 1);
				if (count < 0)
				{
					size = -size;
				}

				return Owner.LoopedListAvailable ? size : Mathf.Max(0, size);
			}

			/// <summary>
			/// Get block index by item index.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <returns>Block index.</returns>
			public override int GetBlockIndex(int index)
			{
				return Mathf.FloorToInt((float)index / (float)GetItemsPerBlock());
			}

			/// <summary>
			/// Gets the item position.
			/// </summary>
			/// <returns>The item position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPosition(int index)
			{
				var block = GetBlockIndex(index);

				var size = 0f;
				for (int i = 0; i < block; i++)
				{
					size += BlockSizes[i];
				}

				return size + (Owner.LayoutBridge.GetSpacing() * block);
			}

			/// <summary>
			/// Gets the item middle position by index.
			/// </summary>
			/// <returns>The item middle position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionMiddle(int index)
			{
				var start = GetItemPosition(index);
				var end = GetItemPositionBottom(index);
				return start + ((end - start) / 2);
			}

			/// <summary>
			/// Gets the item position bottom.
			/// </summary>
			/// <returns>The item position bottom.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionBottom(int index)
			{
				var block = Mathf.Min(GetBlockIndex(index) + 1, BlockSizes.Count);

				var size = 0f;
				for (int i = 0; i < block; i++)
				{
					size += BlockSizes[i];
				}

				return size + (Owner.LayoutBridge.GetSpacing() * (block - 1)) + Owner.LayoutBridge.GetMargin() - Owner.GetScrollSize();
			}

			int GetIndexAtPosition(float total_size)
			{
				var spacing = Owner.LayoutBridge.GetSpacing();
				int count = 0;

				if (total_size >= 0f)
				{
					for (int index = 0; index < BlockSizes.Count; index++)
					{
						total_size -= BlockSizes[index];
						if (index > 0)
						{
							total_size -= spacing;
						}

						if (total_size < 0)
						{
							break;
						}

						count += 1;
					}
				}
				else
				{
					total_size = -total_size;
					for (int index = BlockSizes.Count - 1; index >= 0; index--)
					{
						total_size -= BlockSizes[index];
						if (index > 0)
						{
							total_size -= spacing;
						}

						count--;
						if (total_size < 0)
						{
							break;
						}
					}
				}

				if (count >= BlockSizes.Count)
				{
					count = BlockSizes.Count - 1;
				}

				return Mathf.Min(count * GetItemsPerBlock(), Owner.DataSource.Count - 1);
			}

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var first_visible_index = Mathf.Max(0, GetIndexAtPosition(Owner.GetScrollValue()));

				return first_visible_index;
			}

			/// <summary>
			/// Gets the last index of the visible.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				var last_visible_index = GetIndexAtPosition(Owner.GetScrollValue() + Owner.GetScrollSize());

				return strict ? last_visible_index : last_visible_index + GetItemsPerBlock();
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			public override int GetNearestIndex(Vector2 point)
			{
				if (Owner.IsSortEnabled())
				{
					return -1;
				}

				var pos_block = Owner.IsHorizontal() ? point.x : -point.y;
				var start = GetIndexAtPosition(pos_block);

				var pos_elem = Owner.IsHorizontal() ? -point.y : point.x;
				var spacing = Owner.LayoutBridge.GetSpacing();
				var end = Mathf.Min(Owner.DataSource.Count, start + GetItemsPerBlock());

				for (int index = start; index < end; index++)
				{
					pos_elem -= GetItemSize(Owner.DataSource[index]);
					if (index > 0)
					{
						pos_elem -= spacing;
					}

					if (pos_elem < 0)
					{
						return index - 1;
					}
				}

				return Mathf.Min(Owner.DataSource.Count, start);
			}

			/// <summary>
			/// Count of items the per block.
			/// </summary>
			/// <returns>The per block.</returns>
			public override int GetItemsPerBlock()
			{
				return Owner.IsHorizontal() ? ItemsPerColumn : ItemsPerRow;
			}

			/// <summary>
			/// Get the size of the ListView.
			/// </summary>
			/// <returns>The size.</returns>
			public override float ListSize()
			{
				if (Owner.DataSource.Count == 0)
				{
					return 0;
				}

				return Utilites.Sum(BlockSizes) + (BlockSizes.Count * Owner.LayoutBridge.GetSpacing()) - Owner.LayoutBridge.GetSpacing();
			}
		}
	}
}