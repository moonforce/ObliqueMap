namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileViewSample DataSource.
	/// </summary>
	[RequireComponent(typeof(TileViewSample))]
	public class TileViewSampleDataSource : MonoBehaviour
	{
		/// <summary>
		/// Start this instances.
		/// </summary>
		protected virtual void Start()
		{
			GenerateItems(40);
		}

		/// <summary>
		/// Generate DataSource with specified items count.
		/// </summary>
		/// <param name="count">Items count.</param>
		public void GenerateItems(int count)
		{
			var tiles = GetComponent<TileViewSample>();

			tiles.DataSource = Utilites.CreateList(count, x =>
			{
				return new TileViewItemSample()
				{
					Name = "Tile " + x,
					Capital = string.Empty,
					Area = Random.Range(10, 10 * 6),
					Population = Random.Range(100, 100 * 6),
				};
			});
		}
	}
}