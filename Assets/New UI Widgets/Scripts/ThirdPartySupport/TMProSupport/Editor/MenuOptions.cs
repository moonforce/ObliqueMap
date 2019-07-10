#if UIWIDGETS_TMPRO_SUPPORT_OBSOLETE && UNITY_EDITOR
namespace UIWidgets.TMProSupport
{
	using UnityEditor;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class MenuOptions
	{
#region Collections

		/// <summary>
		/// Create Combobox.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/Combobox", false, 1000)]
		public static void CreateCombobox()
		{
			Utilites.CreateWidgetFromAsset("ComboboxTMPro");
		}

		/// <summary>
		/// Create ComboboxIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ComboboxIcons", false, 1010)]
		public static void CreateComboboxIcons()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIconsTMPro");
		}

		/// <summary>
		/// Create ComboboxIconsMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ComboboxIconsMultiselect", false, 1020)]
		public static void CreateComboboxIconsMultiselect()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIconsMultiselectTMPro");
		}

		/// <summary>
		/// Create DirectoryTreeView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/DirectoryTreeView", false, 1030)]
		public static void CreateDirectoryTreeView()
		{
			Utilites.CreateWidgetFromAsset("DirectoryTreeViewTMPro");
		}

		/// <summary>
		/// Create FileListView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/FileListView", false, 1040)]
		public static void CreateFileListView()
		{
			Utilites.CreateWidgetFromAsset("FileListViewTMPro");
		}

		/// <summary>
		/// Create ListView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ListView", false, 1050)]
		public static void CreateListView()
		{
			Utilites.CreateWidgetFromAsset("ListViewTMPro");
		}

		/// <summary>
		/// Create istViewColors.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ListViewColors", false, 1055)]
		public static void CreateListViewColors()
		{
			Utilites.CreateWidgetFromAsset("ListViewColors");
		}

		/// <summary>
		/// Create ListViewInt.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ListViewInt", false, 1060)]
		public static void CreateListViewInt()
		{
			Utilites.CreateWidgetFromAsset("ListViewIntTMPro");
		}

		/// <summary>
		/// Create ListViewHeight.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ListViewHeight", false, 1070)]
		public static void CreateListViewHeight()
		{
			Utilites.CreateWidgetFromAsset("ListViewHeightTMPro");
		}

		/// <summary>
		/// Create ListViewIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ListViewIcons", false, 1090)]
		public static void CreateListViewIcons()
		{
			Utilites.CreateWidgetFromAsset("ListViewIconsTMPro");
		}

		/// <summary>
		/// Create ListViewPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/ListViewPaginator", false, 1100)]
		public static void CreateListViewPaginator()
		{
			Utilites.CreateWidgetFromAsset("ListViewPaginatorTMPro");
		}

		/// <summary>
		/// Create TreeView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Collections/TreeView", false, 1110)]
		public static void CreateTreeView()
		{
			Utilites.CreateWidgetFromAsset("TreeViewTMPro");
		}
#endregion

#region Containers

		/// <summary>
		/// Create Accordion.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Containers/Accordion", false, 2000)]
		public static void CreateAccordion()
		{
			Utilites.CreateWidgetFromAsset("AccordionTMPro");
		}

		/// <summary>
		/// Create Tabs.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Containers/Tabs", false, 2010)]
		public static void CreateTabs()
		{
			Utilites.CreateWidgetFromAsset("TabsTMPro");
		}

		/// <summary>
		/// Create TabsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Containers/TabsLeft", false, 2020)]
		public static void CreateTabsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsLeftTMPro");
		}

		/// <summary>
		/// Create TabsIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Containers/TabsIcons", false, 2030)]
		public static void CreateTabsIcons()
		{
			Utilites.CreateWidgetFromAsset("TabsIconsTMPro");
		}

		/// <summary>
		/// Create TabsIconsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Containers/TabsIconsLeft", false, 2040)]
		public static void CreateTabsIconsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsIconsLeftTMPro");
		}
#endregion

#region Dialogs

		/// <summary>
		/// Create DatePicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/DatePicker", false, 3000)]
		public static void CreateDatePicker()
		{
			Utilites.CreateWidgetFromAsset("DatePickerTMPro");
		}

		/// <summary>
		/// Create DateTimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/DateTimePicker", false, 3005)]
		public static void CreateDateTimePicker()
		{
			Utilites.CreateWidgetFromAsset("DateTimePickerTMPro");
		}

		/// <summary>
		/// Create Dialog.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/Dialog Template", false, 3010)]
		public static void CreateDialog()
		{
			Utilites.CreateWidgetFromAsset("DialogTemplateTMPro");
		}

		/// <summary>
		/// Create FileDialog.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/FileDialog", false, 3020)]
		public static void CreateFileDialog()
		{
			Utilites.CreateWidgetFromAsset("FileDialogTMPro");
		}

		/// <summary>
		/// Create FolderDialog.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/FolderDialog", false, 3030)]
		public static void CreateFolderDialog()
		{
			Utilites.CreateWidgetFromAsset("FolderDialogTMPro");
		}

		/// <summary>
		/// Create Notify.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/Notify Template", false, 3040)]
		public static void CreateNotify()
		{
			Utilites.CreateWidgetFromAsset("NotifyTemplateTMPro");
		}

		/// <summary>
		/// Create PickerBool.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/PickerBool", false, 3050)]
		public static void CreatePickerBool()
		{
			Utilites.CreateWidgetFromAsset("PickerBoolTMPro");
		}

		/// <summary>
		/// Create PickerIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/PickerIcons", false, 3060)]
		public static void CreatePickerIcons()
		{
			Utilites.CreateWidgetFromAsset("PickerIconsTMPro");
		}

		/// <summary>
		/// Create PickerInt.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/PickerInt", false, 3070)]
		public static void CreatePickerInt()
		{
			Utilites.CreateWidgetFromAsset("PickerIntTMPro");
		}

		/// <summary>
		/// Create PickerString.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/PickerString", false, 3080)]
		public static void CreatePickerString()
		{
			Utilites.CreateWidgetFromAsset("PickerStringTMPro");
		}

		/// <summary>
		/// Create Popup.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/Popup", false, 3090)]
		public static void CreatePopup()
		{
			Utilites.CreateWidgetFromAsset("PopupTMPro");
		}

		/// <summary>
		/// Create TimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Dialogs/TimePicker", false, 3100)]
		public static void CreateTimePicker()
		{
			Utilites.CreateWidgetFromAsset("TimePickerTMPro");
		}
		#endregion

		#region Input

		/// <summary>
		/// Create Autocomplete.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/Autocomplete", false, 3980)]
		public static void CreateAutocomplete()
		{
			Utilites.CreateWidgetFromAsset("AutocompleteTMPro");
		}

		/// <summary>
		/// Create AutocompleteIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/AutocompleteIcons", false, 3990)]
		public static void CreateAutocompleteIcons()
		{
			Utilites.CreateWidgetFromAsset("AutocompleteIconsTMPro");
		}

		/// <summary>
		/// Create ButtonBig.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/ButtonBig", false, 4000)]
		public static void CreateButtonBig()
		{
			Utilites.CreateWidgetFromAsset("ButtonBigTMPro");
		}

		/// <summary>
		/// Create ButtonSmall.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/ButtonSmall", false, 4010)]
		public static void CreateButtonSmall()
		{
			Utilites.CreateWidgetFromAsset("ButtonSmallTMPro");
		}

		/// <summary>
		/// Create Calendar.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/Calendar", false, 4020)]
		public static void CreateCalendar()
		{
			Utilites.CreateWidgetFromAsset("CalendarTMPro");
		}

		/// <summary>
		/// Create CenteredSlider.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/CenteredSlider", false, 4030)]
		public static void CreateCenteredSlider()
		{
			Utilites.CreateWidgetFromAsset("CenteredSlider");
		}

		/// <summary>
		/// Create CenteredSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/CenteredSliderVertical", false, 4040)]
		public static void CreateCenteredSliderVertical()
		{
			Utilites.CreateWidgetFromAsset("CenteredSliderVertical");
		}

		/// <summary>
		/// Create ColorPicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/ColorPicker", false, 4050)]
		public static void CreateColorPicker()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerTMPro");
		}

		/// <summary>
		/// Create ColorPickerRange.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/ColorPickerRange", false, 4060)]
		public static void CreateColorPickerRange()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerRange");
		}

		/// <summary>
		/// Create ColorPickerRangeHSV.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/ColorPickerRangeHSV", false, 4063)]
		public static void CreateColorPickerRangeHSV()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerRangeHSV");
		}

		/// <summary>
		/// Create ColorsList.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/ColorsList", false, 4065)]
		public static void CreateColorsList()
		{
			Utilites.CreateWidgetFromAsset("ColorsList");
		}

		/// <summary>
		/// Create DateTime.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/DateTime", false, 4067)]
		public static void CreateDateTime()
		{
			Utilites.CreateWidgetFromAsset("DateTimeTMPro");
		}

		/// <summary>
		/// Create RangeSlider.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/RangeSlider", false, 4070)]
		public static void CreateRangeSlider()
		{
			Utilites.CreateWidgetFromAsset("RangeSlider");
		}

		/// <summary>
		/// Create RangeSliderFloat.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/RangeSliderFloat", false, 4080)]
		public static void CreateRangeSliderFloat()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloat");
		}

		/// <summary>
		/// Create RangeSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/RangeSliderVertical", false, 4090)]
		public static void CreateRangeSliderVertical()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderVertical");
		}

		/// <summary>
		/// Create RangeSliderFloatVertical.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/RangeSliderFloatVertical", false, 4100)]
		public static void CreateRangeSliderFloatVertical()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloatVertical");
		}

		/// <summary>
		/// Create Spinner.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/Spinner", false, 4110)]
		public static void CreateSpinner()
		{
			Utilites.CreateWidgetFromAsset("SpinnerTMPro");
		}

		/// <summary>
		/// Create SpinnerFloat.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/SpinnerFloat", false, 4120)]
		public static void CreateSpinnerFloat()
		{
			Utilites.CreateWidgetFromAsset("SpinnerFloatTMPro");
		}

		/// <summary>
		/// Create Switch.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/Switch", false, 4130)]
		public static void CreateSwitch()
		{
			Utilites.CreateWidgetFromAsset("Switch");
		}

		/// <summary>
		/// Create Time12.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/Time12", false, 4140)]
		public static void CreateTime12()
		{
			Utilites.CreateWidgetFromAsset("Time12TMPro");
		}

		/// <summary>
		/// Create Time24.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Input/Time24", false, 4150)]
		public static void CreateTime24()
		{
			Utilites.CreateWidgetFromAsset("Time24TMPro");
		}
		#endregion

		/// <summary>
		/// Create AudioPlayer.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Misc/AudioPlayer", false, 5000)]
		public static void CreateAudioPlayer()
		{
			Utilites.CreateWidgetFromAsset("AudioPlayer");
		}

		/// <summary>
		/// Create Progressbar.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Misc/(obsolete) Progressbar", false, 5010)]
		public static void CreateProgressbar()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarTMPro");
		}

		/// <summary>
		/// Create ProgressbarDeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Misc/ProgressbarDeterminate", false, 5014)]
		public static void CreateProgressbarDeterminate()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarDeterminateTMPro");
		}

		/// <summary>
		/// Create ProgressbarIndeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Misc/ProgressbarIndeterminate", false, 5017)]
		public static void CreateProgressbarIndeterminate()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarIndeterminate");
		}

		/// <summary>
		/// Create ScrollRectPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Misc/ScrollRectPaginator", false, 5020)]
		public static void CreateScrollRectPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectPaginator");
		}

		/// <summary>
		/// Create ScrollRectNumericPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets with TextMesh Pro/Misc/ScrollRectNumericPaginator", false, 5030)]
		public static void CreateScrollRectNumericPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectNumericPaginatorTMPro");
		}
	}
}
#endif