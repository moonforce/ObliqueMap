namespace UIWidgets
{
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// FileDialog.
	/// </summary>
	public class FileDialog : Picker<string, FileDialog>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public FileListView FileListView;

		/// <summary>
		/// Confirm Dialog.
		/// </summary>
		[SerializeField]
		public PickerBool ConfirmDialog;

		/// <summary>
		/// Filename Input.
		/// </summary>
		[SerializeField]
		public InputField FilenameInput;

		/// <summary>
		/// Proxy for InputField.
		/// Required to improve compatibility between different InputFields (like Unity.UI and TextMesh Pro versions).
		/// </summary>
		protected IInputFieldProxy FilenameInputProxy;

		/// <summary>
		/// OK button.
		/// </summary>
		[SerializeField]
		public Button OkButton;

		/// <summary>
		/// Is specified file should exists?
		/// </summary>
		[SerializeField]
		public bool FileShouldExists = false;

		/// <summary>
		/// Request confirmation if file exists?
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("RequestConfirmationIsFileExists")]
		public bool RequestConfirmationIfFileExists = true;

		/// <summary>
		/// Init InputProxy.
		/// </summary>
		protected virtual void InitFilenameInputProxy()
		{
			if (FilenameInputProxy == null)
			{
				FilenameInputProxy = new InputFieldProxy(FilenameInput);
			}
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(string defaultValue)
		{
			InitFilenameInputProxy();
			FileListView.OnSelectObject.AddListener(FileSelected);
			FilenameInputProxy.onValueChanged.AddListener(FilenameChanged);
			OkButton.onClick.AddListener(OkClick);

			if (!string.IsNullOrEmpty(defaultValue))
			{
				FileListView.CurrentDirectory = Path.GetDirectoryName(defaultValue);
				FileListView.Select(FileListView.DataSource.FindIndex(x => x.FullName == defaultValue));
			}

			FilenameChanged(FilenameInputProxy.text);
		}

		/// <summary>
		/// Callback when file selected.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void FileSelected(int index)
		{
			FilenameInputProxy.text = FileListView.SelectedItem.IsFile
				? Path.GetFileName(FileListView.SelectedItem.FullName)
				: string.Empty;
		}

		/// <summary>
		/// Callback when filename changed.
		/// </summary>
		/// <param name="filename">Filename.</param>
		protected virtual void FilenameChanged(string filename)
		{
			OkButton.interactable = IsValidFile(filename);
		}

		/// <summary>
		/// Is file valid?
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <returns>true if file valid; otherwise, false.</returns>
		protected virtual bool IsValidFile(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return false;
			}

			var fullname = Path.Combine(FileListView.CurrentDirectory, filename);
			if (FileShouldExists && !File.Exists(fullname))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Handle OK click event.
		/// </summary>
		public void OkClick()
		{
			var fullname = Path.Combine(FileListView.CurrentDirectory, FilenameInputProxy.text);
			if (RequestConfirmationIfFileExists && File.Exists(fullname))
			{
				var confirm = ConfirmDialog.Clone();
				confirm.Show(
					false,
					(result) =>
					{
						if (result)
						{
							Selected(fullname);
						}
					},
					DoNothing);
			}
			else
			{
				Selected(fullname);
			}
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		protected void DoNothing()
		{
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			FileListView.OnSelectObject.RemoveListener(FileSelected);
			FilenameInputProxy.onValueChanged.RemoveListener(FilenameChanged);
			OkButton.onClick.RemoveListener(OkClick);
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

			FileListView.SetStyle(style);

			style.InputField.ApplyTo(FilenameInput);

			style.Dialog.Button.ApplyTo(OkButton.gameObject);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			ConfirmDialog.SetStyle(style);

			return true;
		}
		#endregion
	}
}