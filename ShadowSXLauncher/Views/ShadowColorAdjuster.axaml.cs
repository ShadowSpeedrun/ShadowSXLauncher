using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShadowSXLauncher.Views;

public partial class ShadowColorAdjuster : Window
{
    public ShadowColorAdjuster()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}