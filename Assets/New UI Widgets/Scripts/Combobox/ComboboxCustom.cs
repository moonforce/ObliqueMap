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
	/// Base class for custom combobox.
	/// </summary>
	/// <typeparam name="TListViewCustom">Type of ListView.</typeparam>
	/// <typeparam name="TComponent">Type of ListView component.</typeparam>
	/// <typeparam name="TItem">Type of ListView item.</typeparam>
	public class ComboboxCustom<TListViewCustom, TComponent, TItem> : MonoBehaviour, ISubmitHandler, IStylable
			where TListViewCustom : ListViewCustom<TComponent, TItem>
			where TComponent : ListViewItem
	{
		/// <summary>
		/// Custom Combobox event.
		/// </summary>
		[Serializable]
		public class ComboboxCustomEvent : UnityEvent<int, TItem>
		{
		}

		[SerializeField]
		TListViewCustom listView;

		/// <summary>
		/// Gets or sets the ListView.
		/// </summary>
		/// <value>ListView component.</value>
		public TListViewCustom ListView
		{
			get
			{
				return listView;
			}

			set
			{
				SetListView(value);
			}
		}

		[SerializeField]
		Button toggleButton;

		/// <summary>
		/// Gets or sets the toggle button.
		/// </summary>
		/// <value>The toggle button.</value>
		public Button ToggleButton
		{
			get
			{
				return toggleButton;
			}

			set
			{
				SetToggleButton(value);
			}
		}

		[SerializeField]
		TComponent current;

		/// <summary>
		/// Gets or sets the current component.
		/// </summary>
		/// <value>The current.</value>
		public TComponent Current
		{
			get
			{
				return current;
			}

			set
			{
				SetCurrent(value);
			}
		}

		/// <summary>
		/// Is Current implements IViewData{TItem}.
		/// </summary>
		protected bool CanSetData;

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ComboboxCustomEvent OnSelect = new ComboboxCustomEvent();

		UnityAction<int> onSelectCallback;

		Transform listCanvas;

		Transform listParent;

		/// <summary>
		/// The components list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TComponent> components = new List<TComponent>();

		/// <summary>
		/// The components cache list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TComponent> componentsCache = new List<TComponent>();

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected virtual void Awake()
		{
			ListView.Sort = false;
		}

		[NonSerialized]
		bool isComboboxCustomInited;

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
			if (isComboboxCustomInited)
			{
				return;
			}

			ListView.Sort = false;

			isComboboxCustomInited = true;

			onSelectCallback = index => OnSelect.Invoke(index, listView.DataSource[index]);

			CanSetData = current is IViewData<TItem>;

			SetToggleButton(toggleButton);

			SetListView(listView);

			if (listView != null)
			{
				current.gameObject.SetActive(false);

				listView.OnSelectObject.RemoveListener(UpdateView);
				listView.OnDeselectObject.RemoveListener(UpdateView);

				listCanvas = Utilites.FindTopmostCanvas(listParent);

				listView.gameObject.SetActive(true);
				listView.Init();
				if ((listView.SelectedIndex == -1) && (listView.DataSource.Count > 0) && (!listView.MultipleSelect))
				{
					listView.SelectedIndex = 0;
				}

				if (listView.SelectedIndex != -1)
				{
					UpdateView();
				}

				listView.gameObject.SetActive(false);

				listView.OnSelectObject.AddListener(UpdateView);
				listView.OnDeselectObject.AddListener(UpdateView);
			}
		}

		/// <summary>
		/// Set new component to display current item.
		/// </summary>
		/// <param name="newCurrent">New component.</param>
		protected virtual void SetCurrent(TComponent newCurrent)
		{
			components.ForEach(DeactivateComponent);
			components.ForEach(x => Destroy(x));
			components.Clear();
			componentsCache.ForEach(x => Destroy(x));
			componentsCache.Clear();

			current = newCurrent;
			current.Owner = ListView;
			current.gameObject.SetActive(false);

			CanSetData = current is IViewData<TItem>;
		}

		/// <summary>
		/// Sets the toggle button.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetToggleButton(Button value)
		{
			if (toggleButton != null)
			{
				toggleButton.onClick.RemoveListener(ToggleList);
			}

			toggleButton = value;

			if (toggleButton != null)
			{
				toggleButton.onClick.AddListener(ToggleList);
			}
		}

		/// <summary>
		/// Sets the list view.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetListView(TListViewCustom value)
		{
			if (listView != null)
			{
				listParent = null;

				listView.OnSelectObject.RemoveListener(UpdateView);
				listView.OnDeselectObject.RemoveListener(UpdateView);
				listView.OnSelectObject.RemoveListener(onSelectCallback);

				listView.OnFocusOut.RemoveListener(OnFocusHideList);

				listView.onCancel.RemoveListener(OnListViewCancel);
				listView.onItemCancel.RemoveListener(OnListViewCancel);

				RemoveDeselectCallbacks();
			}

			listView = value;
			if (listView != null)
			{
				listParent = listView.transform.parent;

				listView.OnSelectObject.AddListener(UpdateView);
				listView.OnDeselectObject.AddListener(UpdateView);
				listView.OnSelectObject.AddListener(onSelectCallback);

				listView.OnFocusOut.AddListener(OnFocusHideList);

				listView.onCancel.AddListener(OnListViewCancel);
				listView.onItemCancel.AddListener(OnListViewCancel);

				AddDeselectCallbacks();
			}
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public virtual int Set(TItem item, bool allowDuplicate = true)
		{
			return listView.Set(item, allowDuplicate);
		}

		/// <summary>
		/// Clear listview and selected item.
		/// </summary>
		public virtual void Clear()
		{
			listView.Clear();
			UpdateView();
		}

		/// <summary>
		/// Toggles the list visibility.
		/// </summary>
		public virtual void ToggleList()
		{
			if (listView == null)
			{
				return;
			}

			if (listView.gameObject.activeSelf)
			{
				HideList();
			}
			else
			{
				ShowList();
			}
		}

		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Shows the list.
		/// </summary>
		public virtual void ShowList()
		{
			if (listView == null)
			{
				return;
			}

			ModalKey = ModalHelper.Open(this, null, new Color(0, 0, 0, 0f), HideList);

			if (listCanvas != null)
			{
				listParent = listView.transform.parent;
				listView.transform.SetParent(listCanvas, true);
			}

			listView.gameObject.SetActive(true);

			if (listView.Layout != null)
			{
				listView.Layout.UpdateLayout();
			}

			if (listView.SelectComponent())
			{
				SetChildDeselectListener(EventSystem.current.currentSelectedGameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(listView.gameObject);
			}
		}

		/// <summary>
		/// Hides the list.
		/// </summary>
		public virtual void HideList()
		{
			if (ModalKey != null)
			{
				ModalHelper.Close((int)ModalKey);
				ModalKey = null;
			}

			if (listView == null)
			{
				return;
			}

			if (listCanvas != null)
			{
				listView.transform.SetParent(listParent);
			}

			listView.gameObject.SetActive(false);
		}

		/// <summary>
		/// The children deselect.
		/// </summary>
		protected List<SelectListener> childrenDeselect = new List<SelectListener>();

		/// <summary>
		/// Hide list when focus lost.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnFocusHideList(BaseEventData eventData)
		{
			if (eventData.selectedObject == gameObject)
			{
				return;
			}

			var ev_item = eventData as ListViewItemEventData;
			if (ev_item != null)
			{
				if (ev_item.NewSelectedObject != null)
				{
					SetChildDeselectListener(ev_item.NewSelectedObject);
				}

				return;
			}

			var ev = eventData as PointerEventData;
			if (ev == null)
			{
				HideList();
				return;
			}

			var go = ev.pointerPressRaycast.gameObject;
			if (go == null)
			{
				HideList();
				return;
			}

			if (go.Equals(toggleButton.gameObject))
			{
				return;
			}

			if (go.transform.IsChildOf(listView.transform))
			{
				SetChildDeselectListener(go);
				return;
			}

			HideList();
		}

		/// <summary>
		/// Sets the child deselect listener.
		/// </summary>
		/// <param name="child">Child.</param>
		protected void SetChildDeselectListener(GameObject child)
		{
			var deselectListener = GetDeselectListener(child);
			if (!childrenDeselect.Contains(deselectListener))
			{
				deselectListener.onDeselect.AddListener(OnFocusHideList);
				childrenDeselect.Add(deselectListener);
			}
		}

		/// <summary>
		/// Gets the deselect listener.
		/// </summary>
		/// <returns>The deselect listener.</returns>
		/// <param name="go">Go.</param>
		protected SelectListener GetDeselectListener(GameObject go)
		{
			return Utilites.GetOrAddComponent<SelectListener>(go);
		}

		/// <summary>
		/// Adds the deselect callbacks.
		/// </summary>
		protected void AddDeselectCallbacks()
		{
			if (listView.ScrollRect == null)
			{
				return;
			}

			if (listView.ScrollRect.verticalScrollbar == null)
			{
				return;
			}

			var scrollbar = listView.ScrollRect.verticalScrollbar.gameObject;
			var deselectListener = GetDeselectListener(scrollbar);

			deselectListener.onDeselect.AddListener(OnFocusHideList);
			childrenDeselect.Add(deselectListener);
		}

		/// <summary>
		/// Removes the deselect callbacks.
		/// </summary>
		protected void RemoveDeselectCallbacks()
		{
			childrenDeselect.ForEach(RemoveDeselectCallback);
			childrenDeselect.Clear();
		}

		/// <summary>
		/// Removes the deselect callback.
		/// </summary>
		/// <param name="listener">Listener.</param>
		protected void RemoveDeselectCallback(SelectListener listener)
		{
			if (listener != null)
			{
				listener.onDeselect.RemoveListener(OnFocusHideList);
			}
		}

		void UpdateView(int index)
		{
			UpdateView();
		}

		/// <summary>
		/// The current indices.
		/// </summary>
		protected List<int> currentIndices;

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			currentIndices = ListView.SelectedIndices;

			UpdateComponentsCount();

			components.ForEach(SetData);
			HideList();

			if ((EventSystem.current != null) && (!EventSystem.current.alreadySelecting))
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="i">The index.</param>
		protected virtual void SetData(TComponent component, int i)
		{
			component.Index = currentIndices[i];
			SetData(component, ListView.DataSource[currentIndices[i]]);
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected virtual void SetData(TComponent component, TItem item)
		{
			if (CanSetData)
			{
				(component as IViewData<TItem>).SetData(item);
			}
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		void OnListViewCancel()
		{
			HideList();
		}

		/// <summary>
		/// Adds the component.
		/// </summary>
		protected virtual void AddComponent()
		{
			TComponent component;
			if (componentsCache.Count > 0)
			{
				component = componentsCache[componentsCache.Count - 1];
				componentsCache.RemoveAt(componentsCache.Count - 1);
			}
			else
			{
				component = Compatibility.Instantiate(current);
				component.transform.SetParent(current.transform.parent, false);

				Utilites.FixInstantiated(current, component);
			}

			component.Index = -2;
			component.Owner = ListView;
			component.transform.SetAsLastSibling();
			component.gameObject.SetActive(true);
			components.Add(component);
		}

		/// <summary>
		/// Deactivates the component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DeactivateComponent(TComponent component)
		{
			if (component != null)
			{
				component.MovedToCache();
				component.Index = -1;
				component.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Updates the components count.
		/// </summary>
		protected void UpdateComponentsCount()
		{
			components.RemoveAll(IsNullComponent);

			if (components.Count == currentIndices.Count)
			{
				return;
			}

			if (components.Count < currentIndices.Count)
			{
				componentsCache.RemoveAll(IsNullComponent);

				for (int i = components.Count; i < currentIndices.Count; i++)
				{
					AddComponent();
				}
			}
			else
			{
				for (int i = currentIndices.Count; i < components.Count; i++)
				{
					DeactivateComponent(components[i]);
					componentsCache.Add(components[i]);
				}

				components.RemoveRange(currentIndices.Count, components.Count - currentIndices.Count);
			}
		}

		/// <summary>
		/// Determines whether the specified component is null.
		/// </summary>
		/// <returns><c>true</c> if the specified component is null; otherwise, <c>false</c>.</returns>
		/// <param name="component">Component.</param>
		protected bool IsNullComponent(TComponent component)
		{
			return component == null;
		}

		/// <summary>
		/// Gets the index of the component.
		/// </summary>
		/// <returns>The component index.</returns>
		/// <param name="item">Item.</param>
		protected int GetComponentIndex(TComponent item)
		{
			return item.Index;
		}

		/// <summary>
		/// Raises the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			ShowList();
		}

		/// <summary>
		/// Updates the current component.
		/// </summary>
		[Obsolete("Use SetData() instead.")]
		protected virtual void UpdateCurrent()
		{
			HideList();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			ListView = null;
			ToggleButton = null;
		}

		#region IStylable implementation

		/// <summary>
		/// Set components style.
		/// </summary>
		/// <param name="style">Style.</param>
		/// <param name="component">Component.</param>
		protected virtual void SetComponentStyle(Style style, TComponent component)
		{
			if (component == null)
			{
				return;
			}

			if (listView.MultipleSelect)
			{
				component.SetStyle(style.Combobox.MultipleDefaultItemBackground, style.Combobox.MultipleDefaultItemText, style);
			}
			else
			{
				component.SetStyle(style.Combobox.SingleDefaultItemBackground, style.Combobox.SingleDefaultItemText, style);
			}

			var remove_button = component.transform.Find("Remove");
			if (remove_button != null)
			{
				style.Combobox.RemoveBackground.ApplyTo(remove_button);
				style.Combobox.RemoveText.ApplyTo(remove_button.Find("Text"));
			}
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			if (listView.MultipleSelect)
			{
				style.Combobox.MultipleInputBackground.ApplyTo(GetComponent<Image>());
			}
			else
			{
				style.Combobox.SingleInputBackground.ApplyTo(GetComponent<Image>());
			}

			if (toggleButton != null)
			{
				style.Combobox.ToggleButton.ApplyTo(toggleButton.GetComponent<Image>());
			}

			if (current != null)
			{
				SetComponentStyle(style, current);
			}

			components.ForEach(x => SetComponentStyle(style, x));
			componentsCache.ForEach(x => SetComponentStyle(style, x));

			if (listView)
			{
				listView.SetStyle(style);
			}

			return true;
		}
		#endregion
	}
}