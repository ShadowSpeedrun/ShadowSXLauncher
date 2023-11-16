using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.ViewModels;

namespace ShadowSXLauncher.Views;

public partial class ColorPicker : UserControl
{
    private ColorPickerViewModel viewModel
    {
        get { return DataContext as ColorPickerViewModel; }
    }
    
    public ColorPicker()
    {
        InitializeComponent();
        SetupDefaults();
    }

    public ColorPicker(byte[] color) : this()
    {
        
    }

    private void SetupDefaults()
    {
        ((NumericSliderViewModel)this.FindControl<NumericSlider>("NumSliderR").DataContext).Label = "R";
        ((NumericSliderViewModel)this.FindControl<NumericSlider>("NumSliderG").DataContext).Label = "G";
        ((NumericSliderViewModel)this.FindControl<NumericSlider>("NumSliderB").DataContext).Label = "B";
        ((NumericSliderViewModel)this.FindControl<NumericSlider>("NumSliderH").DataContext).Label = "H";
        ((NumericSliderViewModel)this.FindControl<NumericSlider>("NumSliderS").DataContext).Label = "S";
        ((NumericSliderViewModel)this.FindControl<NumericSlider>("NumSliderV").DataContext).Label = "V";
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        DataContext = new ColorPickerViewModel();
    }
}