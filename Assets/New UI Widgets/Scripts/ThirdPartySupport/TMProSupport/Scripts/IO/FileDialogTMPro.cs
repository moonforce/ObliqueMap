#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// FileDialog TMPro.
	/// </summary>
	public class FileDialogTMPro : FileDialog
	{
		/// <summary>
		/// Filename Input.
		/// </summary>
		[SerializeField]
		public TMP_InputField FilenameInputTMPro;

		/// <summary>
		/// Init InputProxy.
		/// </summary>
		protected override void InitFilenameInputProxy()
		{
			if (FilenameInputProxy == null)
			{
				FilenameInputProxy = new InputFieldTMProProxy(FilenameInputTMPro);
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

			if (FilenameInputTMPro != null)
			{
				style.InputField.ApplyTo(FilenameInputTMPro);
			}

			return true;
		}
#endregion
	}
}
#endif