namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Scripting Define Symbols.
	/// </summary>
	static class ScriptingDefineSymbols
	{
		/// <summary>
		/// Add scripting define symbols.
		/// </summary>
		/// <param name="symbol">Symbol to add.</param>
		public static void Add(string symbol)
		{
			var symbols = All();

			if (symbols.Contains(symbol))
			{
				return;
			}

			symbols.Add(symbol);

			Save(symbols);
		}

		/// <summary>
		/// Remove scripting define symbols.
		/// </summary>
		/// <param name="symbol">Symbol to remove.</param>
		public static void Remove(string symbol)
		{
			var symbols = All();

			if (!symbols.Contains(symbol))
			{
				return;
			}

			symbols.Remove(symbol);

			Save(symbols);
		}

		/// <summary>
		/// Get scripting define symbols.
		/// </summary>
		/// <returns>Scripting define symbols.</returns>
		public static HashSet<string> All()
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

			return new HashSet<string>(symbols.Split(';'));
		}

		/// <summary>
		/// Check if symbol defined in scripting define symbols.
		/// </summary>
		/// <param name="symbol">Symbol.</param>
		/// <returns>True if symbol defined; otherwise false.</returns>
		public static bool Contains(string symbol)
		{
			return All().Contains(symbol);
		}

		static void Save(HashSet<string> symbols)
		{
			var arr = new string[symbols.Count];
			symbols.CopyTo(arr);

			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", arr));
			AssetDatabase.Refresh();
		}
	}
}