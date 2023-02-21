namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
#if UNITY_EDITOR
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using UnityEditor;
#endif

	/// <summary>
	/// Utilites.
	/// </summary>
	public static class Utilites
	{
#if UNITY_EDITOR
		/// <summary>
		/// Get friendly name of the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Friendly name.</returns>
		public static string GetFriendlyTypeName(Type type)
		{
			var friendly_name = type.Name;

			if (type.IsGenericType)
			{
				var backtick_index = friendly_name.IndexOf('`');
				if (backtick_index > 0)
				{
					friendly_name = friendly_name.Remove(backtick_index);
				}

				friendly_name += "<";

				var type_parameters = type.GetGenericArguments();
				for (int i = 0; i < type_parameters.Length; ++i)
				{
					var type_param_name = GetFriendlyTypeName(type_parameters[i]);
					friendly_name += i == 0 ? type_param_name : "," + type_param_name;
				}

				friendly_name += ">";
			}

			return string.IsNullOrEmpty(type.Namespace) ? friendly_name : type.Namespace + "." + friendly_name;
		}

		/// <summary>
		/// Get type by full name.
		/// </summary>
		/// <param name="typename">Type name.</param>
		/// <returns>Type.</returns>
		public static Type GetType(string typename)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var assembly_type in assembly.GetTypes())
				{
					if (assembly_type.FullName == typename)
					{
						return assembly_type;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Creates the object by path to asset prefab.
		/// </summary>
		/// <returns>The created object.</returns>
		/// <param name="path">Path.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="setParent">Set parent for the created gameobject.</param>
		static GameObject CreateObject(string path, Transform parent = null, bool setParent = true)
		{
			var prefab = Compatibility.LoadAssetAtPath<GameObject>(path);
			if (prefab == null)
			{
				throw new ArgumentException(string.Format("Prefab not found at path {0}.", path));
			}

			var go = Compatibility.Instantiate(prefab);

			if (setParent)
			{
				var go_parent = (parent != null) ? parent : Selection.activeTransform;
				if ((go_parent == null) || ((go_parent.gameObject.transform as RectTransform) == null))
				{
					go_parent = GetCanvasTransform();
				}

				if (go_parent != null)
				{
					go.transform.SetParent(go_parent, false);
				}
			}

			go.name = prefab.name;

			var rectTransform = go.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.anchoredPosition = new Vector2(0, 0);

				FixInstantiated(prefab, go);
			}

			return go;
		}

		/// <summary>
		/// Find assets by specified search.
		/// </summary>
		/// <typeparam name="T">Assets type.</typeparam>
		/// <param name="search">Search string.</param>
		/// <returns>Assets list.</returns>
		public static List<T> GetAssets<T>(string search)
			where T : UnityEngine.Object
		{
			var guids = AssetDatabase.FindAssets(search);

			var result = new List<T>(guids.Length);
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = Compatibility.LoadAssetAtPath<T>(path);
				if (asset != null)
				{
					result.Add(asset);
				}
			}

			return result;
		}

		/// <summary>
		/// Creates the object from asset.
		/// </summary>
		/// <returns>The object from asset.</returns>
		/// <param name="key">Search string.</param>
		/// <param name="undoName">Undo name.</param>
		static GameObject CreateObjectFromAsset(string key, string undoName)
		{
			var path = GetAssetPath(key);
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			var go = CreateObject(path);

			Undo.RegisterCreatedObjectUndo(go, undoName);
			Selection.activeObject = go;

			return go;
		}

		/// <summary>
		/// Get asset by label.
		/// </summary>
		/// <typeparam name="T">Asset type.</typeparam>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset.</returns>
		public static T GetAsset<T>(string label)
			where T : UnityEngine.Object
		{
			var path = GetAssetPath(label + "Asset");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return Compatibility.LoadAssetAtPath<T>(path);
		}

		/// <summary>
		/// Get asset path by label.
		/// </summary>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset path.</returns>
		public static string GetAssetPath(string label)
		{
			var key = "l:Uiwidgets" + label;
			var guids = AssetDatabase.FindAssets(key);
			if (guids.Length == 0)
			{
				Debug.LogError("Label not found: " + label);
				return null;
			}

			return AssetDatabase.GUIDToAssetPath(guids[0]);
		}

		/// <summary>
		/// Get prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		public static GameObject GetPrefab(string label)
		{
			var path = GetAssetPath(label + "Prefab");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return Compatibility.LoadAssetAtPath<GameObject>(path);
		}

		/// <summary>
		/// Get generated prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		public static GameObject GetGeneratedPrefab(string label)
		{
			return GetPrefab("Generated" + label);
		}

		/// <summary>
		/// Set prefabs label.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		public static void SetPrefabLabel(UnityEngine.Object prefab)
		{
			AssetDatabase.SetLabels(prefab, new[] { "UiwidgetsGenerated" + prefab.name + "Prefab", });
		}

		/// <summary>
		/// Create widget template from asset specified by label.
		/// </summary>
		/// <param name="templateLabel">Template label.</param>
		/// <returns>Widget template.</returns>
		public static GameObject CreateWidgetTemplateFromAsset(string templateLabel)
		{
			var path = GetAssetPath(templateLabel + "Prefab");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return CreateObject(path, null, false);
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <returns>Created GameObject.</returns>
		/// <param name="widget">Widget name.</param>
		/// <param name="applyStyle">Apply style to created widget.</param>
		public static GameObject CreateWidgetFromAsset(string widget, bool applyStyle = true)
		{
			var go = CreateObjectFromAsset(widget + "Prefab", "Create " + widget);

			if ((go != null) && applyStyle)
			{
				var style = Style.DefaultStyle();
				if (style != null)
				{
					style.ApplyTo(go);
				}
			}

			return go;
		}

		/// <summary>
		/// Gets the canvas transform.
		/// </summary>
		/// <returns>The canvas transform.</returns>
		public static Transform GetCanvasTransform()
		{
			var canvas = (Selection.activeGameObject != null) ? Selection.activeGameObject.GetComponentInParent<Canvas>() : null;
			if (canvas == null)
			{
				canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
			}

			if (canvas != null)
			{
				return canvas.transform;
			}

			var canvasGO = new GameObject("Canvas");
			canvasGO.layer = LayerMask.NameToLayer("UI");
			canvas = canvasGO.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler>();
			canvasGO.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(canvasGO, "Create " + canvasGO.name);

			if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
			{
				Compatibility.CreateEventSystem();
			}

			return canvasGO.transform;
		}

		/// <summary>
		/// Serialize object with BinaryFormatter to base64 string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <returns>Serialized string.</returns>
		public static string Serialize(object obj)
		{
			var serializer = new BinaryFormatter();

			using (var ms = new MemoryStream())
			{
				serializer.Serialize(ms, obj);
				return Convert.ToBase64String(ms.ToArray());
			}
		}

		/// <summary>
		/// Deserialize object with BinaryFormatter from base64 string.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <param name="encoded">Serialized string.</param>
		/// <returns>Deserialized object.</returns>
		public static T Deserialize<T>(string encoded)
		{
			var serializer = new BinaryFormatter();
			var ms = new MemoryStream();

			var bytes = Convert.FromBase64String(encoded);
			ms.Write(bytes, 0, bytes.Length);
			ms.Seek(0, SeekOrigin.Begin);

			return (T)serializer.Deserialize(ms);
		}
#endif

		/// <summary>
		/// Function to get time to use with animations.
		/// Can be replaced with custom function.
		/// </summary>
		public static Func<bool, float> GetTime = DefaultGetTime;

		/// <summary>
		/// Function to get unscaled time to use by widgets.
		/// Can be replaced with custom function.
		/// </summary>
		public static Func<float> GetUnscaledTime = DefaultGetUnscaledTime;

		/// <summary>
		/// Default GetTime function fron the default Time class.
		/// </summary>
		/// <param name="unscaledTime">Return unscaled time.</param>
		/// <returns>Time.</returns>
		public static float DefaultGetTime(bool unscaledTime)
		{
			return unscaledTime ? Time.unscaledTime : Time.time;
		}

		/// <summary>
		/// Default GetUnscaledTime function.
		/// </summary>
		/// <returns>Default Time.unscaledTime.</returns>
		public static float DefaultGetUnscaledTime()
		{
			return Time.unscaledTime;
		}

		/// <summary>
		/// Create list.
		/// </summary>
		/// <typeparam name="T">Type of the item.</typeparam>
		/// <param name="count">Items count.</param>
		/// <param name="create">Function to create item.</param>
		/// <returns>List.</returns>
		public static ObservableList<T> CreateList<T>(int count, Func<int, T> create)
		{
			var result = new ObservableList<T>(true, count);

			result.BeginUpdate();

			for (int i = 1; i <= count; i++)
			{
				result.Add(create(i));
			}

			result.EndUpdate();

			return result;
		}

		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <typeparam name="T">Item type.</typeparam>
		/// <param name="source">Items.</param>
		/// <param name="match">The Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
		/// <returns>A List&lt;T&gt; containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty List&lt;T&gt;.</returns>
		public static ObservableList<T> FindAll<T>(List<T> source, Func<T, bool> match)
		{
			var result = new ObservableList<T>();

			for (int i = 0; i < source.Count; i++)
			{
				if (match(source[i]))
				{
					result.Add(source[i]);
				}
			}

			return result;
		}

		/// <summary>
		/// Get sum of the list.
		/// </summary>
		/// <param name="source">List to sum.</param>
		/// <returns>Sum.</returns>
		public static float Sum(List<float> source)
		{
			var result = 0f;

			for (int i = 0; i < source.Count; i++)
			{
				result += source[i];
			}

			return result;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <returns>Component.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static T GetOrAddComponent<T>(Component obj)
			where T : Component
		{
			var component = obj.GetComponent<T>();
			if (component == null)
			{
				component = obj.gameObject.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <returns>Component.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static T GetOrAddComponent<T>(GameObject obj)
			where T : Component
		{
			var component = obj.GetComponent<T>();
			if (component == null)
			{
				component = obj.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Fixs the instantiated object (in some cases object have wrong position, rotation and scale).
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="instance">Instance.</param>
		public static void FixInstantiated(Component source, Component instance)
		{
			FixInstantiated(source.gameObject, instance.gameObject);
		}

		/// <summary>
		/// Fix the instantiated object (in some cases object have wrong position, rotation and scale).
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="instance">Instance.</param>
		public static void FixInstantiated(GameObject source, GameObject instance)
		{
			var defaultRectTransform = source.transform as RectTransform;
			if (defaultRectTransform == null)
			{
				return;
			}

			var rectTransform = instance.transform as RectTransform;

			rectTransform.localPosition = defaultRectTransform.localPosition;
			rectTransform.localRotation = defaultRectTransform.localRotation;
			rectTransform.localScale = defaultRectTransform.localScale;
			rectTransform.anchoredPosition = defaultRectTransform.anchoredPosition;
			rectTransform.sizeDelta = defaultRectTransform.sizeDelta;
		}

		/// <summary>
		/// Finds the canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		public static Transform FindCanvas(Transform currentObject)
		{
			var canvas = currentObject.GetComponentInParent<Canvas>();
			if (canvas == null)
			{
				return null;
			}

			return canvas.transform;
		}

		/// <summary>
		/// Finds the topmost canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		public static Transform FindTopmostCanvas(Transform currentObject)
		{
			var canvases = currentObject.GetComponentsInParent<Canvas>();
			if (canvases.Length == 0)
			{
				return null;
			}

			return canvases[canvases.Length - 1].transform;
		}

		/// <summary>
		/// Calculates the drag position.
		/// </summary>
		/// <returns>The drag position.</returns>
		/// <param name="screenPosition">Screen position.</param>
		/// <param name="canvas">Canvas.</param>
		/// <param name="canvasRect">Canvas rect.</param>
		public static Vector3 CalculateDragPosition(Vector3 screenPosition, Canvas canvas, RectTransform canvasRect)
		{
			Vector3 result;
			var canvasSize = canvasRect.sizeDelta;
			Vector2 min = Vector2.zero;
			Vector2 max = canvasSize;

			var isOverlay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;
			var noCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null;
			if (isOverlay || noCamera)
			{
				result = screenPosition;
			}
			else
			{
				var ray = canvas.worldCamera.ScreenPointToRay(screenPosition);
				var plane = new Plane(canvasRect.forward, canvasRect.position);

				float distance;
				plane.Raycast(ray, out distance);

				result = canvasRect.InverseTransformPoint(ray.origin + (ray.direction * distance));

				min = -Vector2.Scale(max, canvasRect.pivot);
				max = canvasSize - min;
			}

			result.x = Mathf.Clamp(result.x, min.x, max.y);
			result.y = Mathf.Clamp(result.y, min.x, max.y);

			return result;
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		[Obsolete("Use LayoutUtilites.UpdateLayout() instead.")]
		public static void UpdateLayout(LayoutGroup layout)
		{
			LayoutUtilites.UpdateLayout(layout);
		}

		/// <summary>
		/// Get top left corner position of specified RectTransform.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <returns>Top left corner position.</returns>
		public static Vector2 GetTopLeftCorner(RectTransform rect)
		{
			var size = rect.rect.size;
			var pos = rect.anchoredPosition;
			var pivot = rect.pivot;

			pos.x -= size.x * pivot.x;
			pos.y += size.y * (1f - pivot.y);

			return pos;
		}

		/// <summary>
		/// Set top left corner position of specified RectTransform.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <param name="position">Top left corner position.</param>
		public static void SetTopLeftCorner(RectTransform rect, Vector2 position)
		{
			var delta = position - GetTopLeftCorner(rect);
			rect.anchoredPosition += delta;
		}

		/// <summary>
		/// Suspends the coroutine execution for the given amount of seconds using unscaled time.
		/// </summary>
		/// <param name="seconds">Delay in seconds.</param>
		/// <returns>Yield instruction to wait.</returns>
		public static IEnumerator WaitForSecondsUnscaled(float seconds)
		{
			var end_time = GetUnscaledTime() + seconds;
			while (GetUnscaledTime() < end_time)
			{
				yield return null;
			}
		}

		/// <summary>
		/// Check how much time takes to run specified action.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="log">Text to add to log.</param>
		public static void CheckRunTime(Action action, string log = "")
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			action();

			sw.Stop();
			var ts = sw.Elapsed;

			string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:0000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
			Debug.Log("RunTime " + elapsedTime + "; " + log);
		}

		/// <summary>
		/// Determines if slider is horizontal.
		/// </summary>
		/// <returns><c>true</c> if slider is horizontal; otherwise, <c>false</c>.</returns>
		/// <param name="slider">Slider.</param>
		public static bool IsHorizontal(Slider slider)
		{
			return slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft;
		}

		/// <summary>
		/// Convert specified color to RGB hex.
		/// </summary>
		/// <returns>RGB hex.</returns>
		/// <param name="c">Color.</param>
		public static string RGB2Hex(Color32 c)
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", c.r, c.g, c.b);
		}

		/// <summary>
		/// Convert specified color to RGBA hex.
		/// </summary>
		/// <returns>RGBA hex.</returns>
		/// <param name="c">Color.</param>
		public static string RGBA2Hex(Color32 c)
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
		}

		/// <summary>
		/// Converts the string representation of a number to its Byte equivalent. A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <returns><c>true</c> if hex was converted successfully; otherwise, <c>false</c>.</returns>
		/// <param name="hex">A string containing a number to convert.</param>
		/// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent to the number contained in s if the conversion succeeded, or zero if the conversion failed. The conversion fails if the s parameter is null or String.Empty, is not of the correct format, or represents a number less than MinValue or greater than MaxValue. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
		public static bool TryParseHex(string hex, out byte result)
		{
			return byte.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out result);
		}

		/// <summary>
		/// Converts the string representation of a color to its Color equivalent. A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <returns><c>true</c> if hex was converted successfully; otherwise, <c>false</c>.</returns>
		/// <param name="hex">A string containing a colot to convert.</param>
		/// <param name="result">When this method returns, contains the color value equivalent to the color contained in hex if the conversion succeeded, or Color.black if the conversion failed. The conversion fails if the hex parameter is null or String.Empty, is not of the correct format. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
		public static bool TryHexToRGBA(string hex, out Color32 result)
		{
			result = Color.black;

			if (string.IsNullOrEmpty(hex))
			{
				return false;
			}

			var h = hex.Trim(new char[] { '#', ';' });
			byte r, g, b, a;

			if (h.Length == 8)
			{
				if (!TryParseHex(h.Substring(0, 2), out r))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(2, 2), out g))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(4, 2), out b))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(6, 2), out a))
				{
					return false;
				}
			}
			else if (h.Length == 6)
			{
				if (!TryParseHex(h.Substring(0, 2), out r))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(2, 2), out g))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(4, 2), out b))
				{
					return false;
				}

				a = 255;
			}
			else if (h.Length == 3)
			{
				if (!TryParseHex(h.Substring(0, 1), out r))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(1, 1), out g))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(2, 1), out b))
				{
					return false;
				}

				r *= 17;
				g *= 17;
				b *= 17;
				a = 255;
			}
			else
			{
				return false;
			}

			result = new Color32(r, g, b, a);

			return true;
		}

		/// <summary>
		/// Is two float values is nearly equal?
		/// </summary>
		/// <param name="a">First value.</param>
		/// <param name="b">Second value.</param>
		/// <param name="epsilon">Epsilon.</param>
		/// <returns>true if two float values is nearly equal; otherwise false.</returns>
		public static bool NearlyEqual(float a, float b, float epsilon)
		{
			if (a == b)
			{
				return true;
			}

			var diff = Mathf.Abs(a - b);

			if (a == 0 || b == 0 || diff < float.Epsilon)
			{
				return diff < (epsilon * float.Epsilon);
			}

			var absA = Mathf.Abs(a);
			var absB = Mathf.Abs(b);

			return diff / (absA + absB) < epsilon;
		}
	}
}