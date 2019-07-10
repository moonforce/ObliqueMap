namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// PlaylistDialog helper.
	/// </summary>
	public class PlaylistDialogHelper : MonoBehaviour
	{
		/// <summary>
		/// Username input.
		/// </summary>
		[SerializeField]
		public InputField Name;

		/// <summary>
		/// Reset input.
		/// </summary>
		public void Refresh()
		{
			Name.text = string.Empty;
		}

		/// <summary>
		/// Validate input.
		/// </summary>
		/// <returns>true if input data is valid; otherwise, false.</returns>
		public bool Validate()
		{
			var valid_name = Name.text.Trim().Length > 0;

			if (!valid_name)
			{
				Name.Select();
			}

			return valid_name;
		}
	}
}