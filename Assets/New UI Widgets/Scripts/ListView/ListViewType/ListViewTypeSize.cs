namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
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
		protected class ListViewTypeSize : ListViewTypeBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeSize"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public ListViewTypeSize(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
			}

			/// <summary>
			/// Items sizes.
			/// </summary>
			protected readonly Dictionary<TItem, Vector2> ItemSizes = new Dictionary<TItem, Vector2>();

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
			protected virtual int GetMaxVisibleItems(ObservableList<TItem> items)
			{
				if (items.Count == 0)
				{
					return 0;
				}

				var spacing = Owner.LayoutBridge.GetSpacing();
				var min = MinSize(items);

				var size = Owner.GetScrollSize();
				var result = 0;
				for (; ;)
				{
					size -= min;
					if (result > 0)
					{
						size -= spacing;
					}

					if (size < 0)
					{
						break;
					}

					result += 1;
				}

				return result + 2;
			}

			/// <summary>
			/// Get the minimal size of the specified items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <returns>Minimal size.</returns>
			protected virtual float MinSize(ObservableList<TItem> items)
			{
				if (items.Count == 0)
				{
					return 0f;
				}

				var result = GetItemSize(items[0]);

				for (int i = 1; i < items.Count; i++)
				{
					result = Mathf.Min(result, GetItemSize(items[i]));
				}

				return result;
			}

			/// <summary>
			/// Gets the size of the item.
			/// </summary>
			/// <returns>The item size.</returns>
			/// <param name="index">Item index.</param>
			protected float GetItemSize(int index)
			{
				return GetItemSize(Owner.DataSource[index]);
			}

			/// <summary>
			/// Gets the size of the item.
			/// </summary>
			/// <returns>The item size.</returns>
			/// <param name="item">Item.</param>
			protected float GetItemSize(TItem item)
			{
				return Owner.IsHorizontal() ? ItemSizes[item].x : ItemSizes[item].y;
			}

			/// <summary>
			/// Calculates the size of the top filler.
			/// </summary>
			/// <returns>The top filler size.</returns>
			public override float CalculateTopFillerSize()
			{
				return GetItemsSize(0, Owner.topHiddenItems);
			}

			/// <summary>
			/// Calculates the size of the bottom filler.
			/// </summary>
			/// <returns>The bottom filler size.</returns>
			public override float CalculateBottomFillerSize()
			{
				var last = Owner.DisplayedIndicesLast;
				return last < 0 ? 0f : GetItemsSize(last, Owner.DataSource.Count - last - 1);
			}

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var first_visible_index = Mathf.Max(0, GetIndexAtPosition(Owner.GetScrollValue()));

				if (Owner.LoopedListAvailable)
				{
					return first_visible_index;
				}

				if (strict)
				{
					return first_visible_index;
				}

				return Mathf.Min(first_visible_index, Mathf.Max(0, Owner.DataSource.Count - Owner.visibleItems));
			}

			/// <summary>
			/// Gets the last index of the visible.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				var last_visible_index = GetIndexAtPosition(Owner.GetScrollValue() + Owner.GetScrollSize());

				return strict ? last_visible_index : last_visible_index + 2;
			}

			/// <summary>
			/// Gets the width of the items.
			/// </summary>
			/// <returns>The items width.</returns>
			/// <param name="start">Start index.</param>
			/// <param name="count">Items count.</param>
			protected float GetItemsSize(int start, int count)
			{
				if (count == 0)
				{
					return 0f;
				}

				var width = 0f;
				var n = Owner.LoopedListAvailable ? start + count : Mathf.Min(start + count, Owner.DataSource.Count);
				for (int i = start; i < n; i++)
				{
					width += GetItemSize(Owner.DataSource[Owner.VisibleIndex2ItemIndex(i)]);
				}

				return Mathf.Max(0, width + (Owner.LayoutBridge.GetSpacing() * (count - 1)));
			}

			/// <summary>
			/// Gets the item position.
			/// </summary>
			/// <returns>The item position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPosition(int index)
			{
				var n = Mathf.Min(index, Owner.DataSource.Count);
				var size = 0f;
				for (int i = 0; i < n; i++)
				{
					size += GetItemSize(Owner.DataSource[i]);
				}

				return size + (Owner.LayoutBridge.GetSpacing() * index);
			}

			/// <summary>
			/// Gets the item middle position by index.
			/// </summary>
			/// <returns>The item middle position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionMiddle(int index)
			{
				return GetItemPosition(index) - (Owner.GetScrollSize() / 2) + (GetItemSize(index) / 2);
			}

			/// <summary>
			/// Gets the item position bottom.
			/// </summary>
			/// <returns>The item position bottom.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionBottom(int index)
			{
				return GetItemPosition(index) + GetItemSize(index) + Owner.LayoutBridge.GetMargin() - Owner.LayoutBridge.GetSpacing() - Owner.GetScrollSize();
			}

			List<TItem> ItemsToRemove = new List<TItem>();

			/// <summary>
			/// Remove old items from saved sizes.
			/// </summary>
			/// <param name="items">New items.</param>
			protected virtual void RemoveOldItems(ObservableList<TItem> items)
			{
				foreach (var item in ItemSizes.Keys)
				{
					if (!items.Contains(item))
					{
						ItemsToRemove.Add(item);
					}
				}

				foreach (var item in ItemsToRemove)
				{
					ItemSizes.Remove(item);
				}

				ItemsToRemove.Clear();
			}

			/// <summary>
			/// Calculate and sets the width of the items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
			public override void CalculateItemsSizes(ObservableList<TItem> items, bool forceUpdate = true)
			{
				RemoveOldItems(items);

				if (Owner.PrecalculateItemSizes)
				{
					Owner.DefaultItemCopy.gameObject.SetActive(true);

					if (Owner.DefaultItemLayout == null)
					{
						Owner.DefaultItemLayout = Owner.DefaultItemCopy.GetComponent<LayoutGroup>();

						Owner.DefaultItemNestedLayouts = Owner.DefaultItemCopy.GetComponentsInChildren<LayoutGroup>();
						Array.Reverse(Owner.DefaultItemNestedLayouts);
					}

					if (forceUpdate)
					{
						items.ForEach(x => ItemSizes[x] = CalculateComponentSize(x));
					}
					else
					{
						foreach (var item in items)
						{
							if (!ItemSizes.ContainsKey(item))
							{
								ItemSizes[item] = CalculateComponentSize(item);
							}
						}
					}

					Owner.DefaultItemCopy.gameObject.SetActive(false);
				}
				else
				{
					if (forceUpdate)
					{
						items.ForEach(x => ItemSizes[x] = Owner.ItemSize);
					}
					else
					{
						foreach (var item in items)
						{
							if (!ItemSizes.ContainsKey(item))
							{
								ItemSizes[item] = Owner.ItemSize;
							}
						}
					}
				}
			}

			/// <summary>
			/// Gets the the index of the item at he specified position.
			/// </summary>
			/// <returns>The index.</returns>
			/// <param name="position">Position.</param>
			int GetIndexAtPosition(float position)
			{
				var spacing = Owner.LayoutBridge.GetSpacing();

				var result = 0;
				for (int index = 0; index < Owner.DataSource.Count; index++)
				{
					position -= GetItemSize(Owner.DataSource[index]);
					if (index > 0)
					{
						position -= spacing;
					}

					if (position < 0)
					{
						break;
					}

					result += 1;
				}

				if (result >= Owner.DataSource.Count)
				{
					result = Owner.DataSource.Count - 1;
				}

				return result;
			}

			/// <summary>
			/// Adds the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public override void AddCallback(ListViewItem item)
			{
				item.onResize.AddListener(OnItemSizeChanged);
			}

			/// <summary>
			/// Removes the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public override void RemoveCallback(ListViewItem item)
			{
				item.onResize.RemoveListener(OnItemSizeChanged);
			}

			/// <summary>
			/// Handle component size changed event.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <param name="size">New size.</param>
			protected void OnItemSizeChanged(int index, Vector2 size)
			{
				UpdateItemSize(index, size);
			}

			/// <summary>
			/// Update saved size of item.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <param name="newSize">New size.</param>
			/// <returns>true if size different; otherwise, false.</returns>
			protected virtual bool UpdateItemSize(int index, Vector2 newSize)
			{
				if (index < 0)
				{
					return false;
				}

				var current_size = ItemSizes[Owner.DataSource[index]];

				if (current_size == newSize)
				{
					return false;
				}

				ItemSizes[Owner.DataSource[index]] = newSize;

				if (GetMaxVisibleItems(Owner.DataSource) > Owner.maxVisibleItems)
				{
					Owner.NeedResize = true;
				}
				else
				{
					Owner.ScrollUpdate();
				}

				return true;
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
				var index = GetIndexAtPosition(pos_block);

				if (index != (Owner.DataSource.Count - 1))
				{
					var size = GetItemsSize(0, index);
					var top = pos_block - size;
					var bottom = pos_block - (size + GetItemSize(index + 1) + Owner.LayoutBridge.GetSpacing());
					if (bottom < top)
					{
						index += 1;
					}
				}

				return Mathf.Min(index, Owner.DataSource.Count - 1);
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			public override int GetNearestItemIndex()
			{
				return GetIndexAtPosition(Owner.GetScrollValue());
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

				var size = 0f;
				foreach (var item in Owner.DataSource)
				{
					size += GetItemSize(item);
				}

				return size + (Owner.DataSource.Count * Owner.LayoutBridge.GetSpacing()) - Owner.LayoutBridge.GetSpacing();
			}
		}
	}
}