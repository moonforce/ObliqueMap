namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect events.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/ScrollRect Events")]
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectEvents : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		/// <summary>
		/// Pull directions.
		/// </summary>
		public enum PullDirection
		{
			/// <summary>
			/// None.
			/// </summary>
			None = 0,

			/// <summary>
			/// Up.
			/// </summary>
			Up = 1,

			/// <summary>
			/// Down.
			/// </summary>
			Down = 2,

			/// <summary>
			/// Left.
			/// </summary>
			Left = 3,

			/// <summary>
			/// Right.
			/// </summary>
			Right = 4,
		}

		/// <summary>
		/// Pull event.
		/// </summary>
		[Serializable]
		public class PullEvent : UnityEvent<PullDirection>
		{
		}

		/// <summary>
		/// The required pull distance to raise events.
		/// </summary>
		[SerializeField]
		public float RequiredMovement = 50f;

		/// <summary>
		/// OnPull event.
		/// </summary>
		[SerializeField]
		public PullEvent OnPull = new PullEvent();

		/// <summary>
		/// Event raised when drag distance is equal or more than RequiredMovement.
		/// </summary>
		[SerializeField]
		public PullEvent OnPullAllowed = new PullEvent();

		/// <summary>
		/// Event raised when drag distance is less than RequiredMovement after OnPullValid was called.
		/// </summary>
		[SerializeField]
		public PullEvent OnPullCancel = new PullEvent();

		/// <summary>
		/// OnPullUp event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullUp = new UnityEvent();

		/// <summary>
		/// OnPullDown event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullDown = new UnityEvent();

		/// <summary>
		/// OnPullLeft event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullLeft = new UnityEvent();

		/// <summary>
		/// OnPullRight event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullRight = new UnityEvent();

		ScrollRect scrollRect;

		/// <summary>
		/// Gets the ScrollRect.
		/// </summary>
		/// <value>ScrollRect.</value>
		public ScrollRect ScrollRect
		{
			get
			{
				if (scrollRect == null)
				{
					scrollRect = GetComponent<ScrollRect>();
				}

				return scrollRect;
			}
		}

		PullDirection pullDirection = PullDirection.None;
		
		bool isPullValid = false;
		
		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			ResetDrag();
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (isPullValid)
			{
				OnPull.Invoke(pullDirection);

				switch (pullDirection)
				{
					case PullDirection.Up:
						OnPullUp.Invoke();
						break;
					case PullDirection.Down:
						OnPullDown.Invoke();
						break;
					case PullDirection.Left:
						OnPullLeft.Invoke();
						break;
					case PullDirection.Right:
						OnPullRight.Invoke();
						break;
					case PullDirection.None:
						break;
					default:
						Debug.LogWarning("Unsupported pull direction: " + pullDirection);
						break;
				}
			}
			
			ResetDrag();
		}

		/// <summary>
		/// Resets the drag values.
		/// </summary>
		protected virtual void ResetDrag()
		{
			pullDirection = PullDirection.None;
			isPullValid = false;
		}

		/// <summary>
		/// When draging is occuring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			var movement = 0f;

			var size = (ScrollRect.transform as RectTransform).rect.size;

			var max_x = Mathf.Max(0f, ScrollRect.content.rect.width - size.x);
			var max_y = Mathf.Max(0f, ScrollRect.content.rect.height - size.y);

			if (ScrollRect.content.anchoredPosition.y < 0f)
			{
				movement = -ScrollRect.content.anchoredPosition.y;
				pullDirection = PullDirection.Up;
			}
			else if (ScrollRect.content.anchoredPosition.y > max_y)
			{
				movement = ScrollRect.content.anchoredPosition.y - max_y;
				pullDirection = PullDirection.Down;
			}
			else if (ScrollRect.content.anchoredPosition.x < 0f)
			{
				movement = -ScrollRect.content.anchoredPosition.x;
				pullDirection = PullDirection.Left;
			}
			else if (ScrollRect.content.anchoredPosition.x > max_x)
			{
				movement = ScrollRect.content.anchoredPosition.x - max_x;
				pullDirection = PullDirection.Right;
			}

			if (movement >= RequiredMovement)
			{
				if (!isPullValid)
				{
					OnPullAllowed.Invoke(pullDirection);
					isPullValid = true;
				}
			}
			else
			{
				if (isPullValid)
				{
					OnPullCancel.Invoke(pullDirection);
					isPullValid = false;
					pullDirection = PullDirection.None;
				}
			}
		}
	}
}