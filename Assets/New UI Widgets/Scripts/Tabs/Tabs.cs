namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Tabs.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Tabs.png
	/// </summary>
	public class Tabs : MonoBehaviour, IStylable<StyleTabs>
	{
		/// <summary>
		/// The container for tab toggle buttons.
		/// </summary>
		[SerializeField]
		public Transform Container;

		/// <summary>
		/// The default tab button.
		/// </summary>
		[SerializeField]
		public Button DefaultTabButton;

		/// <summary>
		/// The active tab button.
		/// </summary>
		[SerializeField]
		public Button ActiveTabButton;

		[SerializeField]
		Tab[] tabObjects = new Tab[] { };

		/// <summary>
		/// Gets or sets the tab objects.
		/// </summary>
		/// <value>The tab objects.</value>
		public Tab[] TabObjects
		{
			get
			{
				return tabObjects;
			}

			set
			{
				tabObjects = value;
				UpdateButtons();
			}
		}

		/// <summary>
		/// The name of the default tab.
		/// </summary>
		[SerializeField]
		[Tooltip("Tab name which will be active by default, if not specified will be opened first Tab.")]
		public string DefaultTabName = string.Empty;

		/// <summary>
		/// If true does not deactivate hidden tabs.
		/// </summary>
		[SerializeField]
		[Tooltip("If true does not deactivate hidden tabs.")]
		public bool KeepTabsActive = false;

		/// <summary>
		/// OnTabSelect event.
		/// </summary>
		[SerializeField]
		public TabSelectEvent OnTabSelect = new TabSelectEvent();

		/// <summary>
		/// Gets or sets the selected tab.
		/// </summary>
		/// <value>The selected tab.</value>
		public Tab SelectedTab
		{
			get;
			protected set;
		}

		/// <summary>
		/// The default buttons.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<Button> DefaultButtons = new List<Button>();

		/// <summary>
		/// The active buttons.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<Button> ActiveButtons = new List<Button>();

		/// <summary>
		/// The callbacks.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<UnityAction> Callbacks = new List<UnityAction>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (Container == null)
			{
				throw new NullReferenceException("Container is null. Set object of type GameObject to Container.");
			}

			if (DefaultTabButton == null)
			{
				throw new NullReferenceException("DefaultTabButton is null. Set object of type GameObject to DefaultTabButton.");
			}

			if (ActiveTabButton == null)
			{
				throw new NullReferenceException("ActiveTabButton is null. Set object of type GameObject to ActiveTabButton.");
			}

			DefaultTabButton.gameObject.SetActive(false);
			ActiveTabButton.gameObject.SetActive(false);

			UpdateButtons();
		}

		/// <summary>
		/// Updates the buttons.
		/// </summary>
		protected virtual void UpdateButtons()
		{
			RemoveCallbacks();

			CreateButtons();

			AddCallbacks();

			if (tabObjects.Length == 0)
			{
				return;
			}

			if (!string.IsNullOrEmpty(DefaultTabName))
			{
				if (IsExistsTabName(DefaultTabName))
				{
					SelectTab(DefaultTabName);
				}
				else
				{
					Debug.LogWarning(string.Format("Tab with specified DefaultTabName \"{0}\" not found. Opened first Tab.", DefaultTabName), this);
					SelectTab(tabObjects[0].Name);
				}
			}
			else
			{
				SelectTab(tabObjects[0].Name);
			}
		}

		/// <summary>
		/// Is exists tab with specified name.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		/// <returns>true if exists tab with specified name; otherwise, false.</returns>
		protected virtual bool IsExistsTabName(string tabName)
		{
			for (int i = 0; i < tabObjects.Length; i++)
			{
				if (tabObjects[i].Name == tabName)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Add callback.
		/// </summary>
		/// <param name="tab">Tab.</param>
		/// <param name="index">Index.</param>
		protected virtual void AddCallback(Tab tab, int index)
		{
			var tabName = tab.Name;
			UnityAction callback = () => SelectTab(tabName);
			Callbacks.Add(callback);

			DefaultButtons[index].onClick.AddListener(Callbacks[index]);
		}

		/// <summary>
		/// Add callbacks.
		/// </summary>
		protected virtual void AddCallbacks()
		{
			tabObjects.ForEach(AddCallback);
		}

		/// <summary>
		/// Remove callback.
		/// </summary>
		/// <param name="tab">Tab.</param>
		/// <param name="index">Index.</param>
		protected virtual void RemoveCallback(Tab tab, int index)
		{
			if ((tab != null) && (index < Callbacks.Count))
			{
				DefaultButtons[index].onClick.RemoveListener(Callbacks[index]);
			}
		}

		/// <summary>
		/// Remove callbacks.
		/// </summary>
		protected virtual void RemoveCallbacks()
		{
			if (Callbacks.Count > 0)
			{
				tabObjects.ForEach(RemoveCallback);
				Callbacks.Clear();
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveCallbacks();
		}

		/// <summary>
		/// Selects the tab.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		public void SelectTab(string tabName)
		{
			var index = Array.FindIndex(tabObjects, x => x.Name == tabName);
			if (index == -1)
			{
				throw new ArgumentException(string.Format("Tab with name \"{0}\" not found.", tabName));
			}

			if (KeepTabsActive)
			{
				tabObjects[index].TabObject.transform.SetAsLastSibling();
			}
			else
			{
				tabObjects.ForEach(DeactivateTab);
				tabObjects[index].TabObject.SetActive(true);
			}

			DefaultButtons.ForEach(ActivateButton);
			DefaultButtons[index].gameObject.SetActive(false);

			ActiveButtons.ForEach(DeactivateButton);
			ActiveButtons[index].gameObject.SetActive(true);

			SelectedTab = tabObjects[index];
			OnTabSelect.Invoke(index);

			EventSystem.current.SetSelectedGameObject(ActiveButtons[index].gameObject);
		}

		/// <summary>
		/// Deactivate tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		protected virtual void DeactivateTab(Tab tab)
		{
			tab.TabObject.SetActive(false);
		}

		/// <summary>
		/// Activate button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected virtual void ActivateButton(Button button)
		{
			button.gameObject.SetActive(true);
		}

		/// <summary>
		/// Deactivate button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected virtual void DeactivateButton(Button button)
		{
			button.gameObject.SetActive(false);
		}

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		protected virtual void CreateButtons()
		{
			DefaultButtons.ForEach(x => x.interactable = true);
			ActiveButtons.ForEach(x => x.interactable = true);
			if (tabObjects.Length > DefaultButtons.Count)
			{
				for (var i = DefaultButtons.Count; i < tabObjects.Length; i++)
				{
					var defaultButton = Compatibility.Instantiate(DefaultTabButton);
					defaultButton.transform.SetParent(Container, false);
					DefaultButtons.Add(defaultButton);

					var activeButton = Compatibility.Instantiate(ActiveTabButton);
					activeButton.transform.SetParent(Container, false);
					ActiveButtons.Add(activeButton);
				}
			}

			// del existing ui elements if necessary
			if (tabObjects.Length < DefaultButtons.Count)
			{
				for (var i = DefaultButtons.Count - 1; i > tabObjects.Length - 1; i--)
				{
					Destroy(DefaultButtons[i].gameObject);
					Destroy(ActiveButtons[i].gameObject);

					DefaultButtons.RemoveAt(i);
					ActiveButtons.RemoveAt(i);
				}
			}

			DefaultButtons.ForEach(SetButtonName);
			ActiveButtons.ForEach(SetButtonName);
		}

		/// <summary>
		/// Sets the name of the button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="index">Index.</param>
		protected virtual void SetButtonName(Button button, int index)
		{
			var tab_button = button.GetComponent<TabButtonComponentBase>();
			if (tab_button == null)
			{
				button.gameObject.SetActive(true);
				button.GetComponentInChildren<Text>().text = TabObjects[index].Name;
			}
			else
			{
				tab_button.SetButtonData(TabObjects[index]);
			}
		}

		/// <summary>
		/// Disable the tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void DisableTab(Tab tab)
		{
			var i = Array.IndexOf(TabObjects, tab);
			if (i != -1)
			{
				DefaultButtons[i].interactable = false;
				ActiveButtons[i].interactable = false;
			}
		}

		/// <summary>
		/// Enable the tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void EnableTab(Tab tab)
		{
			var i = Array.IndexOf(TabObjects, tab);
			if (i != -1)
			{
				DefaultButtons[i].interactable = true;
				ActiveButtons[i].interactable = true;
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="styleTyped">Style for the tabs.</param>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(StyleTabs styleTyped, Style style)
		{
			if (DefaultTabButton != null)
			{
				styleTyped.DefaultButton.ApplyTo(DefaultTabButton.gameObject);
				DefaultButtons.ForEach(x => styleTyped.DefaultButton.ApplyTo(x.gameObject));
			}

			if (ActiveTabButton != null)
			{
				styleTyped.ActiveButton.ApplyTo(ActiveTabButton.gameObject);
				ActiveButtons.ForEach(x => styleTyped.ActiveButton.ApplyTo(x.gameObject));
			}

			foreach (var tab in tabObjects)
			{
				if (tab.TabObject != null)
				{
					styleTyped.ContentBackground.ApplyTo(tab.TabObject.GetComponent<Image>());
					style.ApplyForChildren(tab.TabObject);
				}
			}

			return true;
		}
		#endregion
	}
}