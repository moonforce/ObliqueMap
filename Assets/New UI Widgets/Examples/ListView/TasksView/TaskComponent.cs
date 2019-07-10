namespace UIWidgets.Examples.Tasks
{
	using UIWidgets;
	using UnityEngine.UI;

	/// <summary>
	/// Task component.
	/// </summary>
	public class TaskComponent : ListViewItem, IViewData<Task>
	{
		/// <summary>
		/// Name.
		/// </summary>
		public Text Name;

		/// <summary>
		/// Progressbar.
		/// </summary>
		public Progressbar Progressbar;

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Name, };
			}
		}

		Task currentItem;

		/// <summary>
		/// Current task.
		/// </summary>
		public Task Item
		{
			get
			{
				return currentItem;
			}

			set
			{
				if (currentItem != null)
				{
					currentItem.OnProgressChange -= UpdateProgressbar;
				}

				currentItem = value;
				if (currentItem != null)
				{
					Name.text = currentItem.Name;
					Progressbar.Value = currentItem.Progress;

					currentItem.OnProgressChange += UpdateProgressbar;
				}
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(Task item)
		{
			Item = item;
		}

		void UpdateProgressbar()
		{
			Progressbar.Animate(currentItem.Progress);
		}

		/// <summary>
		/// Reset current item.
		/// </summary>
		protected override void OnDestroy()
		{
			Item = null;
		}
	}
}