namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag support.
	/// Drop component should implement IDropSupport{T} with same type.
	/// </summary>
	/// <typeparam name="T">Type of draggable data.</typeparam>
	public abstract class DragSupport<T> : BaseDragSupport, IBeginDragHandler, IEndDragHandler, IDragHandler, ICancelHandler
	{
		/// <summary>
		/// Process the cancel event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnCancel(BaseEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			if (currentTarget != null)
			{
				currentTarget.DropCanceled(Data, null);
			}

			Dropped(false);

			IsDragged = false;
			Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Compatibility.GetCursorMode());
		}

		/// <summary>
		/// Gets or sets the data.
		/// Data will be passed to Drop component.
		/// </summary>
		/// <value>The data.</value>
		public T Data
		{
			get;
			protected set;
		}

		/// <summary>
		/// The Allow drop cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D AllowDropCursor;

		/// <summary>
		/// The Allow drop cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 AllowDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The Denied drop cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D DeniedDropCursor;

		/// <summary>
		/// The Denied drop cursor hot spot.
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
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		public virtual bool CanDrag(PointerEventData eventData)
		{
			return true;
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected abstract void InitDrag(PointerEventData eventData);

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public virtual void Dropped(bool success)
		{
			Data = default(T);
		}

		/// <summary>
		/// If this object is dragged?
		/// </summary>
		protected bool IsDragged;

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (CanDrag(eventData))
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
				IsDragged = true;
				InitDrag(eventData);
			}
		}

		/// <summary>
		/// The old drop target.
		/// </summary>
		protected IDropSupport<T> oldTarget;

		/// <summary>
		/// The current drop target.
		/// </summary>
		protected IDropSupport<T> currentTarget;

		/// <summary>
		/// When draging is occuring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			oldTarget = currentTarget;

			currentTarget = FindTarget(eventData);
			if ((oldTarget != null) && (currentTarget != oldTarget))
			{
				oldTarget.DropCanceled(Data, eventData);
			}

			if (currentTarget != null)
			{
				// set cursor can drop
				Cursor.SetCursor(AllowDropCursor, AllowDropCursorHotSpot, Compatibility.GetCursorMode());
			}
			else
			{
				// set cursor fail drop
				Cursor.SetCursor(DeniedDropCursor, DeniedDropCursorHotSpot, Compatibility.GetCursorMode());
			}

			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTransform as RectTransform, Input.mousePosition, eventData.pressEventCamera, out point))
			{
				return;
			}

			DragPoint.localPosition = point;
		}

		List<RaycastResult> raycastResults = new List<RaycastResult>();

		/// <summary>
		/// Finds the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="eventData">Event data.</param>
		protected IDropSupport<T> FindTarget(PointerEventData eventData)
		{
			raycastResults.Clear();

			EventSystem.current.RaycastAll(eventData, raycastResults);

			foreach (var raycastResult in raycastResults)
			{
				if (!raycastResult.isValid)
				{
					continue;
				}

				#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				var target = raycastResult.gameObject.GetComponent<IDropSupport<T>>();
				#else
				var target = raycastResult.gameObject.GetComponent(typeof(IDropSupport<T>)) as IDropSupport<T>;
				#endif
				if (target != null)
				{
					return target.CanReceiveDrop(Data, eventData) ? target : null;
				}
			}

			return null;
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			var target = FindTarget(eventData);
			if (target != null)
			{
				target.Drop(Data, eventData);
				Dropped(true);
			}
			else
			{
				Dropped(false);
			}

			IsDragged = false;
			Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Compatibility.GetCursorMode());
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if ((DragPoints != null) && (CanvasTransform != null) && DragPoints.ContainsKey(CanvasTransform.GetInstanceID()))
			{
				DragPoints.Remove(CanvasTransform.GetInstanceID());
			}
		}
	}
}