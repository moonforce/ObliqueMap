namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Index of the currently playing item in the playlist.
	/// </summary>
	public class PlaylistCurrent : MonoBehaviour
	{
		int index = -1;

		/// <summary>
		/// Index.
		/// </summary>
		public int Index
		{
			get
			{
				return index;
			}

			set
			{
				if (index != value)
				{
					index = value;

					OnChange.Invoke(index);
				}
			}
		}

		/// <summary>
		/// OnChange event.
		/// </summary>
		public PlaylistCurrentEvent OnChange = new PlaylistCurrentEvent();
	}

	/// <summary>
	/// PlaylistCurrent event.
	/// </summary>
	public class PlaylistCurrentEvent : UnityEvent<int>
	{
	}
}