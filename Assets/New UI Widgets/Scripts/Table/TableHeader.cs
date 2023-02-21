namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Table Header.
	/// How to change DefaultItem:
	/// var order = tableHeader.GetColumnsOrder();
	/// tableHeader.RestoreColumnsOrder();
	/// listView.DefaultItem = newDefaultItem;
	/// tableHeader.SetColumnsOrder(order);
	/// </summary>
	[RequireComponent(typeof(LayoutGroup))]
	[AddComponentMenu("UI/New UI Widgets/Table Header")]
	public class TableHeader : MonoBehaviour, IDropSupport<TableHeaderDragCell>, IPointerEnterHandler, IPointerExitHandler, IStylable
	{
		/// <summary>
		/// ListView instance.
		/// </summary>
		[SerializeField]
		public ListViewBase List;

		/// <summary>
		/// Allow resize.
		/// </summary>
		[SerializeField]
		public bool AllowResize = true;

		/// <summary>
		/// Allow reorder.
		/// </summary>
		[SerializeField]
		public bool AllowReorder = true;

		/// <summary>
		/// Is now processed cell reorder?
		/// </summary>
		[NonSerialized]
		[HideInInspector]
		public bool ProcessCellReorder = false;

		/// <summary>
		/// Update ListView columns width on drag.
		/// </summary>
		[SerializeField]
		public bool OnDragUpdate = true;

		/// <summary>
		/// The active region in points from left or right border where resize allowed.
		/// </summary>
		[SerializeField]
		public float ActiveRegion = 5;

		/// <summary>
		/// The current camera. For Screen Space - Overlay let it empty.
		/// </summary>
		[SerializeField]
		public Camera CurrentCamera;

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D CursorTexture;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 CursorHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D AllowDropCursor;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 AllowDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D DeniedDropCursor;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 DeniedDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// The drop indicator.
		/// </summary>
		[SerializeField]
		public LayoutDropIndicator DropIndicator;

		RectTransform rectTransform;

		/// <summary>
		/// Gets the rect transform.
		/// </summary>
		/// <value>The rect transform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// The cells info.
		/// </summary>
		protected List<TableHeaderCellInfo> CellsInfo = new List<TableHeaderCellInfo>();

		/// <summary>
		/// Header cells.
		/// </summary>
		/// <value>The cells.</value>
		public RectTransform[] Cells
		{
			get
			{
				var result = new RectTransform[CellsInfo.Count];

				for (int i = 0; i < CellsInfo.Count; i++)
				{
					result[i] = CellsInfo[i].Rect;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets a value indicating whether mouse position in active region.
		/// </summary>
		/// <value><c>true</c> if in active region; otherwise, <c>false</c>.</value>
		public bool InActiveRegion
		{
			get
			{
				return CheckInActiveRegion(Input.mousePosition, CurrentCamera);
			}
		}

		/// <summary>
		/// Is cursor over gameobject?
		/// </summary>
		protected bool IsCursorOver;

		Canvas canvas;

		RectTransform canvasRect;

		LayoutElement leftTarget;
		LayoutElement rightTarget;
		bool processDrag;

		LayoutGroup layout;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Refresh();
		}

		/// <summary>
		/// Raises the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			// Init();
		}

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ShowDropIndicator(int index)
		{
			if (DropIndicator != null)
			{
				DropIndicator.Show(index, RectTransform);
			}
		}

		/// <summary>
		/// Hides the drop indicator.
		/// </summary>
		protected virtual void HideDropIndicator()
		{
			if (DropIndicator != null)
			{
				DropIndicator.Hide();
			}
		}

		/// <summary>
		/// Restore initial cells order.
		/// </summary>
		[Obsolete("Renamed to RestoreColumnOrder().")]
		public void RestoreOrder()
		{
			RestoreColumnsOrder();
		}

		/// <summary>
		/// Get the columns order.
		/// </summary>
		/// <returns>Columns order.</returns>
		public List<int> GetColumnsOrder()
		{
			var result = new List<int>(CellsInfo.Count);

			GetColumnsOrder(result);

			return result;
		}

		/// <summary>
		/// Get the columns order.
		/// </summary>
		/// <param name="order">Columns order.</param>
		public void GetColumnsOrder(List<int> order)
		{
			order.Clear();

			foreach (var cell in CellsInfo)
			{
				order.Add(cell.Position);
			}
		}

		/// <summary>
		/// Set the columns order.
		/// </summary>
		/// <param name="order">New columns order.</param>
		public void SetColumnsOrder(List<int> order)
		{
			// restore original order
			RestoreColumnsOrder();

			// convert list of the new positions to list of the old positions
			var reverse_order = new List<int>();
			for (int i = 0; i < order.Count; i++)
			{
				reverse_order.Add(order.IndexOf(i));
			}

			// todo set cells order
			var cells = new List<Transform>();

			// restore list components cells order
			List.Init();
			List.ForEachComponent(component =>
			{
				cells.Clear();
				foreach (Transform child in component.transform)
				{
					cells.Add(child);
				}

				for (int i = 0; i < reverse_order.Count; i++)
				{
					cells[i].SetAsLastSibling();
				}
			});

			for (int new_position = 0; new_position < reverse_order.Count; new_position++)
			{
				var old_position = reverse_order[new_position];
				CellsInfo[old_position].Position = new_position;
				CellsInfo[old_position].Rect.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Restore initial cells order.
		/// </summary>
		public void RestoreColumnsOrder()
		{
			// restore order
			if ((List != null) && (CellsInfo != null) && (CellsInfo.Count > 1))
			{
				var cells = new List<Transform>();

				// restore list components cells order
				List.Init();
				List.ForEachComponent(component =>
				{
					cells.Clear();
					foreach (Transform child in component.transform)
					{
						cells.Add(child);
					}

					foreach (var cell in CellsInfo)
					{
						cells[cell.Position].SetAsLastSibling();
					}
				});

				// restore header cells order
				CellsInfo.ForEach((x, i) =>
				{
					x.Position = i;
					x.Rect.SetSiblingIndex(i);
				});
			}
		}

		/// <summary>
		/// Reinit this instance in case if you remove or add cells manually.
		/// </summary>
		public void Reinit()
		{
			RestoreColumnsOrder();

			// clear cells list
			CellsInfo.Clear();

			// clear cell settings and events
			foreach (Transform child in transform)
			{
				transform.gameObject.SetActive(true);

				var cell = Utilites.GetOrAddComponent<TableHeaderDragCell>(child);
				cell.Position = -1;

				var events = Utilites.GetOrAddComponent<TableHeaderCell>(child);
				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}

			Refresh();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			canvasRect = Utilites.FindTopmostCanvas(transform) as RectTransform;
			canvas = canvasRect.GetComponent<Canvas>();

			foreach (Transform child in transform)
			{
				var cell = Utilites.GetOrAddComponent<TableHeaderDragCell>(child);

				if (cell.Position == -1)
				{
					cell.Position = CellsInfo.Count;
					cell.TableHeader = this;
					cell.AllowDropCursor = AllowDropCursor;
					cell.AllowDropCursorHotSpot = AllowDropCursorHotSpot;
					cell.DeniedDropCursor = DeniedDropCursor;
					cell.DeniedDropCursorHotSpot = DeniedDropCursorHotSpot;

					var events = Utilites.GetOrAddComponent<TableHeaderCell>(child);
					events.OnInitializePotentialDragEvent.AddListener(OnInitializePotentialDrag);
					events.OnBeginDragEvent.AddListener(OnBeginDrag);
					events.OnDragEvent.AddListener(OnDrag);
					events.OnEndDragEvent.AddListener(OnEndDrag);

					CellsInfo.Add(new TableHeaderCellInfo()
					{
						Rect = child as RectTransform,
						LayoutElement = Utilites.GetOrAddComponent<LayoutElement>(child),
						Position = CellsInfo.Count,
					});
				}
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerEnter event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			IsCursorOver = true;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerExit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOver = false;

			cursorChanged = false;
			Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Compatibility.GetCursorMode());
		}

		/// <summary>
		/// Is cursor changed?
		/// </summary>
		protected bool cursorChanged;

		/// <summary>
		/// Update cursors.
		/// </summary>
		protected virtual void LateUpdate()
		{
			if (!AllowResize)
			{
				return;
			}

			if (!IsCursorOver)
			{
				return;
			}

			if (processDrag || ProcessCellReorder)
			{
				return;
			}

			if ((CursorTexture == null) || (!Input.mousePresent))
			{
				return;
			}

			if (InActiveRegion)
			{
				Cursor.SetCursor(CursorTexture, CursorHotSpot, Compatibility.GetCursorMode());
				cursorChanged = true;
			}
			else if (cursorChanged)
			{
				Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Compatibility.GetCursorMode());
				cursorChanged = false;
			}
		}

		/// <summary>
		/// Check if event happened in active region.
		/// </summary>
		/// <param name="eventData">Event data</param>
		/// <returns>True if event happened in active region; otherwise false.</returns>
		public virtual bool CheckInActiveRegion(PointerEventData eventData)
		{
			return CheckInActiveRegion(eventData.pressPosition, eventData.pressEventCamera);
		}

		/// <summary>
		/// Check if cursor in active region to resize.
		/// </summary>
		/// <param name="position">Cursor position.</param>
		/// <param name="currentCamera">Current camera.</param>
		/// <returns>true if cursor in active region to resize; otherwise, false.</returns>
		protected virtual bool CheckInActiveRegion(Vector2 position, Camera currentCamera)
		{
			Vector2 point;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, position, currentCamera, out point))
			{
				return false;
			}

			var rect = RectTransform.rect;
			if (!rect.Contains(point))
			{
				return false;
			}

			point += new Vector2(rect.width * RectTransform.pivot.x, 0);

			int i = 0;
			foreach (var cell in CellsInfo.OrderBy(x => x.Position))
			{
				if (!cell.ActiveSelf)
				{
					i++;
					continue;
				}

				if (GetTargetIndex(i, -1) != -1)
				{
					if (CheckLeft(cell.Rect, point))
					{
						return true;
					}
				}

				if (GetTargetIndex(i, +1) != -1)
				{
					if (CheckRight(cell.Rect, point))
					{
						return true;
					}
				}

				i++;
			}

			return false;
		}

		float widthLimit;

		/// <summary>
		/// Raises the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!AllowResize)
			{
				return;
			}

			if (ProcessCellReorder)
			{
				return;
			}

			Vector2 point;
			processDrag = false;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out point))
			{
				return;
			}

			var r = RectTransform.rect;
			point += new Vector2(r.width * RectTransform.pivot.x, 0);

			foreach (var cell in CellsInfo.OrderBy(x => x.Position))
			{
				var i = cell.Position;
				if (!cell.ActiveSelf)
				{
					continue;
				}

				if (CheckLeft(cell.Rect, point))
				{
					var left = GetTargetIndex(i, -1);
					if (left != -1)
					{
						processDrag = true;

						var left_cell = GetCellInfo(left);
						leftTarget = left_cell.LayoutElement;
						rightTarget = cell.LayoutElement;
						widthLimit = left_cell.Width + cell.Width;
						return;
					}
				}

				if (CheckRight(cell.Rect, point))
				{
					var right = GetTargetIndex(i, +1);
					if (right != -1)
					{
						processDrag = true;

						var right_cell = GetCellInfo(right);

						leftTarget = cell.LayoutElement;
						rightTarget = right_cell.LayoutElement;
						widthLimit = cell.Width + right_cell.Width;
						return;
					}
				}
			}
		}

		TableHeaderCellInfo GetCellInfo(int position)
		{
			foreach (var cell in CellsInfo)
			{
				if (cell.Position == position)
				{
					return cell;
				}
			}

			return null;
		}

		int GetTargetIndex(int index, int direction)
		{
			if ((index + direction) == -1)
			{
				return -1;
			}

			if ((index + direction) == CellsInfo.Count)
			{
				return -1;
			}

			var is_active = GetCellInfo(index + direction).ActiveSelf;

			var result = is_active
				? index + direction
				: GetTargetIndex(index + direction, direction);

			return result;
		}

		/// <summary>
		/// Checks if point in the left region.
		/// </summary>
		/// <returns><c>true</c>, if point in the left region, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">RectTransform.</param>
		/// <param name="point">Point.</param>
		bool CheckLeft(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;
			r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
			r.width = ActiveRegion;

			return r.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">Child rect transform.</param>
		/// <param name="point">Point.</param>
		bool CheckRight(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;

			r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
			r.position = new Vector2(r.position.x + r.width - ActiveRegion - 1, r.position.y);
			r.width = ActiveRegion + 1;

			return r.Contains(point);
		}

		/// <summary>
		/// Raises the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Compatibility.GetCursorMode());

			ResetChildren();
			if (!OnDragUpdate)
			{
				Resize();
			}

			processDrag = false;
		}

		/// <summary>
		/// Raises the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			if (canvas == null)
			{
				throw new MissingComponentException(gameObject.name + " not in Canvas hierarchy.");
			}

			Cursor.SetCursor(CursorTexture, CursorHotSpot, Compatibility.GetCursorMode());

			Vector2 p1;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, CurrentCamera, out p1);
			Vector2 p2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position - eventData.delta, CurrentCamera, out p2);
			var delta = p1 - p2;

			if (delta.x > 0)
			{
				leftTarget.preferredWidth = Mathf.Min(leftTarget.preferredWidth + delta.x, widthLimit - rightTarget.minWidth);
				rightTarget.preferredWidth = widthLimit - leftTarget.preferredWidth;
			}
			else
			{
				rightTarget.preferredWidth = Mathf.Min(rightTarget.preferredWidth - delta.x, widthLimit - leftTarget.minWidth);
				leftTarget.preferredWidth = widthLimit - rightTarget.preferredWidth;
			}

			LayoutUtilites.UpdateLayout(layout);

			if (OnDragUpdate)
			{
				Resize();
			}
		}

		/// <summary>
		/// Resets the children widths.
		/// </summary>
		void ResetChildren()
		{
			CellsInfo.ForEach(ResetChildrenWidth);
		}

		void ResetChildrenWidth(TableHeaderCellInfo cell)
		{
			cell.LayoutElement.preferredWidth = cell.Rect.rect.width;
		}

		/// <summary>
		/// Resize items in ListView.
		/// </summary>
		public void Resize()
		{
			if (List == null)
			{
				return;
			}

			if (CellsInfo.Count < 2)
			{
				return;
			}

			List.Init();
			List.ForEachComponent(ResizeComponent);
		}

		/// <summary>
		/// Resizes the game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="index">The index.</param>
		void ResizeGameObject(GameObject go, int index)
		{
			var cell = GetCellInfo(index);

			(go.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cell.Width);

			var layoutElement = go.GetComponent<LayoutElement>();
			if (layoutElement)
			{
				layoutElement.minWidth = cell.LayoutElement.minWidth;
				layoutElement.flexibleWidth = cell.LayoutElement.flexibleWidth;
				layoutElement.preferredWidth = cell.Width;
			}
		}

		/// <summary>
		/// Resizes the component.
		/// </summary>
		/// <param name="component">Component.</param>
		void ResizeComponent(ListViewItem component)
		{
			for (int i = 0; i < component.transform.childCount; i++)
			{
				ResizeGameObject(component.transform.GetChild(i).gameObject, i);
			}
		}

		static void ReorderComponent(ListViewItem component, int prevPosition, int newPosition)
		{
			var target = component.transform.GetChild(prevPosition);
			target.SetSiblingIndex(newPosition);
		}

		/// <summary>
		/// Move column from oldColumnPosition to newColumnPosition.
		/// </summary>
		/// <param name="oldColumnPosition">Old column position.</param>
		/// <param name="newColumnPosition">New column position.</param>
		public void Reorder(int oldColumnPosition, int newColumnPosition)
		{
			if (CellsInfo.Count < 2)
			{
				return;
			}

			if (List != null)
			{
				List.Init();
				List.ForEachComponent(x => ReorderComponent(x, CellsInfo[oldColumnPosition].Position, CellsInfo[newColumnPosition].Position));
			}

			var target = CellsInfo[oldColumnPosition].Rect;
			target.SetSiblingIndex(CellsInfo[newColumnPosition].Position);

			CellsInfo.ForEach(x => x.Position = x.Rect.GetSiblingIndex());
		}

		#region IDropSupport<TableHeaderCell>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TableHeaderDragCell data, PointerEventData eventData)
		{
			if (!AllowReorder)
			{
				return false;
			}

			var target = FindTarget(eventData);
			if ((target == null) || (target.GetInstanceID() == data.GetInstanceID()))
			{
				return false;
			}

			ShowDropIndicator(CellsInfo[target.Position].Position);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TableHeaderDragCell data, PointerEventData eventData)
		{
			HideDropIndicator();

			var target = FindTarget(eventData);

			Reorder(data.Position, target.Position);
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TableHeaderDragCell data, PointerEventData eventData)
		{
			HideDropIndicator();
		}

		/// <summary>
		/// Change value position in specified list.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="oldPosition">Old position.</param>
		/// <param name="newPosition">New position.</param>
		protected static void ChangePosition<T>(List<T> list, int oldPosition, int newPosition)
		{
			var item = list[oldPosition];
			list.RemoveAt(oldPosition);
			list.Insert(newPosition, item);
		}

		List<RaycastResult> raycastResults = new List<RaycastResult>();

		/// <summary>
		/// Get TableHeaderDragCell in position specified with event data.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>TableHeaderDragCell if found; otherwise null.</returns>
		protected TableHeaderDragCell FindTarget(PointerEventData eventData)
		{
			raycastResults.Clear();

			EventSystem.current.RaycastAll(eventData, raycastResults);

			foreach (var raycastResult in raycastResults)
			{
				if (!raycastResult.isValid)
				{
					continue;
				}

				var target = raycastResult.gameObject.GetComponent<TableHeaderDragCell>();
				if ((target != null) && target.transform.IsChildOf(transform))
				{
					return target;
				}
			}

			return null;
		}
		#endregion

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			CellsInfo.Clear();
			foreach (Transform child in transform)
			{
				var events = child.GetComponent<TableHeaderCell>();
				if (events == null)
				{
					continue;
				}

				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}
		}

		/// <summary>
		/// Refresh header.
		/// </summary>
		public void Refresh()
		{
			if (layout == null)
			{
				layout = GetComponent<LayoutGroup>();
			}

			if (layout != null)
			{
				LayoutUtilites.UpdateLayout(layout);
			}

			Init();

			ResetChildren();
			Resize();
		}

		/// <summary>
		/// Change column state.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="active">If set state to active.</param>
		public void ColumnToggle(int index, bool active)
		{
			var target = CellsInfo[index];
			target.Rect.gameObject.SetActive(active);

			List.ForEachComponent(component =>
			{
				var child = component.transform.GetChild(target.Position);
				child.gameObject.SetActive(false);
			});

			List.ComponentsColoring();

			Refresh();
		}

		/// <summary>
		/// Disable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ColumnDisable(int index)
		{
			ColumnToggle(index, false);
		}

		/// <summary>
		/// Enable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ColumnEnable(int index)
		{
			ColumnToggle(index, true);
		}

		/// <summary>
		/// Add header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void AddCell(GameObject cell)
		{
			cell.transform.SetParent(transform, false);
			cell.SetActive(true);

			Refresh();
		}

		/// <summary>
		/// Remove header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		/// <param name="parent">Parent.</param>
		public void RemoveCell(GameObject cell, RectTransform parent = null)
		{
			var index = CellsInfo.FindIndex(x => x.Rect.gameObject == cell);
			var cell_info = CellsInfo[index];
			if (index == -1)
			{
				Debug.LogWarning("Cell not in header", cell);
				return;
			}

			cell.SetActive(false);
			cell.transform.SetParent(parent, false);
			if (parent == null)
			{
				Destroy(cell);
			}

			// remove from cells
			CellsInfo.RemoveAt(index);

			// remove events
			var events = Utilites.GetOrAddComponent<TableHeaderCell>(cell);
			events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
			events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
			events.OnDragEvent.RemoveListener(OnDrag);
			events.OnEndDragEvent.RemoveListener(OnEndDrag);

			// decrease position for cells where >cell_position
			CellsInfo.ForEach(x =>
			{
				if (x.Position > cell_info.Position)
				{
					x.Position -= 1;
				}
			});

			// update list widths
			LayoutUtilites.UpdateLayout(layout);
			Resize();
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			// apply style for header
			style.Table.Border.ApplyTo(gameObject);

			foreach (Transform cell in transform)
			{
				var style_support = cell.GetComponent<StyleSupportHeaderCell>();

				if (style_support != null)
				{
					style_support.SetStyle(style);
				}
				else
				{
					style.Table.Background.ApplyTo(cell);
					style.Table.HeaderText.ApplyTo(cell.Find("Text"));
				}
			}

			// apply style to list
			style.Table.Border.ApplyTo(List.Container);

			List.ForEachComponent(component =>
			{
				style.Table.Border.ApplyTo(component);

				foreach (Transform child in component.transform)
				{
					style.Table.Background.ApplyTo(child.gameObject);
				}
			});

			return true;
		}
		#endregion
	}
}