namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Filter sample.
	/// </summary>
	[RequireComponent(typeof(Combobox))]
	public class SampleFilter : MonoBehaviour
	{
		/// <summary>
		/// Container.
		/// </summary>
		[SerializeField]
		public GameObject Container;

		Combobox combobox;

		/// <summary>
		/// Adds listener.
		/// </summary>
		protected virtual void Start()
		{
			combobox = GetComponent<Combobox>();
			if (combobox != null)
			{
				combobox.OnSelect.AddListener(Filter);
			}
		}

		/// <summary>
		/// Enable or disable gameobjects if it's match specified item.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <param name="item">Item.</param>
		protected virtual void Filter(int index, string item)
		{
			foreach (Transform child in Container.transform)
			{
				var child_active = (item == "All") ? true : child.gameObject.name.StartsWith(item);
				child.gameObject.SetActive(child_active);
			}
		}

		/// <summary>
		/// Remove listner.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (combobox != null)
			{
				combobox.OnSelect.RemoveListener(Filter);
			}
		}
	}
}