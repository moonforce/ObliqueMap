namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Spinner with string values.
	/// Display strings instead numeric value.
	/// </summary>
	public class SpinnerString : MonoBehaviour
	{
		/// <summary>
		/// Spinner.
		/// </summary>
		[SerializeField]
		protected Spinner Spinner;

		/// <summary>
		/// Text component to display selected option.
		/// </summary>
		[SerializeField]
		protected Text Text;

		/// <summary>
		/// Options list.
		/// </summary>
		[SerializeField]
		protected List<string> Options = new List<string>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			Spinner.Min = -1;
			Spinner.Max = Options.Count;
			Spinner.Step = 1;

			// add callback
			Spinner.onValueChangeInt.AddListener(Changed);

			// display initial option
			Changed(Spinner.Value);
		}

		/// <summary>
		/// Handle change event.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void Changed(int value)
		{
			if (value == -1)
			{
				Spinner.Value = Options.Count - 1;
			}
			else if (value == Options.Count)
			{
				Spinner.Value = 0;
			}
			else
			{
				// display option
				Text.text = Options[Spinner.Value];
			}
		}
	}
}