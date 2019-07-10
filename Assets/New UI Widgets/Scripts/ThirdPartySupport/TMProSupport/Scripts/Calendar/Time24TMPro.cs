#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Time widget for 24 hour format.
	/// </summary>
	public class Time24TMPro : TimeBase
	{
		/// <summary>
		/// The input field for the hours.
		/// </summary>
		[SerializeField]
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		protected TMP_InputField InputHours;
#else
		protected InputField InputHours;
#endif

		/// <summary>
		/// The input field for the minutes.
		/// </summary>
		[SerializeField]
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		protected TMP_InputField InputMinutes;
#else
		protected InputField InputMinutes;
#endif

		/// <summary>
		/// The input field for the seconds.
		/// </summary>
		[SerializeField]
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		protected TMP_InputField InputSeconds;
#else
		protected InputField InputSeconds;
#endif

		/// <summary>
		/// Init the input.
		/// </summary>
		protected override void InitInput()
		{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			if (InputHours != null)
			{
				InputProxyHours = new InputFieldTMProProxy(InputHours);
			}

			if (InputMinutes != null)
			{
				InputProxyMinutes = new InputFieldTMProProxy(InputMinutes);
			}

			if (InputSeconds != null)
			{
				InputProxySeconds = new InputFieldTMProProxy(InputSeconds);
			}
#else
			if (InputHours != null)
			{
				InputProxyHours = new InputFieldProxy(InputHours);
			}

			if (InputMinutes != null)
			{
				InputProxyMinutes = new InputFieldProxy(InputMinutes);
			}

			if (InputSeconds != null)
			{
				InputProxySeconds = new InputFieldProxy(InputSeconds);
			}
#endif
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

			if (InputHours != null)
			{
				style.Time.InputBackground.ApplyTo(InputHours);
				style.Time.InputText.ApplyTo(InputHours.textComponent.gameObject);
			}

			if (InputMinutes)
			{
				style.Time.InputBackground.ApplyTo(InputMinutes);
				style.Time.InputText.ApplyTo(InputMinutes.textComponent.gameObject);
			}

			if (InputSeconds)
			{
				style.Time.InputBackground.ApplyTo(InputSeconds);
				style.Time.InputText.ApplyTo(InputSeconds.textComponent.gameObject);
			}

			return true;
		}
#endregion
	}
}
#endif