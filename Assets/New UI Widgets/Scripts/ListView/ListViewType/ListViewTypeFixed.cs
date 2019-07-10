namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewBase, IStylable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListView renderer with items of fixed size.
		/// </summary>
		protected class ListViewTypeFixed : ListViewTypeBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeFixed"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public ListViewTypeFixed(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
			}

			/// <summary>
			/// Gets the size of the item.
			/// </summary>
			/// <returns>The item size.</returns>
			protected float GetItemSize()
			{
				return Owner.IsHorizontal()
					? Owner.ItemSize.x + Owner.LayoutBridge.GetSpacing()
					: Owner.ItemSize.y + Owner.LayoutBridge.GetSpacing();
			}

			/// <summary>
			/// Calculates the size of the top filler.
			/// </summary>
			/// <returns>The top filler size.</returns>
			public override float CalculateTopFillerSize()
			{
				if (Owner.topHiddenItems == 0)
				{
					return 0f;
				}

				var size = Owner.topHiddenItems * GetItemSize();
				return Owner.LoopedListAvailable ? size : Mathf.Max(0, size);
			}

			/// <summary>
			/// Calculates the size of the bottom filler.
			/// </summary>
			/// <returns>The bottom filler size.</returns>
			public override float CalculateBottomFillerSize()
			{
				return Mathf.Max(0, (Owner.DataSource.Count - Owner.DisplayedIndicesLast - 1) * GetItemSize());
			}

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var first_visible_index = strict
					? Mathf.CeilToInt(Owner.GetScrollValue() / GetItemSize())
					: Mathf.FloorToInt(Owner.GetScrollValue() / GetItemSize());
				if (Owner.LoopedListAvailable)
				{
					return first_visible_index;
				}

				first_visible_index = Mathf.Max(0, first_visible_index);
				if (strict)
				{
					return first_visible_index;
				}

				return Mathf.Min(first_visible_index, Mathf.Max(0, Owner.DataSource.Count - Owner.visibleItems));
			}

			/// <summary>
			/// Gets the last visible index.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				var window = Owner.GetScrollValue() + Owner.GetScrollSize();
				var last_visible_index = strict
					? Mathf.FloorToInt(window / GetItemSize())
					: Mathf.CeilToInt(window / GetItemSize());

				return last_visible_index - 1;
			}

			/// <summary>
			/// Gets the item position.
			/// </summary>
			/// <returns>The item position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPosition(int index)
			{
				var block_index = Owner.GetBlockIndex(index);
				return block_index * GetItemSize();
			}

			/// <summary>
			/// Gets the item middle position by index.
			/// </summary>
			/// <returns>The item middle position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionMiddle(int index)
			{
				return GetItemPosition(index) - (Owner.GetScrollSize() / 2) + (GetItemSize() / 2);
			}

			/// <summary>
			/// Gets the item position bottom.
			/// </summary>
			/// <returns>The item position bottom.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionBottom(int index)
			{
				return GetItemPosition(index) + GetItemSize() + Owner.LayoutBridge.GetMargin() - Owner.LayoutBridge.GetSpacing() - Owner.GetScrollSize();
			}

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			/// <returns>Maximum count of the visible items.</returns>
			public override int GetMaxVisibleItems()
			{
				var max = Owner.IsHorizontal()
					? Mathf.CeilToInt(Owner.ScrollRectSize.x / Owner.ItemSize.x)
					: Mathf.CeilToInt(Owner.ScrollRectSize.y / Owner.ItemSize.y);

				return Mathf.Max(max, 1) + 1;
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

				var pos = Owner.IsHorizontal() ? point.x : -point.y;
				var index = Mathf.RoundToInt(pos / GetItemSize());

				return Mathf.Min(index, Owner.DataSource.Count);
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			public override int GetNearestItemIndex()
			{
				return Mathf.Clamp(Mathf.RoundToInt(Owner.GetScrollValue() / GetItemSize()), 0, Owner.DataSource.Count - 1);
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

				var blocks = Mathf.CeilToInt((float)Owner.DataSource.Count / (float)Owner.GetItemsPerBlock());
				return (blocks * GetItemSize()) - Owner.LayoutBridge.GetSpacing();
			}

			/// <summary>
			/// Validates the content size and item size.
			/// </summary>
			public override void ValidateContentSize()
			{
				var spacing_x = Owner.GetItemSpacingX();
				var spacing_y = Owner.GetItemSpacingY();

				var height = Owner.ScrollRectSize.y + spacing_y - Owner.LayoutBridge.GetFullMarginY();
				var width = Owner.ScrollRectSize.x + spacing_x - Owner.LayoutBridge.GetFullMarginX();

				int per_block;
				if (Owner.IsHorizontal())
				{
					per_block = Mathf.FloorToInt(height / (Owner.ItemSize.y + spacing_y));
					per_block = Mathf.Max(1, per_block);
					per_block = Owner.LayoutBridge.RowsConstraint(per_block);
				}
				else
				{
					per_block = Mathf.FloorToInt(width / (Owner.ItemSize.x + spacing_x));
					per_block = Mathf.Max(1, per_block);
					per_block = Owner.LayoutBridge.ColumnsConstraint(per_block);
				}

				if (per_block > 1)
				{
					// Debug.LogWarning("More that one item per row or column, consider change DefaultItem size or set layout constraint or use TileViewWithFixedSize (TileViewWithVariableSize)", Owner);
				}
			}
		}
	}
}