namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// NotifyInfoBase.
	/// </summary>
	public abstract class NotifyInfoBase : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="message">Message.</param>
		public abstract void SetInfo(string message);

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public abstract bool SetStyle(Style style);
		#endregion
	}
}