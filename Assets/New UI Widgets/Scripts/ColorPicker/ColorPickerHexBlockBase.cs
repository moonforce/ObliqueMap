namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Color picker Hex block base class.
	/// </summary>
	public abstract class ColorPickerHexBlockBase : MonoBehaviour
	{
		/// <summary>
		/// InputFieldProxy Hex.
		/// </summary>
		[SerializeField]
		protected IInputFieldProxy InputProxyHex;

		/// <summary>
		/// Is process color with alpha value?
		/// </summary>
		[SerializeField]
		protected bool withAlpha = true;

		/// <summary>
		/// Is process color with alpha value?
		/// </summary>
		public bool WithAlpha
		{
			get
			{
				return withAlpha;
			}

			set
			{
				withAlpha = value;
				UpdateInputs();
			}
		}

		ColorPickerInputMode inputMode;

		/// <summary>
		/// Gets or sets the input mode.
		/// </summary>
		/// <value>The input mode.</value>
		public ColorPickerInputMode InputMode
		{
			get
			{
				return inputMode;
			}

			set
			{
				inputMode = value;
			}
		}

		ColorPickerPaletteMode paletteMode;

		/// <summary>
		/// Gets or sets the palette mode.
		/// </summary>
		/// <value>The palette mode.</value>
		public ColorPickerPaletteMode PaletteMode
		{
			get
			{
				return paletteMode;
			}

			set
			{
				paletteMode = value;
			}
		}

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		public ColorRGBChangedEvent OnChangeRGB = new ColorRGBChangedEvent();

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			InitInput();

			AddListeners();

			UpdateInputs();
		}

		/// <summary>
		/// Inits the input.
		/// </summary>
		protected abstract void InitInput();

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			InputProxyHex.onEndEdit.AddListener(InputChanged);
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			InputProxyHex.onEndEdit.RemoveListener(InputChanged);
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		protected virtual void UpdateInputs()
		{
			if (InputProxyHex != null)
			{
				InputProxyHex.text = WithAlpha ? Utilites.RGBA2Hex(currentColor) : Utilites.RGB2Hex(currentColor);
			}
		}

		/// <summary>
		/// Process input changed event.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void InputChanged(string input)
		{
			Color32 color;
			if (Utilites.TryHexToRGBA(input, out color))
			{
				if (!WithAlpha)
				{
					color.a = currentColor.a;
				}

				currentColor = color;
				OnChangeRGB.Invoke(currentColor);
			}
		}

		/// <summary>
		/// Current color.
		/// </summary>
		protected Color32 currentColor;

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(Color32 color)
		{
			currentColor = color;
			UpdateInputs();
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(ColorHSV color)
		{
			currentColor = color;
			UpdateInputs();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public abstract void SetStyle(StyleColorPicker styleColorPicker, Style style);
	}
}