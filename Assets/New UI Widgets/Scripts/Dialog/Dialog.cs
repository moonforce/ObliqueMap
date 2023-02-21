namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// Dialog.
	/// </summary>
	public class Dialog : MonoBehaviour, ITemplatable, IStylable
	{
		[SerializeField]
		Button defaultButton;

		/// <summary>
		/// Gets or sets the default button.
		/// </summary>
		/// <value>The default button.</value>
		public Button DefaultButton
		{
			get
			{
				return defaultButton;
			}

			set
			{
				defaultButton = value;
			}
		}

		[SerializeField]
		Text titleText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		public Text TitleText
		{
			get
			{
				return titleText;
			}

			set
			{
				titleText = value;
			}
		}

		[SerializeField]
		Text contentText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		public Text ContentText
		{
			get
			{
				return contentText;
			}

			set
			{
				contentText = value;
			}
		}

		[SerializeField]
		Image dialogIcon;

		/// <summary>
		/// Gets or sets the icon component.
		/// </summary>
		/// <value>The icon.</value>
		public Image Icon
		{
			get
			{
				return dialogIcon;
			}

			set
			{
				dialogIcon = value;
			}
		}

		DialogInfoBase dialogInfo;

		/// <summary>
		/// Gets the dialog info.
		/// </summary>
		/// <value>The dialog info.</value>
		public DialogInfoBase DialogInfo
		{
			get
			{
				if (dialogInfo == null)
				{
					dialogInfo = GetComponent<DialogInfoBase>();
				}

				return dialogInfo;
			}
		}

		bool isTemplate = true;

		/// <summary>
		/// Gets a value indicating whether this instance is template.
		/// </summary>
		/// <value><c>true</c> if this instance is template; otherwise, <c>false</c>.</value>
		public bool IsTemplate
		{
			get
			{
				return isTemplate;
			}

			set
			{
				isTemplate = value;
			}
		}

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		public string TemplateName
		{
			get;
			set;
		}

		static Templates<Dialog> templates;

		/// <summary>
		/// Dialog templates.
		/// </summary>
		public static Templates<Dialog> Templates
		{
			get
			{
				if (templates == null)
				{
					templates = new Templates<Dialog>();
				}

				return templates;
			}

			set
			{
				templates = value;
			}
		}

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		protected virtual void Awake()
		{
			if (IsTemplate)
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			DeactivateButtons();

			if (!IsTemplate)
			{
				templates = null;
				return;
			}

			// if FindTemplates never called than TemplateName == null
			if (TemplateName != null)
			{
				Templates.Delete(TemplateName);
			}
		}

		/// <summary>
		/// Return dialog instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New Dialog instance.</returns>
		[Obsolete("Use Clone(templateName) instead.")]
		public static Dialog Template(string templateName)
		{
			return Clone(templateName);
		}

		/// <summary>
		/// Return dialog instance using current instance as template.
		/// </summary>
		/// <returns>New Dialog instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public Dialog Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return dialog instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New Dialog instance.</returns>
		public static Dialog Clone(string templateName)
		{
			return Templates.Instance(templateName);
		}

		/// <summary>
		/// Return dialog instance using current instance as template.
		/// </summary>
		/// <returns>New Dialog instance.</returns>
		public Dialog Clone()
		{
			if ((TemplateName != null) && Templates.Exists(TemplateName))
			{
				// do nothing
			}
			else if (!Templates.Exists(gameObject.name))
			{
				Templates.Add(gameObject.name, this);
			}
			else if (Templates.Get(gameObject.name) != this)
			{
				Templates.Add(gameObject.name, this);
			}

			var id = gameObject.GetInstanceID().ToString();
			if (!Templates.Exists(id))
			{
				Templates.Add(id, this);
			}
			else if (Templates.Get(id) != this)
			{
				Templates.Add(id, this);
			}

			return Templates.Instance(id);
		}

		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Show dialog.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="buttons">Buttons.</param>
		/// <param name="focusButton">Set focus on button with specified name.</param>
		/// <param name="position">Position.</param>
		/// <param name="icon">Icon.</param>
		/// <param name="modal">If set to <c>true</c> modal.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="canvas">Canvas.</param>
		public virtual void Show(
			string title = null,
			string message = null,
			DialogActions buttons = null,
			string focusButton = null,
			Vector3? position = null,
			Sprite icon = null,
			bool modal = false,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null)
		{
			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: DialogTemplate.Clone().Show(...), not DialogTemplate.Show(...)");
			}

			if (position == null)
			{
				position = new Vector3(0, 0, 0);
			}

			SetInfo(title, message, icon);

			var parent = (canvas != null) ? canvas.transform : Utilites.FindTopmostCanvas(gameObject.transform);
			if (parent != null)
			{
				transform.SetParent(parent, false);
			}

			if (modal)
			{
				ModalKey = ModalHelper.Open(this, modalSprite, modalColor);
			}
			else
			{
				ModalKey = null;
			}

			transform.SetAsLastSibling();

			transform.localPosition = (Vector3)position;
			gameObject.SetActive(true);

			CreateButtons(buttons, focusButton);
		}

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title = null, string message = null, Sprite icon = null)
		{
			if (DialogInfo != null)
			{
				DialogInfo.SetInfo(title, message, icon);
			}
			else
			{
				if ((title != null) && (TitleText != null))
				{
					TitleText.text = title;
				}

				if ((message != null) && (ContentText != null))
				{
					ContentText.text = message;
				}

				if ((icon != null) && (Icon != null))
				{
					Icon.sprite = icon;
				}
			}
		}

		/// <summary>
		/// Close dialog.
		/// </summary>
		public virtual void Hide()
		{
			if (ModalKey != null)
			{
				ModalHelper.Close((int)ModalKey);
			}

			Return();
		}

		/// <summary>
		/// The buttons cache.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<Button> buttonsCache = new List<Button>();

		/// <summary>
		/// The buttons in use.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<Button> buttonsInUse = new List<Button>();

		/// <summary>
		/// The buttons actions.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<UnityAction> buttonsActions = new List<UnityAction>();

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		/// <param name="buttons">Buttons.</param>
		/// <param name="focusButton">Focus button.</param>
		protected virtual void CreateButtons(DialogActions buttons, string focusButton)
		{
			defaultButton.gameObject.SetActive(false);

			if (buttons == null)
			{
				return;
			}

			buttons.ForEach(x =>
			{
				var button = GetButton();

				UnityAction callback = () =>
				{
					if (x.Value())
					{
						Hide();
					}
				};

				buttonsInUse.Add(button);
				buttonsActions.Add(callback);

				button.gameObject.SetActive(true);
				button.transform.SetAsLastSibling();

				var dialog_button = button.GetComponentInChildren<DialogButtonComponentBase>();
				if (dialog_button != null)
				{
					dialog_button.SetButtonName(x.Key);
				}
				else
				{
					var text = button.GetComponentInChildren<Text>();
					if (text != null)
					{
						text.text = x.Key;
					}
				}

				button.onClick.AddListener(callback);

				if (x.Key == focusButton)
				{
					button.Select();
				}
			});
		}

		/// <summary>
		/// Gets the button.
		/// </summary>
		/// <returns>The button.</returns>
		protected virtual Button GetButton()
		{
			if (buttonsCache.Count > 0)
			{
				return buttonsCache.Pop();
			}

			var button = Compatibility.Instantiate(DefaultButton);

			button.transform.SetParent(DefaultButton.transform.parent, false);

			return button;
		}

		/// <summary>
		/// Return this instance to cache.
		/// </summary>
		protected virtual void Return()
		{
			Templates.ToCache(this);

			DeactivateButtons();
			ResetParametres();
		}

		/// <summary>
		/// Deactivates the buttons.
		/// </summary>
		protected virtual void DeactivateButtons()
		{
			for (int index = 0; index < buttonsInUse.Count; index++)
			{
				DeactivateButton(index, buttonsInUse[index]);
			}

			buttonsInUse.Clear();
			buttonsActions.Clear();
		}

		/// <summary>
		/// Deactivates the button.
		/// </summary>
		/// <param name="index">Index of the button.</param>
		/// <param name="button">Button.</param>
		protected virtual void DeactivateButton(int index, Button button)
		{
			button.gameObject.SetActive(false);
			button.onClick.RemoveListener(buttonsActions[index]);
			buttonsCache.Add(button);
		}

		/// <summary>
		/// Resets the parametres.
		/// </summary>
		protected virtual void ResetParametres()
		{
			var template = Templates.Get(TemplateName);

			var title = template.TitleText != null ? template.TitleText.text : string.Empty;
			var content = template.ContentText != null ? template.ContentText.text : string.Empty;
			var icon = template.Icon != null ? template.Icon.sprite : null;

			SetInfo(title, content, icon);
		}

		/// <summary>
		/// Default function to close dialog.
		/// </summary>
		/// <returns>true if dialog can be closed; otherwise false.</returns>
		public static bool Close()
		{
			return true;
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public bool SetStyle(Style style)
		{
			style.Dialog.Background.ApplyTo(GetComponent<Image>());
			style.Dialog.Title.ApplyTo(TitleText);

			style.Dialog.ContentBackground.ApplyTo(transform.Find("Content"));
			style.Dialog.ContentText.ApplyTo(ContentText);

			style.Dialog.Delimiter.ApplyTo(transform.Find("Delimiter/Delimiter"));
			style.Dialog.Button.ApplyTo(DefaultButton.gameObject);

			style.ButtonClose.Background.ApplyTo(transform.Find("Header/CloseButton"));

			buttonsInUse.ForEach(x => style.Dialog.Button.ApplyTo(x.gameObject));
			buttonsCache.ForEach(x => style.Dialog.Button.ApplyTo(x.gameObject));

			return true;
		}
		#endregion
	}
}