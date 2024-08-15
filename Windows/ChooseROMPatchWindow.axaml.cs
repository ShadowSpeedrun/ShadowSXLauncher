using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShadowSXLauncher.Windows;

public partial class ChooseROMPatchWindow : Window
{
    public ChooseROMPatchWindow()
    {
        InitializeComponent();
        List<string> options = new List<string>
        {
            "1.0",
            "1.1"
        };
        PatchComboBox.DataContext = options;
    }
}