namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// GroupedListViewComponentGroup.
	/// </summary>
	public class GroupedListViewComponentGroup : ComponentPool<GroupedListViewComponentGroup>, IGroupedListViewComponent
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <param name="parent">New parent.</param>
		/// <returns>GroupedListViewComponent instance.</returns>
		public IGroupedListViewComponent IInstance(Transform parent)
		{
			return Instance(parent);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(IGroupedListItem item)
		{
			Name.text = item.Name;
		}

		/// <summary>
		/// Is graphics colors setted at least once?
		/// </summary>
		protected bool IsColorSetted = false;

		/// <summary>
		/// Set graphics colors.
		/// </summary>
		/// <param name="foreground">Foreground color.</param>
		/// <param name="background">Background color.</param>
		/// <param name="fadeDuration">Fade duration.</param>
		public virtual void GraphicsColoring(Color foreground, Color background, float fadeDuration)
		{
			// reset default color to white, otherwise it will look darker than specified color,
			// because actual color = Text.color * Text.CrossFadeColor
			if (!IsColorSetted)
			{
				Name.color = Color.white;
			}

			// change color instantly for first time
			Name.CrossFadeColor(foreground, IsColorSetted ? fadeDuration : 0f, true, true);

			IsColorSetted = true;
		}
	}
}