using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.ViewModels;

namespace ShadowSXLauncher.Views;

public partial class NumericSlider : UserControl
{
    private NumericSliderViewModel viewModel
    {
        get { return DataContext as NumericSliderViewModel; }
    }
    
    public NumericSlider()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        DataContext = new NumericSliderViewModel();
    }
}