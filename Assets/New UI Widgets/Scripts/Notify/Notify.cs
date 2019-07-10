namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Notify.
	/// Manage notifications.
	/// How to use:
	/// 1. Create container or containers with layout component. Notifications will be shown in those containers. You can check how it works with NotifyContainer in sample scene.
	/// 2. Create template for notification with Notify component.
	/// 3. If you want change text in runtime set Text property in Notify component.
	/// 4. If you want close notification by button set Hide button property in Notify component.
	/// 5. Write code to show notification
	/// <example>
	/// notifyPrefab.Template().Show("Sticky Notification. Click on the × above to close.");
	/// </example>
	/// notifyPrefab.Template() - return the notification instance by template name.
	/// Show("Sticky Notification. Click on the × above to close.") - show notification with following text;
	/// or
	/// Show(message: "Simple Notification.", customHideDelay = 4.5f, hideAnimation = UIWidgets.Notify.AnimationCollapse, slideUpOnHide = false);
	/// Show notification with following text, hide it after 4.5 seconds, run specified animation on hide without SlideUpOnHide.
	/// </summary>
	public class Notify : MonoBehaviour, ITemplatable, IStylable
	{
		[SerializeField]
		Button hideButton;

		/// <summary>
		/// Gets or sets the button that close current notification.
		/// </summary>
		/// <value>The hide button.</value>
		public Button HideButton
		{
			get
			{
				return hideButton;
			}

			set
			{
				if (hideButton != null)
				{
					hideButton.onClick.RemoveListener(Hide);
				}

				hideButton = value;

				if (hideButton != null)
				{
					hideButton.onClick.AddListener(Hide);
				}
			}
		}

		[SerializeField]
		Text text;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		public Text Text
		{
			get
			{
				return text;
			}

			set
			{
				text = value;
			}
		}

		[SerializeField]
		float HideDelay = 10f;

		[SerializeField]
		bool unscaledTime;

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

		static Templates<Notify> templates;

		/// <summary>
		/// Notify templates.
		/// </summary>
		public static Templates<Notify> Templates
		{
			get
			{
				if (templates == null)
				{
					templates = new Templates<Notify>(AddCloseCallback);
				}

				return templates;
			}

			set
			{
				templates = value;
			}
		}

		/// <summary>
		/// Function used to run show animation.
		/// </summary>
		public Func<Notify, IEnumerator> ShowAnimation;

		/// <summary>
		/// Function used to run hide animation.
		/// </summary>
		public Func<Notify, IEnumerator> HideAnimation;
		Func<Notify, IEnumerator> oldShowAnimation;
		Func<Notify, IEnumerator> oldHideAnimation;

		IEnumerator showCoroutine;
		IEnumerator hideCoroutine;

		/// <summary>
		/// Start slide up animations after hide current notification. Turn it off if its managed with HideAnimation.
		/// </summary>
		public bool SlideUpOnHide = true;

		NotifyInfoBase notifyInfo;

		/// <summary>
		/// Gets the dialog info.
		/// </summary>
		/// <value>The dialog info.</value>
		public NotifyInfoBase NotifyInfo
		{
			get
			{
				if (notifyInfo == null)
				{
					notifyInfo = GetComponent<NotifyInfoBase>();
				}

				return notifyInfo;
			}
		}

		/// <summary>
		/// Time between previous notification was hidden and next will be showed.
		/// </summary>
		public float SequenceDelay;

		/// <summary>
		/// The notify manager.
		/// </summary>
		static NotifySequenceManager notifyManager;

		/// <summary>
		/// Gets the notify manager.
		/// </summary>
		/// <value>The notify manager.</value>
		public static NotifySequenceManager NotifyManager
		{
			get
			{
				if (notifyManager == null)
				{
					var go = new GameObject("NotifySequenceManager");
					notifyManager = go.AddComponent<NotifySequenceManager>();
				}

				return notifyManager;
			}
		}

		/// <summary>
		/// Finds the templates.
		/// </summary>
		protected static void FindTemplates()
		{
			Templates.FindTemplates();
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected virtual void Awake()
		{
			if (IsTemplate)
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Remove listeners and template.
		/// </summary>
		protected virtual void OnDestroy()
		{
			HideButton = null;
			if (!IsTemplate)
			{
				templates = null;
				return;
			}

			// if FindTemplates never called than TemplateName == null
			if (TemplateName != null)
			{
				DeleteTemplate(TemplateName);
			}
		}

		/// <summary>
		/// Clears the cached instance of templates.
		/// </summary>
		public static void ClearCache()
		{
			Templates.ClearCache();
		}

		/// <summary>
		/// Clears the cached instance of specified template.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		public static void ClearCache(string templateName)
		{
			Templates.ClearCache(templateName);
		}

		/// <summary>
		/// Gets the template by name.
		/// </summary>
		/// <returns>The template.</returns>
		/// <param name="template">Template name.</param>
		public static Notify GetTemplate(string template)
		{
			return Templates.Get(template);
		}

		/// <summary>
		/// Deletes the template by name.
		/// </summary>
		/// <param name="template">Template.</param>
		public static void DeleteTemplate(string template)
		{
			Templates.Delete(template);
		}

		/// <summary>
		/// Adds the template.
		/// </summary>
		/// <param name="template">Template name.</param>
		/// <param name="notifyTemplate">Notify template object.</param>
		/// <param name="replace">If set to <c>true</c> replace.</param>
		public static void AddTemplate(string template, Notify notifyTemplate, bool replace = true)
		{
			Templates.Add(template, notifyTemplate, replace);
		}

		/// <summary>
		/// Return notify instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>Returns new notify instance.</returns>
		[Obsolete("Use Clone(templateName) instead.")]
		public static Notify Template(string templateName)
		{
			return Clone(templateName);
		}

		/// <summary>
		/// Return popup instance using current instance as template.
		/// </summary>
		/// <returns>Returns new notify instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public Notify Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return notification by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>Returns new notify instance.</returns>
		public static Notify Clone(string templateName)
		{
			return Templates.Instance(templateName);
		}

		/// <summary>
		/// Return Notify instance using current instance as template.
		/// </summary>
		/// <returns>Returns new notify instance.</returns>
		public Notify Clone()
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
		/// Adds the close callback.
		/// </summary>
		/// <param name="notify">Notify.</param>
		static void AddCloseCallback(Notify notify)
		{
			if (notify.hideButton == null)
			{
				return;
			}

			notify.hideButton.onClick.AddListener(notify.Hide);
		}

		/// <summary>
		/// Show the notification.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="customHideDelay">Custom hide delay.</param>
		/// <param name="container">Container. Parent object for current notification.</param>
		/// <param name="showAnimation">Function used to run show animation.</param>
		/// <param name="hideAnimation">Function used to run hide animation.</param>
		/// <param name="slideUpOnHide">Start slide up animations after hide current notification.</param>
		/// <param name="sequenceType">Add notification to sequence and display in order according specified sequenceType.</param>
		/// <param name="sequenceDelay">Time between previous notification was hidden and next will be showed.</param>
		/// <param name="clearSequence">Clear notifications sequence and hide current notification.</param>
		/// <param name="newUnscaledTime">Use unscaled time.</param>
		public void Show(
			string message = null,
			float? customHideDelay = null,
			Transform container = null,
			Func<Notify, IEnumerator> showAnimation = null,
			Func<Notify, IEnumerator> hideAnimation = null,
			bool? slideUpOnHide = null,
			NotifySequence sequenceType = NotifySequence.None,
			float sequenceDelay = 0.3f,
			bool clearSequence = false,
			bool? newUnscaledTime = null)
		{
			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: NotifyTemplate.Clone().Show(...), not NotifyTemplate.Show(...)");
			}

			if (clearSequence)
			{
				NotifyManager.Clear();
			}

			SequenceDelay = sequenceDelay;

			oldShowAnimation = ShowAnimation;
			oldHideAnimation = HideAnimation;

			SetMessage(message);

			if (container != null)
			{
				transform.SetParent(container, false);
			}

			if (newUnscaledTime != null)
			{
				unscaledTime = (bool)newUnscaledTime;
			}

			if (customHideDelay != null)
			{
				HideDelay = (float)customHideDelay;
			}

			if (slideUpOnHide != null)
			{
				SlideUpOnHide = (bool)slideUpOnHide;
			}

			if (showAnimation != null)
			{
				ShowAnimation = showAnimation;
			}

			if (hideAnimation != null)
			{
				HideAnimation = hideAnimation;
			}

			if (sequenceType != NotifySequence.None)
			{
				NotifyManager.Add(this, sequenceType);
			}
			else
			{
				Display();
			}
		}

		/// <summary>
		/// Set message text.
		/// </summary>
		/// <param name="message">Message text.</param>
		public virtual void SetMessage(string message)
		{
			if (NotifyInfo != null)
			{
				NotifyInfo.SetInfo(message);
			}
			else
			{
				if ((message != null) && (Text != null))
				{
					Text.text = message;
				}
			}
		}

		Action OnHideCallback;

		/// <summary>
		/// Display notification.
		/// </summary>
		/// <param name="onHideCallback">On hide callback.</param>
		public void Display(Action onHideCallback = null)
		{
			transform.SetAsLastSibling();
			gameObject.SetActive(true);

			OnHideCallback = onHideCallback;

			if (ShowAnimation != null)
			{
				showCoroutine = ShowAnimation(this);
				StartCoroutine(showCoroutine);
			}
			else
			{
				showCoroutine = null;
			}

			if (HideDelay > 0.0f)
			{
				hideCoroutine = HideCoroutine();
				StartCoroutine(hideCoroutine);
			}
			else
			{
				hideCoroutine = null;
			}
		}

		IEnumerator HideCoroutine()
		{
			yield return new WaitForSeconds(HideDelay);

			if (HideAnimation != null)
			{
				yield return StartCoroutine(HideAnimation(this));
			}

			Hide();
		}

		/// <summary>
		/// Hide notification.
		/// </summary>
		public void Hide()
		{
			if (SlideUpOnHide)
			{
				SlideUp();
			}

			if (OnHideCallback != null)
			{
				OnHideCallback();
			}

			Return();
		}

		/// <summary>
		/// Return this instance to cache.
		/// </summary>
		public void Return()
		{
			Templates.ToCache(this);

			ShowAnimation = oldShowAnimation;
			HideAnimation = oldHideAnimation;

			SetMessage(Templates.Get(TemplateName).text.text);
		}

		static Stack<RectTransform> slides = new Stack<RectTransform>();

		RectTransform GetSlide()
		{
			RectTransform rect;

			if (slides.Count == 0)
			{
				var obj = new GameObject("SlideUp");
				obj.SetActive(false);
				rect = obj.AddComponent<RectTransform>();
				obj.AddComponent<SlideUp>();

				// change height don't work without graphic component
				var image = obj.AddComponent<Image>();
				image.color = Color.clear;
			}
			else
			{
				do
				{
					rect = (slides.Count > 0) ? slides.Pop() : GetSlide();
				}
				while (rect == null);
			}

			return rect;
		}

		/// <summary>
		/// Slides up.
		/// </summary>
		void SlideUp()
		{
			var rect = GetSlide();
			SlideUp slide = rect.GetComponent<SlideUp>();
			slide.UnscaledTime = unscaledTime;

			var sourceRect = transform as RectTransform;

			rect.localRotation = sourceRect.localRotation;
			rect.localPosition = sourceRect.localPosition;
			rect.localScale = sourceRect.localScale;
			rect.anchorMin = sourceRect.anchorMin;
			rect.anchorMax = sourceRect.anchorMax;
			rect.anchoredPosition = sourceRect.anchoredPosition;
			rect.anchoredPosition3D = sourceRect.anchoredPosition3D;
			rect.sizeDelta = sourceRect.sizeDelta;
			rect.pivot = sourceRect.pivot;

			rect.transform.SetParent(transform.parent, false);
			rect.transform.SetSiblingIndex(transform.GetSiblingIndex());

			rect.gameObject.SetActive(true);
			slide.Run();
		}

		/// <summary>
		/// Returns slide to cache.
		/// </summary>
		/// <param name="slide">Slide.</param>
		public static void FreeSlide(RectTransform slide)
		{
			slides.Push(slide);
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <param name="notify">Notify.</param>
		/// <returns>Returns animations.</returns>
		public static IEnumerator AnimationRotate(Notify notify)
		{
			var rect = notify.transform as RectTransform;
			var start_rotarion = rect.localRotation.eulerAngles;
			var length = 0.5f;

			var end_time = Utilites.GetTime(notify.unscaledTime) + length;

			while (Utilites.GetTime(notify.unscaledTime) <= end_time)
			{
				var t = 1 - ((end_time - Utilites.GetTime(notify.unscaledTime)) / length);
				var rotation_x = Mathf.Lerp(0, 90, t);

				rect.localRotation = Quaternion.Euler(rotation_x, start_rotarion.y, start_rotarion.z);
				yield return null;
			}

			// return rotation back for future use
			rect.localRotation = Quaternion.Euler(start_rotarion);
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <param name="notify">Notify.</param>
		/// <returns>Returns animations.</returns>
		[Obsolete("AnimationRotate() now supports UnscaledTime.")]
		public static IEnumerator AnimationRotateUnscaledTime(Notify notify)
		{
			return AnimationRotate(notify);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <param name="notify">Notify.</param>
		/// <returns>Returns animations.</returns>
		public static IEnumerator AnimationCollapse(Notify notify)
		{
			var rect = notify.transform as RectTransform;
			var layout = notify.GetComponentInParent<EasyLayout>();
			var max_height = rect.rect.height;

			var speed = 200f;
			var time = max_height / speed;
			var end_time = Utilites.GetTime(notify.unscaledTime) + time;

			while (Utilites.GetTime(notify.unscaledTime) <= end_time)
			{
				var t = 1 - ((end_time - Utilites.GetTime(notify.unscaledTime)) / time);
				var height = Mathf.Lerp(max_height, 0, t);
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				if (layout != null)
				{
					layout.UpdateLayout();
				}

				yield return null;
			}

			// return height back for future use
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, max_height);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <param name="notify">Notify.</param>
		/// <returns>Returns animations.</returns>
		[Obsolete("AnimationCollapse now supports UnscaledTime.")]
		public static IEnumerator AnimationCollapseUnscaledTime(Notify notify)
		{
			return AnimationCollapse(notify);
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public bool SetStyle(Style style)
		{
			style.Notify.Background.ApplyTo(transform.Find("Background"));
			style.Notify.Text.ApplyTo(Text);

			if (NotifyInfo != null)
			{
				NotifyInfo.SetStyle(style);
			}

			if (HideButton != null)
			{
				style.ButtonClose.Background.ApplyTo(HideButton);
				style.ButtonClose.Text.ApplyTo(HideButton.transform.Find("Text"));
			}

			return true;
		}
		#endregion
	}
}