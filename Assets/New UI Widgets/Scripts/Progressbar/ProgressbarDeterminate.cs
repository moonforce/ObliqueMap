namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ProgressbarDeterminate.
	/// http://ilih.ru/images/unity-assets/UIWidgets/ProgressbarDeterminate.png
	/// </summary>
	public class ProgressbarDeterminate : ProgressbarDeterminateBase
	{
		/// <summary>
		/// The empty bar text.
		/// </summary>
		[SerializeField]
		public Text EmptyBarText;

		/// <summary>
		/// The full bar text.
		/// </summary>
		[SerializeField]
		public Text FullBarText;

		/// <summary>
		/// Updates the text.
		/// </summary>
		protected override void UpdateText()
		{
			var text = TextFunc(this);

			if (FullBarText != null)
			{
				FullBarText.text = text;
			}

			if (EmptyBarText != null)
			{
				EmptyBarText.text = text;
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			style.ProgressbarDeterminate.FullBarText.ApplyTo(FullBarText);
			style.ProgressbarDeterminate.EmptyBarText.ApplyTo(EmptyBarText);

			return true;
		}
		#endregion
	}
}