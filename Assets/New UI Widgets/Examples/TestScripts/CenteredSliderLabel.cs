namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// CenteredSlider with label.
	/// </summary>
	[RequireComponent(typeof(CenteredSlider))]
	public class CenteredSliderLabel : MonoBehaviour
	{
		/// <summary>
		/// Label to display slider value.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("label")]
		protected Text Label;

		/// <summary>
		/// Current slider.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		protected CenteredSlider Slider;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init and adds listeners.
		/// </summary>
		protected virtual void Init()
		{
			Slider = GetComponent<CenteredSlider>();
			if (Slider != null)
			{
				Slider.OnValuesChange.AddListener(ValueChanged);
				ValueChanged(Slider.Value);
			}
		}

		/// <summary>
		/// Callback when slider value changed.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void ValueChanged(int value)
		{
			Label.text = value.ToString();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Slider != null)
			{
				Slider.OnValuesChange.RemoveListener(ValueChanged);
			}
		}
	}
}