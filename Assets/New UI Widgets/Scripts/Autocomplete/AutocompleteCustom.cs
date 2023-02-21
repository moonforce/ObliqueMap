namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// AutocompleteFilter.
	/// </summary>
	public enum AutocompleteFilter
	{
		/// <summary>
		/// Value should beginnig with Input.
		/// </summary>
		Startswith = 0,

		/// <summary>
		/// Input occurs with value.
		/// </summary>
		Contains = 1,
	}

	/// <summary>
	/// AutocompleteInput.
	/// </summary>
	public enum AutocompleteInput
	{
		/// <summary>
		/// Use current word in input.
		/// </summary>
		Word = 0,

		/// <summary>
		/// Use entire input.
		/// </summary>
		AllInput = 1,
	}

	/// <summary>
	/// AutocompleteResult.
	/// Append - add to end of input.
	/// Result - replace input.
	/// </summary>
	public enum AutocompleteResult
	{
		/// <summary>
		/// Append value to input
		/// </summary>
		Append = 0,

		/// <summary>
		/// Replace input.
		/// </summary>
		Replace = 1,
	}

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	/// <typeparam name="TListViewComponent">Type of ListView.DefaultItem.</typeparam>
	/// <typeparam name="TListView">Type of ListView.</typeparam>
	public abstract class AutocompleteCustom<TValue, TListViewComponent, TListView> : MonoBehaviour, IStylable
		where TListView : ListViewCustom<TListViewComponent, TValue>
		where TListViewComponent : ListViewItem
	{
		/// <summary>
		/// Select event.
		/// </summary>
		[Serializable]
		public class SelectEvent : UnityEvent<TValue>
		{
		}

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("InputField")]
		protected InputField inputField;

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		public InputField InputField
		{
			get
			{
				return inputField;
			}

			set
			{
				inputField = value;
				inputFieldProxy = null;
				InitInputField();
			}
		}

		/// <summary>
		/// Proxy for InputField.
		/// Required to improve compatibility between different InputFields (like Unity.UI and TextMesh Pro versions).
		/// </summary>
		protected IInputFieldProxy inputFieldProxy;

		/// <summary>
		/// Gets the InputFieldProxy.
		/// </summary>
		protected virtual IInputFieldProxy InputFieldProxy
		{
			get
			{
				if ((inputFieldProxy == null) && (inputField != null))
				{
					inputFieldProxy = new InputFieldProxy(inputField);
				}

				return inputFieldProxy;
			}
		}

		/// <summary>
		/// ListView to display available values.
		/// </summary>
		[SerializeField]
		public TListView TargetListView;

		/// <summary>
		/// Selected value will be added to this ListView.
		/// </summary>
		[SerializeField]
		public TListView DisplayListView;

		/// <summary>
		/// The allow duplicate of TargetListView items.
		/// </summary>
		[SerializeField]
		public bool AllowDuplicate = false;

		/// <summary>
		/// List of values.
		/// </summary>
		[SerializeField]
		public List<TValue> DataSource;

		/// <summary>
		/// The filter.
		/// </summary>
		[SerializeField]
		protected AutocompleteFilter filter = AutocompleteFilter.Contains;

		/// <summary>
		/// Gets or sets the filter.
		/// </summary>
		/// <value>The filter.</value>
		public AutocompleteFilter Filter
		{
			get
			{
				return filter;
			}

			set
			{
				filter = value;
				CustomFilter = null;
			}
		}

		/// <summary>
		/// Is filter case sensitive?
		/// </summary>
		[SerializeField]
		public bool CaseSensitive;

		/// <summary>
		/// The delimiter chars to find word for autocomplete if InputType == Word.
		/// </summary>
		[SerializeField]
		public char[] DelimiterChars = new char[] { ' ', '\n' };

		/// <summary>
		/// Custom filter.
		/// </summary>
		public Func<string, ObservableList<TValue>> CustomFilter;

		/// <summary>
		/// Use entire input or current word in input.
		/// </summary>
		[SerializeField]
		protected AutocompleteInput InputType = AutocompleteInput.AllInput;

		/// <summary>
		/// Append value to input or replace input.
		/// </summary>
		[SerializeField]
		protected AutocompleteResult Result = AutocompleteResult.Replace;

		/// <summary>
		/// OnOptionSelected event.
		/// </summary>
		public UnityEvent OnOptionSelected = new UnityEvent();

		/// <summary>
		/// OnOptionSelectedItem event.
		/// </summary>
		public SelectEvent OnOptionSelectedItem = new SelectEvent();

		/// <summary>
		/// Current query - word in input or whole input for autocomplete.
		/// </summary>
		[HideInInspector]
		protected string Query = string.Empty;

		/// <summary>
		/// Current word in input or whole input for autocomplete.
		/// </summary>
		[HideInInspector]
		[Obsolete("Use Query instead.")]
		protected string Input
		{
			get
			{
				return Query;
			}

			set
			{
				Query = value;
			}
		}

		/// <summary>
		/// The previous query string.
		/// </summary>
		protected string PrevQuery;

		/// <summary>
		/// The previous query string.
		/// </summary>
		protected string PrevInput
		{
			get
			{
				return PrevQuery;
			}

			set
			{
				PrevQuery = value;
			}
		}

		/// <summary>
		/// InputField.caretPosition. Used to keep caretPosition with Up and Down actions.
		/// </summary>
		protected int CaretPosition;

		/// <summary>
		/// The minimum number of characters a user must type before a search is performed.
		/// </summary>
		[SerializeField]
		public int MinLength = 0;

		/// <summary>
		/// The delay in seconds between when a keystroke occurs and when a search is performed.
		/// </summary>
		[SerializeField]
		public float SearchDelay = 0f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Coroutine to performs search.
		/// </summary>
		protected IEnumerator SearchCoroutine = null;

		/// <summary>
		/// Determines whether the beginnig of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginnig of value matches the Input; otherwise, false.</returns>
		public abstract bool Startswith(TValue value);

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public abstract bool Contains(TValue value);

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected abstract string GetStringValue(TValue value);

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			InitInputField();

			DisplayListView.gameObject.SetActive(false);

			DisplayListView.MultipleSelect = false;
			DisplayListView.OnSelect.AddListener(ItemSelected);
		}

		/// <summary>
		/// Init input field proxy.
		/// </summary>
		protected virtual void InitInputField()
		{
			if (!isInited)
			{
				return;
			}

			InputFieldProxy.onValueChanged.AddListener(ApplyFilter);

			var inputListener = Utilites.GetOrAddComponent<InputFieldListener>(InputFieldProxy.gameObject);
			inputListener.OnMoveEvent.AddListener(SelectResult);
			inputListener.OnSubmitEvent.AddListener(SubmitResult);
			inputListener.onDeselect.AddListener(InputDeselected);
		}

		/// <summary>
		/// Gets the input field text.
		/// </summary>
		/// <returns>The input field text.</returns>
		public virtual string GetInputFieldText()
		{
			return InputFieldProxy.text;
		}

		/// <summary>
		/// Allow to handle item selection event.
		/// </summary>
		protected bool AllowItemSelectionEvent;

		/// <summary>
		/// Handle input deselected event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void InputDeselected(BaseEventData eventData)
		{
			var ev = eventData as PointerEventData;
			if ((ev != null) && (ev.pointerCurrentRaycast.gameObject != null) && ev.pointerCurrentRaycast.gameObject.transform.IsChildOf(DisplayListView.transform))
			{
				AllowItemSelectionEvent = true;
			}
			else
			{
				AllowItemSelectionEvent = false;
				HideOptions();
			}
		}

		/// <summary>
		/// Handle item selected event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="component">Component.</param>
		protected virtual void ItemSelected(int index, ListViewItem component)
		{
			if (AllowItemSelectionEvent)
			{
				AllowItemSelectionEvent = false;
				SubmitResult(null);
			}
		}

		/// <summary>
		/// Canvas will be used as parent for DisplayListView.
		/// </summary>
		protected Transform CanvasTransform;

		/// <summary>
		/// To keep DisplayListView position if InputField inside scrollable area.
		/// </summary>
		protected Vector2 DisplayListViewAnchoredPosition;

		/// <summary>
		/// Default parent for DisplayListView.
		/// </summary>
		protected Transform DisplayListViewParent;

		/// <summary>
		/// Closes the options.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void HideOptions(string input)
		{
			HideOptions();
		}

		/// <summary>
		/// Closes the options.
		/// </summary>
		protected virtual void HideOptions()
		{
			if (CanvasTransform != null)
			{
				Utilites.GetOrAddComponent<HierarchyToggle>(DisplayListView).Restore();
			}

			DisplayListView.gameObject.SetActive(false);
		}

		/// <summary>
		/// Shows the options.
		/// </summary>
		protected virtual void ShowOptions()
		{
			CanvasTransform = Utilites.FindTopmostCanvas(DisplayListView.transform);
			if (CanvasTransform != null)
			{
				Utilites.GetOrAddComponent<HierarchyToggle>(DisplayListView).SetParent(CanvasTransform);
			}

			DisplayListView.gameObject.SetActive(true);
		}

		/// <summary>
		/// Gets the results.
		/// </summary>
		/// <returns>Values matches filter.</returns>
		protected virtual ObservableList<TValue> GetResults()
		{
			if (CustomFilter != null)
			{
				return CustomFilter(Query);
			}
			else
			{
				if (Filter == AutocompleteFilter.Startswith)
				{
					return Utilites.FindAll(DataSource, Startswith);
				}
				else
				{
					return Utilites.FindAll(DataSource, Contains);
				}
			}
		}

		/// <summary>
		/// Sets the input.
		/// </summary>
		/// <param name="input">Input string.</param>
		/// <returns>Query string.</returns>
		protected virtual string Input2Query(string input)
		{
			if (InputType == AutocompleteInput.AllInput)
			{
				return input;
			}

			int end_position = InputFieldProxy.caretPosition;

			if (input.Length >= end_position)
			{
				var text = input.Substring(0, end_position);
				var start_position = text.LastIndexOfAny(DelimiterChars) + 1;

				return text.Substring(start_position).Trim();
			}

			return input;
		}

		/// <summary>
		/// Applies the filter.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void ApplyFilter(string input)
		{
			if (SearchCoroutine != null)
			{
				StopCoroutine(SearchCoroutine);
			}

			if (EventSystem.current.currentSelectedGameObject != InputFieldProxy.gameObject)
			{
				return;
			}

			Query = Input2Query(input);

			if (Query == PrevQuery)
			{
				return;
			}

			PrevQuery = Query;

			if (Query.Length < MinLength)
			{
				DisplayListView.DataSource.Clear();
				HideOptions();
				return;
			}

			DisplayListView.Init();
			DisplayListView.MultipleSelect = false;

			SearchCoroutine = Search();
			StartCoroutine(SearchCoroutine);
		}

		/// <summary>
		/// Perfoms search with delay.
		/// </summary>
		/// <returns>Yield instruction.</returns>
		protected virtual IEnumerator Search()
		{
			if (SearchDelay > 0)
			{
				if (UnscaledTime)
				{
					yield return StartCoroutine(Utilites.WaitForSecondsUnscaled(SearchDelay));
				}
				else
				{
					yield return new WaitForSeconds(SearchDelay);
				}
			}

			DisplayListView.DataSource = GetResults();

			if (DisplayListView.DataSource.Count > 0)
			{
				ShowOptions();
				DisplayListView.SelectedIndex = -1;
			}
			else
			{
				HideOptions();
			}
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		protected virtual void Update()
		{
			if (!AllowItemSelectionEvent)
			{
				CaretPosition = InputFieldProxy.caretPosition;
			}
		}

		/// <summary>
		/// Selects the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SelectResult(AxisEventData eventData)
		{
			if (!DisplayListView.gameObject.activeInHierarchy)
			{
				return;
			}

			if (DisplayListView.DataSource.Count == 0)
			{
				return;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Up:
					if (DisplayListView.SelectedIndex == 0)
					{
						DisplayListView.SelectedIndex = DisplayListView.DataSource.Count - 1;
					}
					else
					{
						DisplayListView.SelectedIndex -= 1;
					}

					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldProxy.caretPosition = CaretPosition;
					break;
				case MoveDirection.Down:
					if (DisplayListView.SelectedIndex == (DisplayListView.DataSource.Count - 1))
					{
						DisplayListView.SelectedIndex = 0;
					}
					else
					{
						DisplayListView.SelectedIndex += 1;
					}

					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldProxy.caretPosition = CaretPosition;
					break;
				default:
					if (Input2Query(InputFieldProxy.text) != Query)
					{
						ApplyFilter(InputFieldProxy.text);
					}

					break;
			}
		}

		/// <summary>
		/// Submits the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SubmitResult(BaseEventData eventData)
		{
			SubmitResult(eventData, false);
		}

		/// <summary>
		/// Submits the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="isEnter">Is Enter pressed?</param>
		protected virtual void SubmitResult(BaseEventData eventData, bool isEnter)
		{
			if (DisplayListView.SelectedIndex == -1)
			{
				return;
			}

			if (InputFieldProxy.IsMultiLineNewline())
			{
				if (!DisplayListView.gameObject.activeInHierarchy)
				{
					return;
				}
				else
				{
					isEnter = false;
				}
			}

			var item = DisplayListView.DataSource[DisplayListView.SelectedIndex];

			if (TargetListView != null)
			{
				TargetListView.Init();
				TargetListView.Set(item, AllowDuplicate);
			}

			var value = GetStringValue(item);
			if (Result == AutocompleteResult.Append)
			{
				int end_position = (DisplayListView.gameObject.activeInHierarchy && eventData != null && !isEnter) ? InputFieldProxy.caretPosition : CaretPosition;
				var text = InputFieldProxy.text.Substring(0, end_position);
				var start_position = text.LastIndexOfAny(DelimiterChars) + 1;

				InputFieldProxy.text = InputFieldProxy.text.Substring(0, start_position) + value + InputFieldProxy.text.Substring(end_position);

				InputFieldProxy.caretPosition = start_position + value.Length;
				#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				// InputField.selectionFocusPosition = start_position + value.Length;
				#else
				InputFieldProxy.MoveToEnd();
				#endif
				if (isEnter)
				{
					FixCaretPosition = start_position + value.Length;
					InputFieldProxy.ActivateInputField();
				}
			}
			else
			{
				InputFieldProxy.text = value;
				#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				InputFieldProxy.caretPosition = value.Length;
				#else
				InputFieldProxy.ActivateInputField();
				#endif
				FixCaretPosition = value.Length;
			}

			OnOptionSelected.Invoke();
			OnOptionSelectedItem.Invoke(item);

			HideOptions();
		}

		/// <summary>
		/// Caret position after Enter pressed.
		/// </summary>
		protected int FixCaretPosition = -1;

		/// <summary>
		/// LateUpdate.
		/// </summary>
		protected virtual void LateUpdate()
		{
			if (FixCaretPosition != -1)
			{
				#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				InputFieldProxy.caretPosition = FixCaretPosition;
				#else
				InputFieldProxy.MoveToEnd();
				#endif
				FixCaretPosition = -1;
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (DisplayListView != null)
			{
				DisplayListView.OnSelect.RemoveListener(ItemSelected);
			}

			if (!InputFieldProxy.IsNull())
			{
				InputFieldProxy.onValueChanged.RemoveListener(ApplyFilter);

				var inputListener = InputFieldProxy.gameObject.GetComponent<InputFieldListener>();
				if (inputListener != null)
				{
					inputListener.OnMoveEvent.RemoveListener(SelectResult);
					inputListener.OnSubmitEvent.RemoveListener(SubmitResult);

					inputListener.onDeselect.RemoveListener(InputDeselected);
				}
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set InputField style.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void SetStyleInput(Style style)
		{
			if (InputField == null)
			{
				return;
			}

			style.Autocomplete.InputField.ApplyTo(InputField.textComponent.gameObject, true);
			if (InputField.placeholder != null)
			{
				style.Autocomplete.Placeholder.ApplyTo(InputField.placeholder.gameObject);
			}
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			style.Autocomplete.Background.ApplyTo(GetComponent<Image>());

			SetStyleInput(style);

			if (DisplayListView != null)
			{
				DisplayListView.SetStyle(style);
			}

			if (TargetListView != null)
			{
				TargetListView.SetStyle(style);
			}

			return true;
		}
		#endregion
	}
}