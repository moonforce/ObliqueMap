namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeViewSample component continent.
	/// </summary>
	public class TreeViewSampleComponentContinent : ComponentPool<TreeViewSampleComponentContinent>, ITreeViewSampleMultipleComponent
	{
		/// <summary>
		/// Text.
		/// </summary>
		public Text Text;

		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <param name="parent">New parent.</param>
		/// <returns>GroupedListViewComponent instance.</returns>
		public ITreeViewSampleMultipleComponent IInstance(Transform parent)
		{
			return Instance(parent);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ITreeViewSampleItem item)
		{
			SetData(item as TreeViewSampleItemContinent);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(TreeViewSampleItemContinent item)
		{
			Text.text = item.Name + " (Countries: " + item.Countries + ") ";
		}

		/// <summary>
		/// Is colors setted at least once?
		/// </summary>
		protected bool GraphicsColorSetted;

		/// <summary>
		/// Set graphics colors.
		/// </summary>
		/// <param name="foregroundColor">Foreground color.</param>
		/// <param name="backgroundColor">Background color.</param>
		/// <param name="fadeDuration">Fade duration.</param>
		public virtual void GraphicsColoring(Color foregroundColor, Color backgroundColor, float fadeDuration)
		{
			// reset default color to white, otherwise it will look darker than specified color,
			// because actual color = Text.color * Text.CrossFadeColor
			if (!GraphicsColorSetted)
			{
				Text.color = Color.white;
			}

			// change color instantly for first time
			Text.CrossFadeColor(foregroundColor, GraphicsColorSetted ? fadeDuration : 0f, true, true);

			GraphicsColorSetted = true;
		}
	}
}