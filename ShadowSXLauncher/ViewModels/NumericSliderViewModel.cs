using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShadowSXLauncher.ViewModels;

public class NumericSliderViewModel : INotifyPropertyChanged
{
    private string label = "R:";

    public string Label
    {
        get { return label; }
        set
        {
            label = value + ":";
            OnPropertyChanged();
        }
    }
    
    private byte value = 100;

    public double Value
    {
        get { return value; }
        set
        {
            this.value = (byte)value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}