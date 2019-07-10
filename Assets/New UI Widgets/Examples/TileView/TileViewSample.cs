namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileViewSample.
	/// </summary>
	public class TileViewSample : TileViewCustom<TileViewComponentSample, TileViewItemSample>
	{
		bool isTileViewSampleInited = false;

		// Comparison<TileViewItemSample> itemsComparison = (x, y) => x.Name.CompareTo(y.Name);

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			// OnSelect.AddListener(ItemSelected);
			// OnSelectObject.AddListener(ItemSelected);
		}

		void ItemSelected(int index, ListViewItem component)
		{
			if (component != null)
			{
				// (component as TileViewComponentSample).DoSomething();
			}

			Debug.Log(index);
			Debug.Log(DataSource[index].Name);
		}

		void ItemSelected(int index)
		{
			Debug.Log(index);
			Debug.Log(DataSource[index].Name);
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isTileViewSampleInited)
			{
				return;
			}

			isTileViewSampleInited = true;

			base.Init();

			// DataSource.Comparison = itemsComparison;
		}
	}
}