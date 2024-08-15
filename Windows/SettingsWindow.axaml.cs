using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        
#if DEBUG
        this.AttachDevTools();
#endif
        
        Configuration.Instance.LoadSettings();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        CustomShadowColorButton.Click += OnCustomShadowColorButtonPressed;
    }

    private void OnCustomShadowColorButtonPressed(object? sender, RoutedEventArgs e)
    {
        var shadowColorDialog = new ShadowColorAdjuster();
        shadowColorDialog.ShowDialog(this);
    }
}