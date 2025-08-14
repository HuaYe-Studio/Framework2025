using System;

namespace UIFramework.Editor
{
    [Flags]
    public enum UIComponentType
    {
        Button = 1 << 0,
        Toggle = 1 << 1,
        Slider = 1 << 2,
        Text = 1 << 3,
        Image = 1 << 4,
        TMPText = 1 << 5,
        Dropdown = 1 << 6,
        InputField = 1 << 7,
        TMPDropdown = 1 << 8,
        TMPInputField = 1 << 9,
        RawImage = 1 << 10,
        ScrollView = 1 << 11,
        RectTransform = 1 << 12,
        All = ~0
    }
}