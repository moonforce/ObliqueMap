namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// DialogInputHelper
	/// </summary>
	public class DialogInputHelper : MonoBehaviour
	{
		/// <summary>
		/// Username input.
		/// </summary>
		[SerializeField]
		public InputField Username;

		/// <summary>
		/// Password input.
		/// </summary>
		[SerializeField]
		public InputField Password;

		/// <summary>
		/// Reset input.
		/// </summary>
		public void Refresh()
		{
			Username.text = string.Empty;
			Password.text = string.Empty;
		}

		/// <summary>
		/// Validate input.
		/// </summary>
		/// <returns>true if input data is valid; otherwise, false.</returns>
		public bool Validate()
		{
			var valid_username = Username.text.Trim().Length > 0;
			var valid_password = Password.text.Length > 0;

			if (!valid_username)
			{
				Username.Select();
			}
			else if (!valid_password)
			{
				Password.Select();
			}

			return valid_username && valid_password;
		}
	}
}