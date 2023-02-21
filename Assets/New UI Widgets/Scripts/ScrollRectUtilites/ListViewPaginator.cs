namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ListView's paginator. Also works with TileView's.
	/// </summary>
	public class ListViewPaginator : ScrollRectPaginator
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		protected ListViewBase ListView;

		/// <summary>
		/// Count of items on one page.
		/// </summary>
		[SerializeField]
		protected int perPage = 1;

		/// <summary>
		/// Gets or sets the count of items on one page.
		/// </summary>
		/// <value>The per page.</value>
		public int PerPage
		{
			get
			{
				return Mathf.Max(1, perPage);
			}

			set
			{
				perPage = Mathf.Max(1, value);
				RecalculatePages();
			}
		}

		bool isListViewPaginatorInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void Init()
		{
			if (isListViewPaginatorInited)
			{
				return;
			}

			isListViewPaginatorInited = true;

			ListView.Init();
			ScrollRect = ListView.GetScrollRect();

			base.Init();
		}

		/// <summary>
		/// Handle scroll changes.
		/// </summary>
		protected override void ScrollChanged()
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			var distance = Mathf.Abs(IsHorizontal() ? DragDelta.x : DragDelta.y);
			var time = Utilites.GetUnscaledTime() - DragStarted;

			var is_fast = (distance >= FastDragDistance) && (time <= FastDragTime);
			if (!is_fast)
			{
				var page = Mathf.FloorToInt(((float)ListView.GetNearestItemIndex()) / (ListView.GetItemsPerBlock() * PerPage));
				GoToPage(page, true);

				DragDelta = Vector2.zero;
				DragStarted = 0f;
			}
			else
			{
				var direction = IsHorizontal() ? DragDelta.x : -DragDelta.y;
				DragDelta = Vector2.zero;
				if (direction == 0f)
				{
					return;
				}

				var page = direction < 0 ? CurrentPage + 1 : CurrentPage - 1;
				GoToPage(page, false);
			}
		}

		/// <summary>
		/// Determines whether direction is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected override bool IsHorizontal()
		{
			if (ScrollRect.horizontal)
			{
				return true;
			}

			if (ScrollRect.vertical)
			{
				return false;
			}

			var rect = ScrollRect.content.rect;

			return rect.width >= rect.height;
		}

		/// <summary>
		/// Recalculate the pages count.
		/// </summary>
		protected override void RecalculatePages()
		{
			SetScrollRectMaxDrag();

			var per_block = ListView.GetItemsPerBlock() * PerPage;

			Pages = (per_block == 0) ? 0 : Mathf.CeilToInt(((float)ListView.GetItemsCount()) / per_block);
			if (currentPage >= Pages)
			{
				GoToPage(Pages - 1);
			}
		}

		/// <summary>
		/// Gets the page position.
		/// </summary>
		/// <returns>The page position.</returns>
		/// <param name="page">Page.</param>
		protected override float GetPagePosition(int page)
		{
			var pos = ListView.GetItemPosition(page * ListView.GetItemsPerBlock() * PerPage);

			return IsHorizontal() ? -pos : pos;
		}
	}
}