namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Draggable restriction.
	/// </summary>
	public enum DraggableRestriction
	{
		/// <summary>
		/// Without restriction.
		/// </summary>
		None = 0,

		/// <summary>
		/// Does not allow drag outside parent.
		/// </summary>
		Strict = 1,

		/// <summary>
		/// Apply restriction after drag.
		/// </summary>
		AfterDrag = 2,
	}

	/// <summary>
	/// Draggable UI object..
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Draggable")]
	[RequireComponent(typeof(RectTransform))]
	public class Draggable : MonoBehaviour
	{
		/// <summary>
		/// The handle.
		/// </summary>
		[SerializeField]
		GameObject handle;

		DraggableHandle handleScript;

		/// <summary>
		/// Allow horizontal movement.
		/// </summary>
		[SerializeField]
		public bool Horizontal = true;

		/// <summary>
		/// Allow vertical movement.
		/// </summary>
		[SerializeField]
		public bool Vertical = true;

		/// <summary>
		/// Drag restriction.
		/// </summary>
		public DraggableRestriction Restriction = DraggableRestriction.None;

		/// <summary>
		/// Animation curve.
		/// </summary>
		[SerializeField]
		public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 0.2f, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		public bool UnscaledTime;

		/// <summary>
		/// If specified, restricts dragging from starting unless the pointerdown occurs on the specified element.
		/// </summary>
		/// <value>The handler.</value>
		public GameObject Handle
		{
			get
			{
				return handle;
			}

			set
			{
				SetHandle(value);
			}
		}

		/// <summary>
		/// Init handle.
		/// </summary>
		protected virtual void Start()
		{
			SetHandle(handle != null ? handle : gameObject);
		}

		/// <summary>
		/// Sets the handle.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetHandle(GameObject value)
		{
			if (handle != null)
			{
				Destroy(handleScript);
			}

			handle = value;
			handleScript = Utilites.GetOrAddComponent<DraggableHandle>(handle);
			handleScript.Drag(gameObject.transform as RectTransform);
		}
	}
}