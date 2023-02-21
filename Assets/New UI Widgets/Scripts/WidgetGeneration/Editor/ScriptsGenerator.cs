#if UNITY_EDITOR
namespace UIWidgets.WidgetGeneration
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Widget scripts generator.
	/// </summary>
	public class ScriptsGenerator : IFormattable
	{
		/// <summary>
		/// Class info.
		/// </summary>
		public ClassInfo Info;

		/// <summary>
		/// Path to save created files.
		/// </summary>
		protected string SavePath;

		/// <summary>
		/// Path to save created editor scripts.
		/// </summary>
		protected string EditorSavePath;

		/// <summary>
		/// Path to save created widgets scripts.
		/// </summary>
		protected string ScriptSavePath;

		/// <summary>
		/// Path to save created prefab.
		/// </summary>
		protected string PrefabSavePath;

		/// <summary>
		/// Scripts templates.
		/// </summary>
		protected Dictionary<string, string> ScriptsTemplates;

		/// <summary>
		/// Script templates values.
		/// </summary>
		protected Dictionary<string, string> TemplateValues;

		/// <summary>
		/// Script editor tempplates.
		/// </summary>
		protected HashSet<string> EditorTemplates = new HashSet<string>()
		{
			"MenuOptions",
			"PrefabGenerator",
			"PrefabGeneratorAutocomplete",
			"PrefabGeneratorTable",
			"PrefabGeneratorScene",
		};

		/// <summary>
		/// Prefabs.
		/// </summary>
		protected List<string> Prefabs = new List<string>()
		{
			"ListView",
			"DragInfo",
			"Combobox",
			"ComboboxMultiselect",
			"Table",
			"TileView",
			"TreeView",
			"TreeGraph",
			"PickerListView",
			"PickerTreeView",
			"Autocomplete",
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptsGenerator"/> class.
		/// </summary>
		/// <param name="info">Class info.</param>
		/// <param name="path">Path to save created files.</param>
		public ScriptsGenerator(ClassInfo info, string path)
		{
			Info = info;
			SavePath = path + "/Widgets" + info.ShortTypeName;

			ScriptsTemplates = new Dictionary<string, string>()
			{
				// collections
				{ "Autocomplete", "Autocomplete" + info.ShortTypeName },
				{ "Combobox", "Combobox" + info.ShortTypeName },
				{ "ListView", "ListView" + info.ShortTypeName },
				{ "ListViewComponent", "ListViewComponent" + info.ShortTypeName },
				{ "TreeGraph", "TreeGraph" + info.ShortTypeName },
				{ "TreeGraphComponent", "TreeGraphComponent" + info.ShortTypeName },
				{ "TreeView", "TreeView" + info.ShortTypeName },
				{ "TreeViewComponent", "TreeViewComponent" + info.ShortTypeName },
				{ "Comparers", "Comparers" + info.ShortTypeName },

				// collections support
				{ "ListViewDragSupport", "ListViewDragSupport" + info.ShortTypeName },
				{ "ListViewDropSupport", "ListViewDropSupport" + info.ShortTypeName },
				{ "TreeViewDropSupport", "TreeViewDropSupport" + info.ShortTypeName },
				{ "TreeViewNodeDragSupport", "TreeViewNodeDragSupport" + info.ShortTypeName },
				{ "TreeViewNodeDropSupport", "TreeViewNodeDropSupport" + info.ShortTypeName },

				// dialogs
				{ "PickerListView", "PickerListView" + info.ShortTypeName },
				{ "PickerTreeView", "PickerTreeView" + info.ShortTypeName },

				// test
				{ "Test", "Test" + info.ShortTypeName },
				{ "TestItem", "TestItem" + info.ShortTypeName },

				// menu
				{ "MenuOptions", "MenuOptions" + info.ShortTypeName },

				// menu
				{ "PrefabGenerator", "PrefabGenerator" + info.ShortTypeName },
				{ "PrefabGeneratorAutocomplete", "PrefabGeneratorAutocomplete" + info.ShortTypeName },
				{ "PrefabGeneratorTable", "PrefabGeneratorTable" + info.ShortTypeName },
				{ "PrefabGeneratorScene", "PrefabGeneratorScene" + info.ShortTypeName },
			};
		}

		void SetAvailableScripts()
		{
			foreach (var template in ScriptsTemplates)
			{
				if (CanCreateScript(template.Key))
				{
					Info.Scripts[template.Key] = !File.Exists(Script2Filename(template.Key));
				}
			}
		}

		void SetAvailablePrefabs()
		{
			foreach (var prefab in Prefabs)
			{
				if (CanCreateWidget(prefab))
				{
					Info.Prefabs[prefab] = !File.Exists(Prefab2Filename(prefab));
				}
			}
		}

		void SetAvailableScenes()
		{
			Info.Scenes["TestScene"] = !File.Exists(Scene2Filename(Info.ShortTypeName));
		}

		void SetTemplateValues()
		{
			TemplateValues = new Dictionary<string, string>()
			{
				{ "WidgetsNamespace", Info.WidgetsNamespace },
				{ "SourceClassShortName", Info.ShortTypeName },
				{ "SourceClass", Info.FullTypeName },
				{ "AutocompleteField", Info.AutocompleteField },
				{ "ComparersEnum", "ComparersFields" + Info.ShortTypeName },
				{ "Info", Utilites.Serialize(Info) },
				{ "Path", Utilites.Serialize(SavePath) },
				{ "TextType", Utilites.GetFriendlyTypeName(Info.TextFieldType) },
				{ "AutocompleteSuffix", Info.TMProInputField != null ? "TMPro" : string.Empty },
				{ "AutocompleteNamespace", Info.TMProInputField != null ? ".TMProSupport" : string.Empty },
				{ "AutocompleteInput", Utilites.GetFriendlyTypeName(Info.InputField) },
				{ "AutocompleteText", Utilites.GetFriendlyTypeName(Info.InputText) },
			};
		}

		/// <summary>
		/// Generate files.
		/// </summary>
		public void Generate()
		{
			EditorSavePath = SavePath + Path.DirectorySeparatorChar + "Editor";
			Directory.CreateDirectory(EditorSavePath);

			ScriptSavePath = SavePath + Path.DirectorySeparatorChar + "Scripts";
			Directory.CreateDirectory(ScriptSavePath);

			PrefabSavePath = SavePath + Path.DirectorySeparatorChar + "Prefabs";
			Directory.CreateDirectory(PrefabSavePath);

			SetAvailableScripts();
			SetAvailablePrefabs();
			SetAvailableScenes();

			OverwriteRequestWindow.Open(this, GenerateScripts);
		}

		void GenerateScripts()
		{
			if (Info.Scenes["TestScene"])
			{
				if (!Compatibility.SceneSave())
				{
					EditorUtility.DisplayDialog("Widget Generation", "Please save scene to continue.", "OK");
					return;
				}

				Compatibility.SceneNew();
			}

			SetTemplateValues();

			try
			{
				var progress = 0;
				ProgressbarUpdate(progress);

				foreach (var template in ScriptsTemplates)
				{
					CreateScript(template.Key);

					progress++;
					ProgressbarUpdate(progress);
				}
			}
			catch (Exception)
			{
				EditorUtility.ClearProgressBar();
				throw;
			}

			AssetDatabase.Refresh();
		}

		void ProgressbarUpdate(int progress)
		{
			if (progress < ScriptsTemplates.Count)
			{
				EditorUtility.DisplayProgressBar("Widget Generation", "Step 1. Creating scripts.", progress / (float)ScriptsTemplates.Count);
			}
			else
			{
				EditorUtility.ClearProgressBar();
			}
		}

		/// <summary>
		/// Is script can be created?
		/// </summary>
		/// <param name="name">Script name.</param>
		/// <returns>True if script can be created; otherwise false.</returns>
		protected virtual bool CanCreateScript(string name)
		{
			if (name == "Autocomplete")
			{
				return !string.IsNullOrEmpty(Info.AutocompleteField);
			}

			if (name == "Table")
			{
				return Info.TextFields.Count > 0;
			}

			return true;
		}

		/// <summary>
		/// Is widget can be created?
		/// </summary>
		/// <param name="name">Widget name.</param>
		/// <returns>True if widget can be created; otherwise false.</returns>
		protected virtual bool CanCreateWidget(string name)
		{
			if (name == "PrefabGeneratorAutocomplete")
			{
				return !string.IsNullOrEmpty(Info.AutocompleteField);
			}

			if (name == "PrefabGeneratorTable")
			{
				return Info.TextFields.Count > 0;
			}

			if (name == "TestItem")
			{
				return Info.ParameterlessConstructor;
			}

			return CanCreateScript(name);
		}

		/// <summary>
		/// Get script filename by script name.
		/// </summary>
		/// <param name="script">Script.</param>
		/// <returns>Filename.</returns>
		public string Script2Filename(string script)
		{
			var classname = ScriptsTemplates[script];
			var dir = EditorTemplates.Contains(script) ? EditorSavePath : ScriptSavePath;

			return dir + Path.DirectorySeparatorChar + classname + ".cs";
		}

		/// <summary>
		/// Get prefab filename by widget name.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Filename.</returns>
		public string Prefab2Filename(string type)
		{
			return PrefabSavePath + "/" + type + Info.ShortTypeName + ".prefab";
		}

		/// <summary>
		/// Get prefab filename by widget name.
		/// </summary>
		/// <param name="scene">Scene.</param>
		/// <returns>Filename.</returns>
		public string Scene2Filename(string scene)
		{
			return SavePath + "/" + scene + ".unity";
		}

		/// <summary>
		/// Create script by the specified template.
		/// </summary>
		/// <param name="type">Template type.</param>
		protected virtual void CreateScript(string type)
		{
			if (!Info.Scripts.ContainsKey(type))
			{
				return;
			}

			if (!Info.Scripts[type])
			{
				return;
			}

			var filename = Script2Filename(type);
			var code = GetScriptCode(type);

			File.WriteAllText(filename, code);
		}

		/// <summary>
		/// Get template.
		/// </summary>
		/// <param name="type">Template type.</param>
		/// <returns>Template.</returns>
		protected virtual string GetTemplate(string type)
		{
			var suffix = CanCreateWidget(type) ? string.Empty : "Off";

			var path = Utilites.GetAssetPath(type + suffix + "ScriptTemplate");
			if (path == null)
			{
				Debug.LogWarning("Script template " + type + suffix + " not found");
				return "// Script template " + type + suffix + " not found";
			}

			return Compatibility.LoadAssetAtPath<TextAsset>(path).text;
		}

		/// <summary>
		/// Get script text.
		/// </summary>
		/// <param name="type">Template type.</param>
		/// <returns>Script text.</returns>
		protected virtual string GetScriptCode(string type)
		{
			return string.Format(GetTemplate(type), this);
		}

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value.</param>
		/// <returns>The value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (TemplateValues.ContainsKey(format))
			{
				return TemplateValues[format];
			}

			if (format.EndsWith("Class"))
			{
				var key = format.Substring(0, format.Length - "Class".Length);
				if (ScriptsTemplates.ContainsKey(key))
				{
					return ScriptsTemplates[key];
				}
			}

			var pos = format.IndexOf("@");
			if (pos != -1)
			{
				return ToStringList(format, formatProvider);
			}

			throw new ArgumentOutOfRangeException("Unsupported format: " + format);
		}

		List<Type> typesInt = new List<Type>()
		{
			typeof(decimal),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
		};

		List<Type> typesFloat = new List<Type>()
		{
			typeof(float),
			typeof(double),
		};

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value.</param>
		/// <returns>The value of the current instance in the specified format.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed.")]
		protected string ToStringList(string format, IFormatProvider formatProvider)
		{
			var template = format.Split(new[] { "@" }, 2, StringSplitOptions.None);
			template[1] = template[1].Replace("[", "{").Replace("]", "}");

			switch (template[0])
			{
				case "IfTMProText":
					return Info.IsTMProText ? string.Format(template[1], this) : string.Empty;
				case "!IfTMProText":
					return !Info.IsTMProText ? string.Format(template[1], this) : string.Empty;
				case "IfTMProInputField":
					return Info.IsTMProInputField ? string.Format(template[1], this) : string.Empty;
				case "!IfTMProInputField":
					return !Info.IsTMProInputField ? string.Format(template[1], this) : string.Empty;
				case "IfAutocomplete":
					return CanCreateWidget("Autocomplete") ? string.Format(template[1], this) : string.Empty;
				case "IfTable":
					return CanCreateWidget("Table") ? string.Format(template[1], this) : string.Empty;
				case "Fields":
					return Info.Fields.ToString(template[1], this, formatProvider);
				case "TextFields":
					return Info.TextFields.ToString(template[1], this, formatProvider);
				case "TextFieldFirst":
					return Info.TextFieldFirst.ToString(template[1], this, formatProvider);
				case "TableFieldFirst":
					return Info.TableFieldFirst.ToString(template[1], this, formatProvider);
				case "ImageFields":
					var fields_image = Info.Fields.Where(x => x.IsImage).ToList();
					return fields_image.ToString(template[1], this, formatProvider);
				case "ImageFieldsNullable":
					var fields_image_nullable = Info.Fields.Where(x => x.IsImage && x.IsNullable).ToList();
					return fields_image_nullable.ToString(template[1], this, formatProvider);
				case "TreeViewFields":
					var fields_tree = Info.Fields.Where(x => x.WidgetFieldName != "Text" && x.WidgetFieldName != "Icon").ToList();
					return fields_tree.ToString(template[1], this, formatProvider);
				case "FieldsString":
					var fields_string = Info.Fields.Where(x => x.FieldType == typeof(string)).ToList();
					return fields_string.ToString(template[1], this, formatProvider);
				case "FieldsStringFirst":
					var fields_string_first = Info.Fields.Where(x => x.FieldType == typeof(string)).Take(1).ToList();
					return fields_string_first.ToString(template[1], this, formatProvider);
				case "FieldsInt":
					var fields_int = Info.Fields.Where(x => typesInt.Contains(x.FieldType)).ToList();
					return fields_int.ToString(template[1], this, formatProvider);
				case "FieldsFloat":
					var fields_float = Info.Fields.Where(x => typesFloat.Contains(x.FieldType)).ToList();
					return fields_float.ToString(template[1], this, formatProvider);
				case "FieldsSprite":
					var fields_sprite = Info.Fields.Where(x => x.FieldType == typeof(Sprite)).ToList();
					return fields_sprite.ToString(template[1], this, formatProvider);
				case "FieldsTexture2D":
					var fields_texture = Info.Fields.Where(x => x.FieldType == typeof(Texture2D)).ToList();
					return fields_texture.ToString(template[1], this, formatProvider);
				case "FieldsColor":
					var fields_color = Info.Fields.Where(x => (x.FieldType == typeof(Color)) || x.FieldType == typeof(Color32)).ToList();
					return fields_color.ToString(template[1], this, formatProvider);
				default:
					throw new ArgumentOutOfRangeException("Unsupported format: " + format);
			}
		}
	}
}
#endif