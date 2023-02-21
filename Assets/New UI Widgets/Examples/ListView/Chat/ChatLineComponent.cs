namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ChatLine component.
	/// </summary>
	public class ChatLineComponent : ListViewItem, IViewData<ChatLine>
	{
		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { };
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsBackground
		{
			get
			{
				return new Graphic[] { };
			}
		}

		/// <summary>
		/// Template for incoming message.
		/// </summary>
		[SerializeField]
		protected ChatLineIncoming IncomingTemplate;

		/// <summary>
		/// Template for outgoing message.
		/// </summary>
		[SerializeField]
		protected ChatLineOutgoing OutgoingTemplate;

		/// <summary>
		/// Current component.
		/// </summary>
		public IChatLineComponent CurrentComponent;

		/// <summary>
		/// Init templates.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			IncomingTemplate.gameObject.SetActive(false);
			OutgoingTemplate.gameObject.SetActive(false);
		}

		/// <summary>
		/// Gets the template.
		/// </summary>
		/// <returns>The template.</returns>
		/// <param name="type">Type.</param>
		protected virtual IChatLineComponent GetTemplate(ChatLineType type)
		{
			if (type == ChatLineType.Incoming)
			{
				return IncomingTemplate;
			}

			if (type == ChatLineType.Outgoing)
			{
				return OutgoingTemplate;
			}

			return null;
		}

		/// <summary>
		/// Current message type.
		/// </summary>
		protected int CurrentItemType = -1;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ChatLine item)
		{
			if (CurrentItemType != (int)item.Type)
			{
				MovedToCache();

				CurrentItemType = (int)item.Type;
				CurrentComponent = GetTemplate(item.Type).IInstance(transform);
			}

			CurrentComponent.SetData(item);
		}

		/// <summary>
		/// Free current component.
		/// </summary>
		public override void MovedToCache()
		{
			if (CurrentComponent != null)
			{
				CurrentComponent.Free(IncomingTemplate.transform.parent);
				CurrentComponent = null;
			}
		}
	}
}