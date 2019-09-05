using System.Reflection;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// http://forum.unity3d.com/threads/change-the-value-of-a-toggle-without-triggering-onvaluechanged.275056/#post-2348336
///
/// Problem:
///     When setting a Unity UI Toggle field isOn, it automatically fires the onchanged event.
///
/// This class allows you to set the Toggle, Slider, Scrollbar and Dropdown's value without invoking the onchanged event.
/// It mostly does this by invoking the private method ('Set(value, sendCallback)') contained in some of the Unity UI elements
/// </summary>
public static class UISetExtensions
{
    private static readonly MethodInfo toggleSetMethod;

    static UISetExtensions()
    {
        // Find the Toggle's set method
        toggleSetMethod = FindSetMethod(typeof(Toggle));
    }

    public static void Set(this Toggle instance, bool value, bool sendCallback = false)
    {
        toggleSetMethod.Invoke(instance, new object[] { value, sendCallback });
    }

    private static MethodInfo FindSetMethod(System.Type objectType)
    {
        var methods = objectType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        for (var i = 0; i < methods.Length; i++)
        {
            if (methods[i].Name == "Set" && methods[i].GetParameters().Length == 2)
            {
                return methods[i];
            }
        }
        return null;
    }

    static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();
    public static void SetValue(this Toggle instance, bool value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyToggleEvent;
        instance.isOn = value;
        instance.onValueChanged = originalEvent;
    }

    static InputField.OnChangeEvent emptyInputFieldEvent = new InputField.OnChangeEvent();
    public static void SetValue(this InputField instance, string value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyInputFieldEvent;
        instance.text = value;
        instance.onValueChanged = originalEvent;
    }

    static TMP_InputField.OnChangeEvent emptyTMP_InputFieldEvent = new TMP_InputField.OnChangeEvent();
    public static void SetValue(this TMP_InputField instance, string value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyTMP_InputFieldEvent;
        instance.text = value;
        instance.onValueChanged = originalEvent;
    }

    static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
    public static void SetValue(this Slider instance, float value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptySliderEvent;
        instance.value = value;
        instance.onValueChanged = originalEvent;
    }

    static Dropdown.DropdownEvent emptyDropdownFieldEvent = new Dropdown.DropdownEvent();
    public static void SetValue(this Dropdown instance, int value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyDropdownFieldEvent;
        instance.value = value;
        instance.onValueChanged = originalEvent;
    }
}