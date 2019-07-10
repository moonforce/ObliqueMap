namespace UIWidgets.Examples
{
	using System.Collections;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test ScrollRectEvents.
	/// </summary>
	public class TestScrollRectEventsPull : MonoBehaviour
	{
		/// <summary>
		/// ListView to add items.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// ScrollRectEvents.
		/// </summary>
		[SerializeField]
		public ScrollRectEvents PullEvents;

		/// <summary>
		/// Text field to display info.
		/// </summary>
		[SerializeField]
		public Text Info;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			ListView.Sort = false;

			PullEvents.OnPull.AddListener(Pull);
			PullEvents.OnPullAllowed.AddListener(PullAllowed);
			PullEvents.OnPullCancel.AddListener(PullCancel);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected void OnDestroy()
		{
			if (PullEvents != null)
			{
				PullEvents.OnPull.RemoveListener(Pull);
				PullEvents.OnPullAllowed.RemoveListener(PullAllowed);
				PullEvents.OnPullCancel.RemoveListener(PullCancel);
			}
		}

		void Pull(ScrollRectEvents.PullDirection direction)
		{
			var items = ListView.DataSource;

			switch (direction)
			{
				case ScrollRectEvents.PullDirection.Up:
					items.Insert(0, new ListViewIconsItemDescription() { Name = "New item. Total: " + (items.Count + 1) });

					Info.text = "New item added.";
					break;
				case ScrollRectEvents.PullDirection.Down:
					StartCoroutine(AddItems());

					break;
			}
		}

		IEnumerator AddItems()
		{
			Info.text = "New item will added after 3 seconds.";

			yield return new WaitForSeconds(3f);

			var items = ListView.DataSource;
			items.Add(new ListViewIconsItemDescription() { Name = "New item. Total: " + (items.Count + 1) });

			ListView.ScrollToAnimated(items.Count - 1);

			Info.text = "New item added.";
		}

		void PullAllowed(ScrollRectEvents.PullDirection direction)
		{
			Info.text = "Pull event will be raised on drag release.";
		}

		void PullCancel(ScrollRectEvents.PullDirection direction)
		{
			Info.text = "Pull event canceled.";
		}
	}
}