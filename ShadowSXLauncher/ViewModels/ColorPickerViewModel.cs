using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace ShadowSXLauncher.ViewModels;

public class ColorPickerViewModel : INotifyPropertyChanged
{
    private SolidColorBrush pickedColor = new SolidColorBrush(Colors.White);
    public SolidColorBrush PickedColor
    {
        get
        {
            return pickedColor;
        }
        set
        {
            pickedColor = value;
            OnPropertyChanged();
            OnPropertyChanged("PickedColorHexString");
        }
    }

    public string PickedColorHexString
    {
        get
        {
            return pickedColor.Color.R.ToString("X")
                    + pickedColor.Color.G.ToString("X")
                    + pickedColor.Color.B.ToString("X");
        }
        // set
        // {
        //     byte r = byte.Parse((value[0] + value[1]).ToString());
        //     byte g = byte.Parse((value[2] + value[3]).ToString());
        //     byte b = byte.Parse((value[4] + value[5]).ToString());
        //     pickedColor.Color = Color.FromRgb(r, g, b);
        //     OnPropertyChanged();
        // }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}