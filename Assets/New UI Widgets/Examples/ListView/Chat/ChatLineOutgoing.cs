namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ChatLineOutgoing component.
	/// </summary>
	public class ChatLineOutgoing : ComponentPool<ChatLineOutgoing>, IChatLineComponent
	{
		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <param name="parent">New parent.</param>
		/// <returns>ChatLineComponent instance.</returns>
		public IChatLineComponent IInstance(Transform parent)
		{
			return Instance(parent);
		}

		/// <summary>
		/// Chat.
		/// </summary>
		[SerializeField]
		public ChatView Chat;

		/// <summary>
		/// Invoke chat event.
		/// </summary>
		public void ChatEventInvoke()
		{
			Chat.MyEvent.Invoke();
			Debug.Log("Chat.MyEvent.Invoke()");
		}

		/// <summary>
		/// Username.
		/// </summary>
		[SerializeField]
		public Text UserName;

		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		public Text Message;

		/// <summary>
		/// Message time.
		/// </summary>
		[SerializeField]
		public Text Time;

		/// <summary>
		/// Image.
		/// </summary>
		[SerializeField]
		public Image Image;

		/// <summary>
		/// AudioPlayer.
		/// </summary>
		[SerializeField]
		public AudioPlayer Audio;

		/// <summary>
		/// Lightbox to display image.
		/// </summary>
		[SerializeField]
		public Lightbox Lightbox;

		/// <summary>
		/// Message data.
		/// </summary>
		protected ChatLine Item;

		/// <summary>
		/// Display ChatLine.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ChatLine item)
		{
			Item = item;

			UserName.text = item.UserName;
			Message.text = item.Message;
			Time.text = item.Time.ToString("[HH:mm:ss]");

			Message.gameObject.SetActive(item.Message != null);

			if (Image != null)
			{
				Image.gameObject.SetActive(item.Image != null);
				Image.sprite = item.Image;
			}

			if (Audio != null)
			{
				Audio.gameObject.SetActive(item.Audio != null);
				Audio.SetAudioClip(item.Audio);
			}
		}

		/// <summary>
		/// Show lightbox with image.
		/// </summary>
		public void ShowLightbox()
		{
			Lightbox.Show(Item.Image);
		}
	}
}