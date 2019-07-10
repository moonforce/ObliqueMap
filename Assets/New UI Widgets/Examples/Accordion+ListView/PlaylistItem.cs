namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Playlist item.
	/// </summary>
	[Serializable]
	public class PlaylistItem
	{
		/// <summary>
		/// Track.
		/// </summary>
		public AudioClip Track;

		/// <summary>
		/// Track title.
		/// </summary>
		public string Title;

		/// <summary>
		/// Start position in samples.
		/// </summary>
		public int PlayStart;

		/// <summary>
		/// End position in samples.
		/// </summary>
		public int PlayStop = 1;

		/// <summary>
		/// Is block with range slider opened?
		/// </summary>
		public bool IsOpened = false;
	}
}