using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace ShadowSXLauncher.UserControls;

public partial class ColorPicker : UserControl
{
    private Dictionary<string, NumericSlider> Sliders = new Dictionary<string, NumericSlider>();
    private bool changeAllowed;

    public ColorPicker()
    {
        InitializeComponent();
        SetupDefaults();
    }
    
    private void SetupDefaults()
    {
        NumSliderR.TextLabel.Text = "R";
        NumSliderR.SliderValue.Maximum = 255;
        NumSliderR.NumericUpDownValue.Maximum = 255;
        NumSliderG.TextLabel.Text = "G";
        NumSliderG.SliderValue.Maximum = 255;
        NumSliderG.NumericUpDownValue.Maximum = 255;
        NumSliderB.TextLabel.Text = "B";
        NumSliderB.SliderValue.Maximum = 255;
        NumSliderB.NumericUpDownValue.Maximum = 255;
        NumSliderH.TextLabel.Text = "H";
        NumSliderH.SliderValue.Maximum = 360;
        NumSliderH.NumericUpDownValue.Maximum = 360;
        NumSliderS.TextLabel.Text = "S";
        NumSliderS.SliderValue.Maximum = 1;
        NumSliderS.NumericUpDownValue.Maximum = 1;
        NumSliderS.NumericUpDownValue.Increment = (decimal)0.05;
        NumSliderV.TextLabel.Text = "V";
        NumSliderV.SliderValue.Maximum = 1;
        NumSliderV.NumericUpDownValue.Maximum = 1;
        NumSliderV.NumericUpDownValue.Increment = (decimal)0.05;
        SetRGBColor(Colors.White);
        UpdateColorHexString();
        
        Sliders.Add("R", NumSliderR);
        Sliders.Add("G", NumSliderG);
        Sliders.Add("B", NumSliderB);
        Sliders.Add("H", NumSliderH);
        Sliders.Add("S", NumSliderS);
        Sliders.Add("V", NumSliderV);

        NumSliderR.SliderValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorBySlider(true); };
        NumSliderR.NumericUpDownValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorByValue(true); };
        NumSliderG.SliderValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorBySlider(true); };
        NumSliderG.NumericUpDownValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorByValue(true); };
        NumSliderB.SliderValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorBySlider(true); };
        NumSliderB.NumericUpDownValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorByValue(true); };
        NumSliderH.SliderValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorBySlider(false); };
        NumSliderH.NumericUpDownValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorByValue(false); };
        NumSliderS.SliderValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorBySlider(false); };
        NumSliderS.NumericUpDownValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorByValue(false); };
        NumSliderV.SliderValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorBySlider(false); };
        NumSliderV.NumericUpDownValue.ValueChanged += (sender, args) => { if (changeAllowed) AdjustColorByValue(false); };
    }

    private void AdjustColorBySlider(bool isRGB)
    {
        if (isRGB)
        {
            SetRGBColor(Color.FromRgb((byte)Sliders["R"].SliderValue.Value, (byte)Sliders["G"].SliderValue.Value, (byte)Sliders["B"].SliderValue.Value));
        }
        else
        {
            var newColor = HsvColor.FromHsv(Sliders["H"].SliderValue.Value, Sliders["S"].SliderValue.Value, Sliders["V"].SliderValue.Value);
            SetRGBColor(newColor.ToRgb());
        }
    }
    private void AdjustColorByValue(bool isRGB)
    {
        if (isRGB)
        {
            SetRGBColor(Color.FromRgb((byte)Sliders["R"].NumericUpDownValue.Value, (byte)Sliders["G"].NumericUpDownValue.Value, (byte)Sliders["B"].NumericUpDownValue.Value));
        }
        else
        {
            var newColor = HsvColor.FromHsv((double)Sliders["H"].NumericUpDownValue.Value, (double)Sliders["S"].NumericUpDownValue.Value, (double)Sliders["V"].NumericUpDownValue.Value);
            SetRGBColor(newColor.ToRgb());
        }
    }

    private void UpdateColorHexString()
    {
        var r = (PickedColor.Background as SolidColorBrush).Color.R.ToString("X2");
        var g = (PickedColor.Background as SolidColorBrush).Color.G.ToString("X2");
        var b = (PickedColor.Background as SolidColorBrush).Color.B.ToString("X2");
        this.Get<TextBox>("PickedColorHexString").Text = (r + g + b).ToUpper();
    }

    public void SetRGBColor(Color fromRgb)
    {
        changeAllowed = false;
        PickedColor.Background = new SolidColorBrush(fromRgb);
        UpdateColorHexString();
        
        NumSliderR.SliderValue.Value = fromRgb.R;
        NumSliderR.NumericUpDownValue.Value = fromRgb.R; 
        NumSliderG.SliderValue.Value = fromRgb.G;
        NumSliderG.NumericUpDownValue.Value = fromRgb.G;
        NumSliderB.SliderValue.Value = fromRgb.B;
        NumSliderB.NumericUpDownValue.Value = fromRgb.B;

        var colorAsHSV = fromRgb.ToHsv();
        NumSliderH.SliderValue.Value = colorAsHSV.H;
        NumSliderH.NumericUpDownValue.Value = (decimal)colorAsHSV.H;
        NumSliderS.SliderValue.Value = colorAsHSV.S;
        NumSliderS.NumericUpDownValue.Value = (decimal)colorAsHSV.S;
        NumSliderV.SliderValue.Value = colorAsHSV.V;
        NumSliderV.NumericUpDownValue.Value = (decimal)colorAsHSV.V;

        changeAllowed = true;
    }
}