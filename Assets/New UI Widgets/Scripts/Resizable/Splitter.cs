namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Splitter type.
	/// </summary>
	public enum SplitterType
	{
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Splitter mode.
	/// </summary>
	public enum SplitterMode
	{
		/// <summary>
		/// Auto mode. Use previous and next siblings in hierarchy.
		/// </summary>
		Auto = 0,

		/// <summary>
		/// Manual mode. Use specified targets to resize.
		/// </summary>
		Manual = 1,
	}

	/// <summary>
	/// Splitter.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Splitter")]
	public class Splitter : MonoBehaviour,
		IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler
	{
		/// <summary>
		/// The type.
		/// </summary>
		public SplitterType Type = SplitterType.Vertical;

		/// <summary>
		/// Is need to update RectTransform on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateRectTransforms = true;

		/// <summary>
		/// Is need to update LayoutElement on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateLayoutElements = true;

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
		/// OnStartResize event.
		/// </summary>
		public SplitterResizeEvent OnStartResize = new SplitterResizeEvent();

		/// <summary>
		/// OnEndResize event.
		/// </summary>
		public SplitterResizeEvent OnEndResize = new SplitterResizeEvent();

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

		Canvas canvas;

		[SerializeField]
		SplitterMode mode = SplitterMode.Auto;

		[SerializeField]
		RectTransform leftTarget;

		[SerializeField]
		RectTransform rightTarget;

		LayoutElement leftTargetElement;

		LayoutElement LeftTargetElement
		{
			get
			{
				if ((leftTargetElement == null) || (leftTargetElement.gameObject != leftTarget.gameObject))
				{
					leftTargetElement = Utilites.GetOrAddComponent<LayoutElement>(leftTarget);
				}

				return leftTargetElement;
			}
		}

		LayoutElement rightTargetElement;

		LayoutElement RightTargetElement
		{
			get
			{
				if ((rightTargetElement == null) || (rightTargetElement.gameObject != rightTarget.gameObject))
				{
					rightTargetElement = Utilites.GetOrAddComponent<LayoutElement>(rightTarget);
				}

				return rightTargetElement;
			}
		}

		Vector2 summarySize;

		public bool processDrag;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Raises the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			canvas = Utilites.FindTopmostCanvas(transform).GetComponent<Canvas>();

			LayoutUtilites.UpdateLayout(transform.parent.GetComponent<LayoutGroup>());
			transform.parent.GetComponentsInChildren<Splitter>().ForEach(x => x.InitSizes());
		}

		void InitSizes()
		{
			var index = transform.GetSiblingIndex();

			if (index == 0 || transform.parent.childCount == (index + 1))
			{
				return;
			}

			if (mode == SplitterMode.Auto)
			{
				leftTarget = transform.parent.GetChild(index - 1) as RectTransform;
				rightTarget = transform.parent.GetChild(index + 1) as RectTransform;
			}

			LeftTargetElement.preferredWidth = leftTarget.rect.width;
			LeftTargetElement.preferredHeight = leftTarget.rect.height;

			RightTargetElement.preferredWidth = rightTarget.rect.width;
			RightTargetElement.preferredHeight = rightTarget.rect.height;
		}

		bool cursorChanged = false;

		/// <summary>
		/// Is cursor over?
		/// </summary>
		protected bool IsCursorOver;

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
		/// Update cursor.
		/// </summary>
		protected virtual void LateUpdate()
		{
			if (!IsCursorOver)
			{
				return;
			}

			if (processDrag)
			{
				return;
			}

			if (CursorTexture == null)
			{
				return;
			}

			if (!Input.mousePresent)
			{
				return;
			}

			Vector2 point;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, Input.mousePosition, CurrentCamera, out point))
			{
				return;
			}

			var rect = RectTransform.rect;
			if (rect.Contains(point))
			{
				cursorChanged = true;
				Cursor.SetCursor(CursorTexture, CursorHotSpot, Compatibility.GetCursorMode());
			}
			else if (cursorChanged)
			{
				cursorChanged = false;
				Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Compatibility.GetCursorMode());
			}
		}

		/// <summary>
		/// Raises the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			Vector2 point;
			processDrag = false;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out point))
			{
				return;
			}

			var index = transform.GetSiblingIndex();

			if (index == 0 || transform.parent.childCount == (index + 1))
			{
				return;
			}

			Cursor.SetCursor(CursorTexture, CursorHotSpot, Compatibility.GetCursorMode());
			cursorChanged = true;

			processDrag = true;

			if (mode == SplitterMode.Auto)
			{
				leftTarget = transform.parent.GetChild(index - 1) as RectTransform;
				rightTarget = transform.parent.GetChild(index + 1) as RectTransform;
			}

			LeftTargetElement.preferredWidth = leftTarget.rect.width;
			LeftTargetElement.preferredHeight = leftTarget.rect.height;

			RightTargetElement.preferredWidth = rightTarget.rect.width;
			RightTargetElement.preferredHeight = rightTarget.rect.height;

			summarySize = new Vector2(leftTarget.rect.width + rightTarget.rect.width, leftTarget.rect.height + rightTarget.rect.height);

			OnStartResize.Invoke(this);
		}

		/// <summary>
		/// Raises the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Compatibility.GetCursorMode());
			cursorChanged = false;

			processDrag = false;

			OnEndResize.Invoke(this);
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

			Vector2 p1;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, CurrentCamera, out p1);
			Vector2 p2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position - eventData.delta, CurrentCamera, out p2);
			var delta = p1 - p2;

			if (UpdateRectTransforms)
			{
				PerformUpdateRectTransforms(delta);
			}

			if (UpdateLayoutElements)
			{
				PerformUpdateLayoutElements(delta);
			}
		}

		bool IsHorizontal()
		{
			return Type == SplitterType.Horizontal;
		}

		void PerformUpdateRectTransforms(Vector2 delta)
		{
			if (!IsHorizontal())
			{
				float left_width;
				float right_width;

				if (delta.x > 0)
				{
					left_width = Mathf.Min(LeftTargetElement.preferredWidth + delta.x, summarySize.x - RightTargetElement.minWidth);
					right_width = summarySize.x - LeftTargetElement.preferredWidth;
				}
				else
				{
					right_width = Mathf.Min(RightTargetElement.preferredWidth - delta.x, summarySize.x - LeftTargetElement.minWidth);
					left_width = summarySize.x - RightTargetElement.preferredWidth;
				}

				leftTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, left_width);
				rightTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, right_width);
			}
			else
			{
				float left_height;
				float right_height;

				delta.y *= -1;
				if (delta.y > 0)
				{
					left_height = Mathf.Min(LeftTargetElement.preferredHeight + delta.y, summarySize.y - RightTargetElement.minHeight);
					right_height = summarySize.y - LeftTargetElement.preferredHeight;
				}
				else
				{
					right_height = Mathf.Min(RightTargetElement.preferredHeight - delta.y, summarySize.y - LeftTargetElement.minHeight);
					left_height = summarySize.y - RightTargetElement.preferredHeight;
				}

				leftTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, left_height);
				rightTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, right_height);
			}
		}

		void PerformUpdateLayoutElements(Vector2 delta)
		{
			if (!IsHorizontal())
			{
				if (delta.x > 0)
				{
					LeftTargetElement.preferredWidth = Mathf.Min(LeftTargetElement.preferredWidth + delta.x, summarySize.x - RightTargetElement.minWidth);
					RightTargetElement.preferredWidth = summarySize.x - LeftTargetElement.preferredWidth;
				}
				else
				{
					RightTargetElement.preferredWidth = Mathf.Min(RightTargetElement.preferredWidth - delta.x, summarySize.x - LeftTargetElement.minWidth);
					LeftTargetElement.preferredWidth = summarySize.x - RightTargetElement.preferredWidth;
				}
			}
			else
			{
				delta.y *= -1;
				if (delta.y > 0)
				{
					LeftTargetElement.preferredHeight = Mathf.Min(LeftTargetElement.preferredHeight + delta.y, summarySize.y - RightTargetElement.minHeight);
					RightTargetElement.preferredHeight = summarySize.y - LeftTargetElement.preferredHeight;
				}
				else
				{
					RightTargetElement.preferredHeight = Mathf.Min(RightTargetElement.preferredHeight - delta.y, summarySize.y - LeftTargetElement.minHeight);
					LeftTargetElement.preferredHeight = summarySize.y - RightTargetElement.preferredHeight;
				}
			}
		}
	}
}