namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for FileListViewPathComponent.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public abstract class FileListViewPathComponentBase : ComponentPool<FileListViewPathComponentBase>, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Path to displayed directory.
		/// </summary>
		public string FullName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Parent FileListViewPath.
		/// </summary>
		[HideInInspector]
		public FileListViewPath Owner;

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="path">Path.</param>
		public abstract void SetPath(string path);

		/// <summary>
		/// OnPointerDown event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerUp event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerClick event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				Owner.Open(FullName);
			}
		}

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			styleBackground.ApplyTo(GetComponent<Image>());
		}
	}
}