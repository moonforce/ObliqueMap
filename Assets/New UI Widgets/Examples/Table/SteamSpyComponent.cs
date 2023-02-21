namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SteamSpy component.
	/// </summary>
	public class SteamSpyComponent : ListViewItem, IViewData<SteamSpyItem>
	{
		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground
		{
			get
			{
				return new Graphic[] { Name, ScoreRank, Owners, Players, PlayersIn2Week, TimeIn2Week, };
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsBackground
		{
			get
			{
				return new Graphic[]
				{
					Name.transform.parent.GetComponent<Graphic>(),
					ScoreRank.transform.parent.GetComponent<Graphic>(),
					Owners.transform.parent.GetComponent<Graphic>(),
					Players.transform.parent.GetComponent<Graphic>(),
					PlayersIn2Week.transform.parent.GetComponent<Graphic>(),
					TimeIn2Week.transform.parent.GetComponent<Graphic>(),
				};
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// ScoreRank.
		/// </summary>
		[SerializeField]
		public Text ScoreRank;

		/// <summary>
		/// Owners.
		/// </summary>
		[SerializeField]
		public Text Owners;

		/// <summary>
		/// Players.
		/// </summary>
		[SerializeField]
		public Text Players;

		/// <summary>
		/// PlayersIn2Week.
		/// </summary>
		[SerializeField]
		public Text PlayersIn2Week;

		/// <summary>
		/// TimeIn2Week.
		/// </summary>
		[SerializeField]
		public Text TimeIn2Week;

		/// <summary>
		/// TooltipText.
		/// </summary>
		[SerializeField]
		public Text TooltipText;

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				return new GameObject[]
				{
					Name.transform.parent.gameObject,
					ScoreRank.transform.parent.gameObject,
					Owners.transform.parent.gameObject,
					Players.transform.parent.gameObject,
					PlayersIn2Week.transform.parent.gameObject,
					TimeIn2Week.transform.parent.gameObject,
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SteamSpyItem item)
		{
			Name.text = item.Name;
			TooltipText.text = item.Name;
			ScoreRank.text = (item.ScoreRank == -1) ? string.Empty : item.ScoreRank.ToString();
			Owners.text = item.Owners.ToString("N0") + "\n±" + item.OwnersVariance.ToString("N0");
			Players.text = item.Players.ToString("N0") + "\n±" + item.PlayersVariance.ToString("N0");
			PlayersIn2Week.text = item.PlayersIn2Week.ToString("N0") + "\n±" + item.PlayersIn2WeekVariance.ToString("N0");
			TimeIn2Week.text = Minutes2String(item.AverageTimeIn2Weeks) + "\n(" + Minutes2String(item.MedianTimeIn2Weeks) + ")";
		}

		static string Minutes2String(int minutes)
		{
			return string.Format("{0:00}:{1:00}", minutes / 60, minutes % 60);
		}
	}
}