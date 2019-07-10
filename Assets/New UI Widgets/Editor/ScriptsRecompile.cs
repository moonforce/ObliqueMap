namespace UIWidgets
{
	using System.IO;
	using UnityEngine;

	/// <summary>
	/// Forced recompilation if compilation was not done after Scripting Define Symbols was changed.
	/// </summary>
	public static class ScriptsRecompile
	{
		/// <summary>
		/// Text label for initial state.
		/// </summary>
		public const string StatusInitial = "initial";

		/// <summary>
		/// Text label for state after symbols added.
		/// </summary>
		public const string StatusSymbolsAdded = "symbols added";

		/// <summary>
		/// Text label for recompilation started state.
		/// </summary>
		public const string StatusRecompiledAdded = "recompiled label added";

		/// <summary>
		/// Text label for recompilation labels removed state.
		/// </summary>
		public const string StatusRecompileRemoved = "recompiled label removed";

		const string folderSuffix = "Folder";
		const string fileSuffix = "RecompilationStatus";

		/// <summary>
		/// Check if forced recompilation required.
		/// </summary>
		[UnityEditor.Callbacks.DidReloadScripts]
		public static void Run()
		{
			#if UIWIDGETS_TMPRO_SUPPORT
			Check("TMPro", "UIWidgets.TMProSupport.ListViewIconsItemComponentTMPro");
			#endif

			#if UIWIDGETS_DATABIND_SUPPORT
			Check("DataBind", "UIWidgets.DataBind.CalendarDateObserver");
			#endif
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Conditional compilation.")]
		static void Check(string label, string typename)
		{
			var status = GetStatus(label);
			Log("check " + label + "; status: " + status);

			switch (status)
			{
				case StatusInitial:
					break;
				case StatusSymbolsAdded:
					var type = Utilites.GetType(typename);
					if (type == null)
					{
						Compatibility.ForceRecompileByLabel(label + folderSuffix);

						SetStatus(label, StatusRecompiledAdded);

						Log("Type " + typename + " not found, forced recompilation started.");
					}
					else
					{
						SetStatus(label, StatusInitial);

						Log("Type found");
					}

					break;
				case StatusRecompiledAdded:
					Compatibility.RemoveForceRecompileByLabel(label + folderSuffix);

					SetStatus(label, StatusRecompileRemoved);

					Log("Forced recompilation done; labels removing started");
					break;
				case StatusRecompileRemoved:
					SetStatus(label, StatusInitial);

					Log("Labels removed.");
					break;
				default:
					Debug.LogWarning("Unkown recompile status: " + status);
					break;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Conditional compilation.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "message", Justification = "For the debug purposes.")]
		static void Log(string message)
		{
		}

		/// <summary>
		/// Get forced recompilation status from file with label.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <returns>Status.</returns>
		public static string GetStatus(string label)
		{
			var path = Utilites.GetAssetPath(label + fileSuffix);
			if (path == null)
			{
				return StatusInitial;
			}

			return File.ReadAllText(path);
		}

		/// <summary>
		/// Set forced recompilation status to file with label.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="status">Status.</param>
		public static void SetStatus(string label, string status)
		{
			var path = Utilites.GetAssetPath(label + fileSuffix);
			if (path == null)
			{
				return;
			}

			File.WriteAllText(path, status);
		}
	}
}