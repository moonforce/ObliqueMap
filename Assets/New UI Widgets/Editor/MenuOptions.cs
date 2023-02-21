#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class MenuOptions
	{
		/// <summary>
		/// Suffix to support different prefabs type.
		/// Made public for testing purpose.
		/// </summary>
		#if UIWIDGETS_TMPRO_SUPPORT
		public static string Suffix = "TMPro";
		#else
		public static string Suffix = "";
		#endif

		#region Collections

		/// <summary>
		/// Create Combobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/Combobox", false, 1000)]
		public static void CreateCombobox()
		{
			Utilites.CreateWidgetFromAsset("Combobox" + Suffix);
		}

		/// <summary>
		/// Create ComboboxIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxIcons", false, 1010)]
		public static void CreateComboboxIcons()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIcons" + Suffix);
		}

		/// <summary>
		/// Create ComboboxIconsMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxIconsMultiselect", false, 1020)]
		public static void CreateComboboxIconsMultiselect()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIconsMultiselect" + Suffix);
		}

		/// <summary>
		/// Create DirectoryTreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/DirectoryTreeView", false, 1030)]
		public static void CreateDirectoryTreeView()
		{
			Utilites.CreateWidgetFromAsset("DirectoryTreeView" + Suffix);
		}

		/// <summary>
		/// Create FileListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/FileListView", false, 1040)]
		public static void CreateFileListView()
		{
			Utilites.CreateWidgetFromAsset("FileListView" + Suffix);
		}

		/// <summary>
		/// Create ListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListView", false, 1050)]
		public static void CreateListView()
		{
			Utilites.CreateWidgetFromAsset("ListView" + Suffix);
		}

		/// <summary>
		/// Create istViewColors.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewColors", false, 1055)]
		public static void CreateListViewColors()
		{
			Utilites.CreateWidgetFromAsset("ListViewColors");
		}

		/// <summary>
		/// Create ListViewInt.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewInt", false, 1060)]
		public static void CreateListViewInt()
		{
			Utilites.CreateWidgetFromAsset("ListViewInt" + Suffix);
		}

		/// <summary>
		/// Create ListViewHeight.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewHeight", false, 1070)]
		public static void CreateListViewHeight()
		{
			Utilites.CreateWidgetFromAsset("ListViewHeight" + Suffix);
		}

		/// <summary>
		/// Create ListViewIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewIcons", false, 1090)]
		public static void CreateListViewIcons()
		{
			Utilites.CreateWidgetFromAsset("ListViewIcons" + Suffix);
		}

		/// <summary>
		/// Create ListViewPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewPaginator", false, 1100)]
		public static void CreateListViewPaginator()
		{
			Utilites.CreateWidgetFromAsset("ListViewPaginator" + Suffix);
		}

		/// <summary>
		/// Create TreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/TreeView", false, 1110)]
		public static void CreateTreeView()
		{
			Utilites.CreateWidgetFromAsset("TreeView" + Suffix);
		}
		#endregion

		#region Containers

		/// <summary>
		/// Create Accordion.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/Accordion", false, 2000)]
		public static void CreateAccordion()
		{
			Utilites.CreateWidgetFromAsset("Accordion" + Suffix);
		}

		/// <summary>
		/// Create Tabs.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/Tabs", false, 2010)]
		public static void CreateTabs()
		{
			Utilites.CreateWidgetFromAsset("Tabs" + Suffix);
		}

		/// <summary>
		/// Create TabsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsLeft", false, 2020)]
		public static void CreateTabsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsLeft" + Suffix);
		}

		/// <summary>
		/// Create TabsIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsIcons", false, 2030)]
		public static void CreateTabsIcons()
		{
			Utilites.CreateWidgetFromAsset("TabsIcons" + Suffix);
		}

		/// <summary>
		/// Create TabsIconsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsIconsLeft", false, 2040)]
		public static void CreateTabsIconsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsIconsLeft" + Suffix);
		}
		#endregion

		#region Dialogs

		/// <summary>
		/// Create DatePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/DatePicker", false, 3000)]
		public static void CreateDatePicker()
		{
			Utilites.CreateWidgetFromAsset("DatePicker" + Suffix);
		}

		/// <summary>
		/// Create DateTimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/DateTimePicker", false, 3005)]
		public static void CreateDateTimePicker()
		{
			Utilites.CreateWidgetFromAsset("DateTimePicker" + Suffix);
		}

		/// <summary>
		/// Create Dialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/Dialog Template", false, 3010)]
		public static void CreateDialog()
		{
			Utilites.CreateWidgetFromAsset("DialogTemplate" + Suffix);
		}

		/// <summary>
		/// Create FileDialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/FileDialog", false, 3020)]
		public static void CreateFileDialog()
		{
			Utilites.CreateWidgetFromAsset("FileDialog" + Suffix);
		}

		/// <summary>
		/// Create FolderDialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/FolderDialog", false, 3030)]
		public static void CreateFolderDialog()
		{
			Utilites.CreateWidgetFromAsset("FolderDialog" + Suffix);
		}

		/// <summary>
		/// Create Notify.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/Notify Template", false, 3040)]
		public static void CreateNotify()
		{
			Utilites.CreateWidgetFromAsset("NotifyTemplate" + Suffix);
		}

		/// <summary>
		/// Create PickerBool.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerBool", false, 3050)]
		public static void CreatePickerBool()
		{
			Utilites.CreateWidgetFromAsset("PickerBool" + Suffix);
		}

		/// <summary>
		/// Create PickerIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerIcons", false, 3060)]
		public static void CreatePickerIcons()
		{
			Utilites.CreateWidgetFromAsset("PickerIcons" + Suffix);
		}

		/// <summary>
		/// Create PickerInt.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerInt", false, 3070)]
		public static void CreatePickerInt()
		{
			Utilites.CreateWidgetFromAsset("PickerInt" + Suffix);
		}

		/// <summary>
		/// Create PickerString.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerString", false, 3080)]
		public static void CreatePickerString()
		{
			Utilites.CreateWidgetFromAsset("PickerString" + Suffix);
		}

		/// <summary>
		/// Create Popup.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/Popup", false, 3090)]
		public static void CreatePopup()
		{
			Utilites.CreateWidgetFromAsset("Popup" + Suffix);
		}

		/// <summary>
		/// Create TimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/TimePicker", false, 3100)]
		public static void CreateTimePicker()
		{
			Utilites.CreateWidgetFromAsset("TimePicker" + Suffix);
		}
		#endregion

		#region Input

		/// <summary>
		/// Create Autocomplete.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Autocomplete", false, 3980)]
		public static void CreateAutocomplete()
		{
			Utilites.CreateWidgetFromAsset("Autocomplete" + Suffix);
		}

		/// <summary>
		/// Create AutocompleteIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/AutocompleteIcons", false, 3990)]
		public static void CreateAutocompleteIcons()
		{
			Utilites.CreateWidgetFromAsset("AutocompleteIcons" + Suffix);
		}

		/// <summary>
		/// Create ButtonBig.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ButtonBig", false, 4000)]
		public static void CreateButtonBig()
		{
			Utilites.CreateWidgetFromAsset("ButtonBig" + Suffix);
		}

		/// <summary>
		/// Create ButtonSmall.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ButtonSmall", false, 4010)]
		public static void CreateButtonSmall()
		{
			Utilites.CreateWidgetFromAsset("ButtonSmall" + Suffix);
		}

		/// <summary>
		/// Create Calendar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Calendar", false, 4020)]
		public static void CreateCalendar()
		{
			Utilites.CreateWidgetFromAsset("Calendar" + Suffix);
		}

		/// <summary>
		/// Create CenteredSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CenteredSlider", false, 4030)]
		public static void CreateCenteredSlider()
		{
			Utilites.CreateWidgetFromAsset("CenteredSlider");
		}

		/// <summary>
		/// Create CenteredSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CenteredSliderVertical", false, 4040)]
		public static void CreateCenteredSliderVertical()
		{
			Utilites.CreateWidgetFromAsset("CenteredSliderVertical");
		}

		/// <summary>
		/// Create ColorPicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPicker", false, 4050)]
		public static void CreateColorPicker()
		{
			Utilites.CreateWidgetFromAsset("ColorPicker" + Suffix);
		}

		/// <summary>
		/// Create ColorPickerRange.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPickerRange", false, 4060)]
		public static void CreateColorPickerRange()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerRange");
		}

		/// <summary>
		/// Create ColorPickerRangeHSV.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPickerRangeHSV", false, 4063)]
		public static void CreateColorPickerRangeHSV()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerRangeHSV");
		}

		/// <summary>
		/// Create ColorsList.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorsList", false, 4065)]
		public static void CreateColorsList()
		{
			Utilites.CreateWidgetFromAsset("ColorsList");
		}

		/// <summary>
		/// Create DateTime.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateTime", false, 4067)]
		public static void CreateDateTime()
		{
			Utilites.CreateWidgetFromAsset("DateTime" + Suffix);
		}

		/// <summary>
		/// Create RangeSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSlider", false, 4070)]
		public static void CreateRangeSlider()
		{
			Utilites.CreateWidgetFromAsset("RangeSlider");
		}

		/// <summary>
		/// Create RangeSliderFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderFloat", false, 4080)]
		public static void CreateRangeSliderFloat()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloat");
		}

		/// <summary>
		/// Create RangeSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderVertical", false, 4090)]
		public static void CreateRangeSliderVertical()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderVertical");
		}

		/// <summary>
		/// Create RangeSliderFloatVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderFloatVertical", false, 4100)]
		public static void CreateRangeSliderFloatVertical()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloatVertical");
		}

		/// <summary>
		/// Create Spinner.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Spinner", false, 4110)]
		public static void CreateSpinner()
		{
			Utilites.CreateWidgetFromAsset("Spinner" + Suffix);
		}

		/// <summary>
		/// Create SpinnerFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/SpinnerFloat", false, 4120)]
		public static void CreateSpinnerFloat()
		{
			Utilites.CreateWidgetFromAsset("SpinnerFloat" + Suffix);
		}

		/// <summary>
		/// Create Switch.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Switch", false, 4130)]
		public static void CreateSwitch()
		{
			Utilites.CreateWidgetFromAsset("Switch");
		}

		/// <summary>
		/// Create Time12.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Time12", false, 4140)]
		public static void CreateTime12()
		{
			Utilites.CreateWidgetFromAsset("Time12" + Suffix);
		}

		/// <summary>
		/// Create Time24.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Time24", false, 4150)]
		public static void CreateTime24()
		{
			Utilites.CreateWidgetFromAsset("Time24" + Suffix);
		}

		#endregion

		/// <summary>
		/// Create AudioPlayer.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Misc/AudioPlayer", false, 5000)]
		public static void CreateAudioPlayer()
		{
			Utilites.CreateWidgetFromAsset("AudioPlayer");
		}

		/// <summary>
		/// Create Progressbar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Misc/(obsolete) Progressbar", false, 5010)]
		public static void CreateProgressbar()
		{
			Utilites.CreateWidgetFromAsset("Progressbar" + Suffix);
		}

		/// <summary>
		/// Create ProgressbarDeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Misc/ProgressbarDeterminate", false, 5014)]
		public static void CreateProgressbarDeterminate()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarDeterminate" + Suffix);
		}

		/// <summary>
		/// Create ProgressbarIndeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Misc/ProgressbarIndeterminate", false, 5017)]
		public static void CreateProgressbarIndeterminate()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarIndeterminate");
		}

		/// <summary>
		/// Create ScrollRectPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Misc/ScrollRectPaginator", false, 5020)]
		public static void CreateScrollRectPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectPaginator");
		}

		/// <summary>
		/// Create ScrollRectNumericPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Misc/ScrollRectNumericPaginator", false, 5030)]
		public static void CreateScrollRectNumericPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectNumericPaginator" + Suffix);
		}

		/// <summary>
		/// Create Sidebar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Misc/Sidebar", false, 5040)]
		public static void CreateSidebar()
		{
			Utilites.CreateWidgetFromAsset("Sidebar");
		}
	}
}
#endif