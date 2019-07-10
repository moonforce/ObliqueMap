namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ChatView test script.
	/// </summary>
	public class ChatViewTest : MonoBehaviour
	{
		/// <summary>
		/// ChatView.
		/// </summary>
		[SerializeField]
		public ChatView Chat;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		public InputField Message;

		/// <summary>
		/// The name of the user.
		/// </summary>
		[SerializeField]
		public InputField UserName;

		/// <summary>
		/// The message type.
		/// </summary>
		[SerializeField]
		public Switch Type;

		/// <summary>
		/// Attach audio?
		/// </summary>
		[SerializeField]
		public Switch AttachAudio;

		/// <summary>
		/// Attach image?
		/// </summary>
		[SerializeField]
		public Switch AttachImage;

		/// <summary>
		/// Attached image.
		/// </summary>
		[SerializeField]
		public Sprite TestImage;

		/// <summary>
		/// Attached audio.
		/// </summary>
		[SerializeField]
		public AudioClip TestAudio;

		/// <summary>
		/// Sends the message.
		/// </summary>
		public void SendMessage()
		{
			if (string.IsNullOrEmpty(UserName.text.Trim()))
			{
				return;
			}

			if (string.IsNullOrEmpty(Message.text.Trim()) && !AttachImage.IsOn && !AttachAudio.IsOn)
			{
				return;
			}

			// add new message to chat
			var line = new ChatLine()
			{
				UserName = UserName.text,
				Message = Message.text,
				Time = DateTime.Now,
				Type = Type.IsOn ? ChatLineType.Outgoing : ChatLineType.Incoming,
			};

			if (AttachImage.IsOn)
			{
				line.Image = TestImage;
			}

			if (AttachAudio.IsOn)
			{
				line.Audio = TestAudio;
			}

			Chat.DataSource.Add(line);

			Message.text = string.Empty;

			// scroll to end
			Chat.ScrollRect.verticalNormalizedPosition = 0f;
		}

		/// <summary>
		/// Add messages to chat.
		/// </summary>
		public void Test()
		{
			var lines = new ObservableList<ChatLine>()
			{
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1",
					Time = DateTime.Now,
					Type = ChatLineType.Incoming,
					Image = TestImage,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = DateTime.Now,
					Type = ChatLineType.Incoming,
					Audio = TestAudio,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2\nline3",
					Time = DateTime.Now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = DateTime.Now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = DateTime.Now,
					Type = ChatLineType.Outgoing,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = DateTime.Now,
					Type = ChatLineType.Outgoing,
					Audio = TestAudio,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = DateTime.Now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = DateTime.Now,
					Type = ChatLineType.Outgoing,
					Image = TestImage,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2\nline3",
					Time = DateTime.Now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2\nline3\nline4",
					Time = DateTime.Now,
					Type = ChatLineType.Incoming,
					Image = TestImage,
					Audio = TestAudio,
				},
			};

			Chat.Init();
			Chat.DataSource = lines;
			Chat.ScrollRect.verticalNormalizedPosition = 0f;
		}
	}
}