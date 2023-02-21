namespace UIWidgets.Examples
{
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// LVInputFields component.
	/// </summary>
	public class LVInputFieldsComponent : ListViewItem, IViewData<LVInputFieldsItem>
	{
		/// <summary>
		/// Input1.
		/// </summary>
		[SerializeField]
		public InputField Input1;

		/// <summary>
		/// Input2.
		/// </summary>
		[SerializeField]
		public InputField Input2;

		/// <summary>
		/// Toggle.
		/// </summary>
		[SerializeField]
		public Toggle Toggle;

		LVInputFieldsItem currentItem;

		/// <summary>
		/// Current item.
		/// </summary>
		public LVInputFieldsItem Item
		{
			get
			{
				return currentItem;
			}

			set
			{
				if (currentItem != null)
				{
					// unsubscribe
					currentItem.PropertyChanged -= ItemPropertyChanged;
				}

				currentItem = value;

				if (currentItem != null)
				{
					// subscribe to event
					// when item properties changed ItemPropertyChanged will called
					currentItem.PropertyChanged += ItemPropertyChanged;

					// and update InputFields values
					UpdateInputFields();
				}
			}
		}

		void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// item is changed, so InputFields values will be changed
			UpdateInputFields();
		}

		void UpdateInputFields()
		{
			Input1.text = Item.Text1;
			Input2.text = Item.Text2;
			Toggle.isOn = Item.IsOn;
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(LVInputFieldsItem item)
		{
			Item = item;
		}

		/// <summary>
		/// Handle Input1.OnEndEdit event.
		/// Attached in Inspector window to InputField1.EndEdit.
		/// </summary>
		public void Text1Changed()
		{
			Item.Text1 = Input1.text;
		}

		/// <summary>
		/// Handle Input2.OnEndEdit event.
		/// Attached in Inspector window to InputField2.EndEdit.
		/// </summary>
		public void Text2Changed()
		{
			Item.Text2 = Input2.text;
		}

		/// <summary>
		/// Handle Toggle.OnValueChanged event.
		/// Attached in Inspector window to Toggle.OnValueChanged.
		/// </summary>
		public void IsOnChanged()
		{
			Item.IsOn = Toggle.isOn;
		}

		/// <summary>
		/// Reset current item.
		/// </summary>
		public override void MovedToCache()
		{
			Item = null;
		}
	}
}