namespace UIWidgets
{
	using System.IO;
	using UnityEditor;

	/// <summary>
	/// Menu options to toggle third party packages support.
	/// </summary>
	public static class ThirdPartySupportMenuOptions
	{
#if UNITY_5_6_OR_NEWER
		#region DataBindSupport
		const string DataBindSupport = "UIWIDGETS_DATABIND_SUPPORT";

		/// <summary>
		/// Is Data Bind for Unity installed?
		/// </summary>
		public static bool DataBindInstalled
		{
			get
			{
				return Utilites.GetType("Slash.Unity.DataBind.Core.Presentation.DataProvider") != null;
			}
		}

		/// <summary>
		/// Enable DataBind support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable Data Bind support", false, 1000)]
#endif
		public static void EnableDataBindSupport()
		{
			if (CanEnableDataBindSupport())
			{
				var root = Path.GetDirectoryName(Utilites.GetAssetPath("ScriptsFolder"));

				var current_path = Utilites.GetAssetPath("DataBindFolder");
				var new_path = root + "/" + Path.GetFileName(current_path);
				if (current_path != new_path)
				{
					AssetDatabase.MoveAsset(current_path, new_path);
				}

				ScriptingDefineSymbols.Add(DataBindSupport);

				ScriptsRecompile.SetStatus("DataBind", ScriptsRecompile.StatusSymbolsAdded);
			}
		}

		/// <summary>
		/// Can enable DataBind support?
		/// </summary>
		/// <returns>true if DataBind installed and support not enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable Data Bind support", true, 1000)]
#endif
		public static bool CanEnableDataBindSupport()
		{
			if (!DataBindInstalled)
			{
				return false;
			}

			return !ScriptingDefineSymbols.Contains(DataBindSupport);
		}

		/// <summary>
		/// Disable DataBind support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable Data Bind support", false, 1010)]
#endif
		public static void DisableDataBindSupport()
		{
			if (CanDisableDataBindSupport())
			{
				var root = Path.GetDirectoryName(Utilites.GetAssetPath("ScriptsFolder"));

				var current_path = Utilites.GetAssetPath("DataBindFolder");
				var new_path = root + "/Scripts/ThirdPartySupport/" + Path.GetFileName(current_path);
				if (current_path != new_path)
				{
					AssetDatabase.MoveAsset(current_path, new_path);
				}

				ScriptingDefineSymbols.Remove(DataBindSupport);
			}
		}

		/// <summary>
		/// Can disable DataBind support?
		/// </summary>
		/// <returns>true if DataBind support enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable Data Bind support", true, 1010)]
#endif
		public static bool CanDisableDataBindSupport()
		{
			return ScriptingDefineSymbols.Contains(DataBindSupport);
		}
#endregion
#endif

		#region TMProSupport

		const string TMProSupport = "UIWIDGETS_TMPRO_SUPPORT";

		const string TMProAssemblies = "l:UiwidgetsTMProRequiredAssemblyDefinition";

		const string TMProPackage = "Unity.TextMeshPro";

		/// <summary>
		/// Is TextMesh Pro installed?
		/// </summary>
		public static bool TMProInstalled
		{
			get
			{
				return !string.IsNullOrEmpty(Compatibility.GetTMProVersion());
			}
		}

		/// <summary>
		/// Enable DataBind Support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable TextMesh Pro support", false, 1020)]
#endif
		public static void EnableTMProSupport()
		{
			if (CanEnableTMProSupport())
			{
				ScriptingDefineSymbols.Add(TMProSupport);
				AssemblyDefinitionsEditor.Add(TMProAssemblies, TMProPackage);

				ScriptsRecompile.SetStatus("TMPro", ScriptsRecompile.StatusSymbolsAdded);

				ImportTMProPackage();
			}
		}

		static bool IsTMProPrefabsInstalled()
		{
			var key = "l:UiwidgetsAccordionTMProPrefab";
			var guids = AssetDatabase.FindAssets(key);

			return guids.Length > 0;
		}

		/// <summary>
		/// Can enable TMPro support?
		/// </summary>
		/// <returns>true if TMPro installed and support not enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Enable TextMesh Pro support", true, 1020)]
#endif
		public static bool CanEnableTMProSupport()
		{
			if (!TMProInstalled)
			{
				return false;
			}

			return !ScriptingDefineSymbols.Contains(TMProSupport);
		}

		/// <summary>
		/// Disable TMPro support.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable TextMesh Pro support", false, 1030)]
#endif
		public static void DisableTMProSupport()
		{
			if (CanDisableTMProSupport())
			{
				ScriptingDefineSymbols.Remove(TMProSupport);
				AssemblyDefinitionsEditor.Remove(TMProAssemblies, TMProPackage);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can disable TMPro support?
		/// </summary>
		/// <returns>true if TMPro suppport enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Disable TextMesh Pro support", true, 1030)]
#endif
		public static bool CanDisableTMProSupport()
		{
			return ScriptingDefineSymbols.Contains(TMProSupport);
		}

		/// <summary>
		/// Import TMPro support package.
		/// </summary>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Import TextMesh Pro support package", false, 1040)]
#endif
		public static void ImportTMProPackage()
		{
			var version = Compatibility.GetTMProVersion();
			var path = Utilites.GetAssetPath("TMProSupport" + version + "Package");
			var interactive = IsTMProPrefabsInstalled();
			Compatibility.ImportPackage(path, interactive);

			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Can import TMPro support package?
		/// </summary>
		/// <returns>true if TMPro suppport enabled; otherwise false.</returns>
#if !UNITY_2018_3_OR_NEWER
		[MenuItem("Edit/Project Settings/New UI Widgets/Import TextMesh Pro support package", true, 1040)]
#endif
		public static bool CanImportTMProPackage()
		{
			return ScriptingDefineSymbols.Contains(TMProSupport);
		}
#endregion
	}
}