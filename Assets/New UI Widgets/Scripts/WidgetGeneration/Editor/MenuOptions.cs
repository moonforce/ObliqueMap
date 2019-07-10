#if UNITY_EDITOR
namespace UIWidgets.WidgetGeneration
{
	using System.IO;
	using UnityEditor;

	/// <summary>
	/// Menu options for the widgets generation.
	/// </summary>
	public static class MenuOptions
	{
		/// <summary>
		/// Create widget.
		/// </summary>
		[MenuItem("Assets/Create/New UI Widgets/Generate Widgets", false)]
		public static void CreateWidget()
		{
			var info = GetClassInfo();

			var path = GetPath();
			var gen = new ScriptsGenerator(info, path);
			gen.Generate();
		}

		/// <summary>
		/// Can widget be created?
		/// </summary>
		/// <returns>True if widget can be created; otherwise false.</returns>
		[MenuItem("Assets/Create/New UI Widgets/Generate Widgets", true)]
		public static bool CreateWidgetValidation()
		{
			var info = GetClassInfo();

			return (info != null) && info.IsValid;
		}

		static string GetPath()
		{
			return Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
		}

		static ClassInfo GetClassInfo()
		{
			if (Selection.activeObject == null)
			{
				return null;
			}

			var type = Selection.activeObject as MonoScript;
			if (type == null)
			{
				return null;
			}

			return new ClassInfo(type.GetClass());
		}
	}
}
#endif