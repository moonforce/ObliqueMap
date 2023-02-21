namespace UIWidgets
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Object sliding direction.
	/// </summary>
	public enum ObjectSlidingDirection
	{
		/// <summary>
		/// Horizontal direction.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical direction.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Allow to drag objects between specified positions.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Object Sliding")]
	public class ObjectSliding : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		/// <summary>
		/// Allowed positions.
		/// </summary>
		public List<float> Positions = new List<float>();

		/// <summary>
		/// Slide direction.
		/// </summary>
		public ObjectSlidingDirection Direction = ObjectSlidingDirection.Horizontal;

		/// <summary>
		/// Movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		public AnimationCurve Movement = AnimationCurve.EaseInOut(0, 0, 1, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		protected RectTransform RectTransform
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

		ScrollRect parentScrollRect;

		/// <summary>
		/// Parent ScrollRect.
		/// </summary>
		protected ScrollRect ParentScrollRect
		{
			get
			{
				if (parentScrollRect == null)
				{
					parentScrollRect = GetComponentInParent<ScrollRect>();
				}

				return parentScrollRect;
			}
		}

		/// <summary>
		/// The current animation.
		/// </summary>
		protected IEnumerator CurrentAnimation;

		/// <summary>
		/// Is animation running?
		/// </summary>
		protected bool IsAnimationRunning;

		/// <summary>
		/// Is drag allowed?
		/// </summary>
		protected bool AllowDrag;

		/// <summary>
		/// Is direction is horizontal?
		/// </summary>
		/// <returns>true if direction is horizontal; otherwise, false.</returns>
		protected bool IsHorizontal()
		{
			return Direction == ObjectSlidingDirection.Horizontal;
		}

		/// <summary>
		/// Get end position.
		/// </summary>
		/// <returns>End position.</returns>
		protected float GetEndPosition()
		{
			var cur_position = IsHorizontal() ? RectTransform.anchoredPosition.x : RectTransform.anchoredPosition.y;

			var nearest_position = Positions[0];

			for (int i = 1; i < Positions.Count; i++)
			{
				if (Mathf.Abs(Positions[i] - cur_position) < Mathf.Abs(nearest_position - cur_position))
				{
					nearest_position = Positions[i];
				}
			}

			return nearest_position;
		}

		#region IBeginDragHandler implementation

		/// <summary>
		/// Handle begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (IsAnimationRunning)
			{
				IsAnimationRunning = false;
				if (CurrentAnimation != null)
				{
					StopCoroutine(CurrentAnimation);
				}
			}

			Vector2 cur_pos;
			Vector2 prev_pos;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, null, out cur_pos);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position - eventData.delta, null, out prev_pos);

			var delta = cur_pos - prev_pos;
			AllowDrag = (IsHorizontal() && (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)))
				|| (!IsHorizontal() && (Mathf.Abs(delta.y) > Mathf.Abs(delta.x)));

			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnBeginDrag(eventData);
				}

				return;
			}
		}
		#endregion

		#region IDragHandler implementation

		/// <summary>
		/// Handle drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnDrag(eventData);
				}

				return;
			}

			if (eventData.used)
			{
				return;
			}

			eventData.Use();

			Vector2 cur_pos;
			Vector2 prev_pos;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out cur_pos);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position - eventData.delta, eventData.pressEventCamera, out prev_pos);

			var pos = RectTransform.localPosition;
			var new_pos = new Vector3(
				IsHorizontal() ? pos.x + (cur_pos.x - prev_pos.x) : pos.x,
				!IsHorizontal() ? pos.y + (cur_pos.y - prev_pos.y) : pos.y,
				pos.z);

			RectTransform.localPosition = new_pos;
		}
		#endregion

		#region IEndDragHandler implementation

		/// <summary>
		/// Handle end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnEndDrag(eventData);
				}

				return;
			}

			if (Positions.Count == 0)
			{
				return;
			}

			IsAnimationRunning = true;
			var start_position = IsHorizontal() ? RectTransform.anchoredPosition.x : RectTransform.anchoredPosition.y;
			var end_position = GetEndPosition();
			CurrentAnimation = RunAnimation(IsHorizontal(), start_position, end_position, UnscaledTime);
			StartCoroutine(CurrentAnimation);
		}
		#endregion

		/// <summary>
		/// Runs the animation.
		/// </summary>
		/// <returns>The animation.</returns>
		/// <param name="isHorizontal">If set to <c>true</c> is horizontal.</param>
		/// <param name="startPosition">Start position.</param>
		/// <param name="endPosition">End position.</param>
		/// <param name="unscaledTime">If set to <c>true</c> use unscaled time.</param>
		protected virtual IEnumerator RunAnimation(bool isHorizontal, float startPosition, float endPosition, bool unscaledTime)
		{
			float delta;

			var animation_length = Movement.keys[Movement.keys.Length - 1].time;
			var start_time = Utilites.GetTime(unscaledTime);
			do
			{
				delta = Utilites.GetTime(unscaledTime) - start_time;
				var value = Movement.Evaluate(delta);

				var position = startPosition + ((endPosition - startPosition) * value);
				if (isHorizontal)
				{
					RectTransform.anchoredPosition = new Vector2(position, RectTransform.anchoredPosition.y);
				}
				else
				{
					RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, position);
				}

				yield return null;
			}
			while (delta < animation_length);

			if (isHorizontal)
			{
				RectTransform.anchoredPosition = new Vector2(endPosition, RectTransform.anchoredPosition.y);
			}
			else
			{
				RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, endPosition);
			}

			IsAnimationRunning = false;
		}
	}
}