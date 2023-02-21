#if UIWIDGETS_DATABIND_SUPPORT && UNITY_EDITOR
namespace UIWidgets.DataBindSupport
{
	using System;
	using System.IO;
	using UnityEditor;

	class MenuOptions
	{
		static string GetPath()
		{
			return Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
		}

		[MenuItem("Assets/Create/New UI Widgets/Add Data Bind support", false)]
		public static void AddDataBindUISupport()
		{
			DataBindGenerator.Run(GetSelectedType(), GetPath());
		}

		[MenuItem("Assets/Create/New UI Widgets/Add Data Bind support", true)]
		public static bool AddDataBindUISupportValidation()
		{
			return DataBindGenerator.IsValidType(GetSelectedType());
		}

		static Type GetSelectedType()
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

			return type.GetClass();
		}
	}
}
#endif