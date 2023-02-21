namespace UIWidgets.Examples.ToDoList
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ToDoListView component.
	/// </summary>
	public class ToDoListViewComponent : ListViewItem, IViewData<ToDoListViewItem>
	{
		/// <summary>
		/// Toggle.
		/// </summary>
		[SerializeField]
		public Toggle Toggle;

		/// <summary>
		/// Task.
		/// </summary>
		[SerializeField]
		public Text Task;

		/// <summary>
		/// Item.
		/// </summary>
		[NonSerialized]
		public ToDoListViewItem Item;

		LayoutGroup layoutGroup;

		/// <summary>
		/// Current layout.
		/// </summary>
		public LayoutGroup LayoutGroup
		{
			get
			{
				if (layoutGroup == null)
				{
					layoutGroup = GetComponent<LayoutGroup>();
				}

				return layoutGroup;
			}
		}

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Task, };
			}
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Toggle.onValueChanged.AddListener(OnToggle);
		}

		void OnToggle(bool toggle)
		{
			Item.Done = toggle;
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ToDoListViewItem item)
		{
			Item = item;

			if (Item == null)
			{
				Toggle.isOn = false;
				Task.text = string.Empty;
			}
			else
			{
				Toggle.isOn = Item.Done;
				Task.text = Item.Task.Replace("\\n", "\n");
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (Toggle != null)
			{
				Toggle.onValueChanged.RemoveListener(OnToggle);
			}
		}
	}
}