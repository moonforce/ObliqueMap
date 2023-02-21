namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Display current version number.
	/// </summary>
	public class SceneVersionNumber : MonoBehaviour
	{
		/// <summary>
		/// File with version number.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("versionFile")]
		public TextAsset VersionFile;

		/// <summary>
		/// Label to display version number.
		/// </summary>
		[SerializeField]
		public Text Label;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			DisplayVersion();
		}

		/// <summary>
		/// Display current version number.
		/// </summary>
		public void DisplayVersion()
		{
			if (Label != null)
			{
				var version = "v" + VersionFile.text;
				if (Label.text != version)
				{
					Label.text = version;
				}
			}
		}

		#if UNITY_EDITOR
		protected void OnValidate()
		{
			DisplayVersion();
		}
		#endif
	}
}