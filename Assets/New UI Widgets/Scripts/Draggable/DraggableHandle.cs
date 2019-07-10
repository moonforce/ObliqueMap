namespace UIWidgets
{
	using System.Collections;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Draggable handle.
	/// </summary>
	public class DraggableHandle : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler
	{
		/// <summary>
		/// The draggable.
		/// </summary>
		protected Draggable Draggable;

		/// <summary>
		/// The draggable rect.
		/// </summary>
		protected RectTransform DragRect;

		/// <summary>
		/// The canvas.
		/// </summary>
		protected Canvas Canvas;

		/// <summary>
		/// The canvas rect.
		/// </summary>
		protected RectTransform CanvasRect;

		/// <summary>
		/// The animation.
		/// </summary>
		protected IEnumerator Animation;

		/// <summary>
		/// Set the specified draggable object.
		/// </summary>
		/// <param name="newDragRect">New drag.</param>
		public void Drag(RectTransform newDragRect)
		{
			DragRect = newDragRect;
			Draggable = DragRect.GetComponent<Draggable>();
		}

		/// <summary>
		/// Raises the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			CanvasRect = Utilites.FindTopmostCanvas(transform) as RectTransform;
			Canvas = CanvasRect.GetComponent<Canvas>();
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.used)
			{
				return;
			}

			eventData.Use();
			if (Canvas == null)
			{
				throw new MissingComponentException(gameObject.name + " not in Canvas hierarchy.");
			}

			Vector2 cur_pos;
			Vector2 prev_pos;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(DragRect, eventData.position, eventData.pressEventCamera, out cur_pos);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(DragRect, eventData.position - eventData.delta, eventData.pressEventCamera, out prev_pos);

			var delta = cur_pos - prev_pos;
			if (!Draggable.Horizontal)
			{
				delta.x = 0;
			}

			if (!Draggable.Vertical)
			{
				delta.y = 0;
			}

			var new_pos = new Vector3(
				DragRect.localPosition.x + delta.x,
				DragRect.localPosition.y + delta.y,
				DragRect.localPosition.z);

			if (Draggable.Restriction == DraggableRestriction.Strict)
			{
				new_pos = RestrictPosition(new_pos);
			}

			DragRect.localPosition = new_pos;
		}

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (Animation != null)
			{
				StopCoroutine(Animation);
			}
		}

		/// <summary>
		/// Procerss the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (Draggable.Restriction == DraggableRestriction.AfterDrag)
			{
				Animation = AnimationCoroutine();
				StartCoroutine(Animation);
			}
		}

		/// <summary>
		/// Animation coroutine.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected IEnumerator AnimationCoroutine()
		{
			var start_pos = DragRect.localPosition;
			var end_pos = RestrictPosition(DragRect.localPosition);
			if (start_pos != end_pos)
			{
				var animation_length = Draggable.Curve.keys[Draggable.Curve.keys.Length - 1].time;
				var startTime = Utilites.GetTime(Draggable.UnscaledTime);
				var animation_position = 0f;

				do
				{
					animation_position = Utilites.GetTime(Draggable.UnscaledTime) - startTime;
					var value = Draggable.Curve.Evaluate(animation_position);
					DragRect.localPosition = Vector3.Lerp(start_pos, end_pos, value);

					yield return null;
				}
				while (animation_position < animation_length);

				DragRect.localPosition = end_pos;
			}
		}

		/// <summary>
		/// Get time.
		/// </summary>
		/// <returns>Time.</returns>
		[System.Obsolete("Use Utilites.GetTime(Draggable.UnscaledTime).")]
		protected float GetTime()
		{
			return Utilites.GetTime(Draggable.UnscaledTime);
		}

		/// <summary>
		/// Restrict the position.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
		protected virtual Vector3 RestrictPosition(Vector3 pos)
		{
			var parent = DragRect.parent as RectTransform;
			var parent_size = parent.rect.size;
			var drag_size = DragRect.rect.size;

			var min_x = (-parent_size[0] / 2f) + (drag_size[0] * DragRect.pivot.x);
			var max_x = (parent_size[0] / 2f) - (drag_size[0] * (1f - DragRect.pivot.x));

			var min_y = (-parent_size[1] / 2f) + (drag_size[1] * DragRect.pivot.y);
			var max_y = (parent_size[1] / 2f) - (drag_size[1] * (1f - DragRect.pivot.y));

			return new Vector3(
				Mathf.Clamp(pos.x, min_x, max_x),
				Mathf.Clamp(pos.y, min_y, max_y),
				pos.z);
		}
	}
}