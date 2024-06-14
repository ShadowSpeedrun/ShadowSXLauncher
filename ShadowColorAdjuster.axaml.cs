using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ColorPicker = ShadowSXLauncher.UserControls.ColorPicker;

namespace ShadowSXLauncher.Views;

public partial class ShadowColorAdjuster : Window
{
    private ColorPicker mainColorPicker => GetColorPicker("MainColorPicker");
    private ColorPicker accentColorPicker => GetColorPicker("AccentColorPicker");
    
    private ColorPicker GetColorPicker(string pickerName)
    {
        var colorPicker = this.FindControl<ColorPicker>(pickerName);
        if (colorPicker == null)
        {
            throw new NotImplementedException();
        }

        return colorPicker;
    }
    
    public ShadowColorAdjuster()
    {
        InitializeComponent();
        SetupDefaults();
    }
    
    private void SetupDefaults()
    {
        mainColorPicker.PickerLabel.Text = "Main Color";
        mainColorPicker.SetRGBColor(Color.FromRgb(49,28,16)); //0x311C10
        accentColorPicker.PickerLabel.Text = "Accent Color";
        accentColorPicker.SetRGBColor(Color.FromRgb(222,0,0)); //0xDE0000
    }
}