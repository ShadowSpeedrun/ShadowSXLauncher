using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace ShadowSXLauncher.ViewModels;

public class MessageBoxViewModel : INotifyPropertyChanged
{
    private string title = "Title";
    public string Title
    {
        get
        {
            return title;
        }
        set
        {
            title = value;
            OnPropertyChanged();
        }
    }

    private string messageText = "Message";
    public string MessageText
    {
        get
        {
            return messageText;
        }
        set
        {
            messageText = value;
            OnPropertyChanged();
        }
    }
    
    private int buttonCount = 1;

    public int ButtonCount
    {
        get { return buttonCount; }
        set
        {
            buttonCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Is1Button));
            OnPropertyChanged(nameof(Is2Button));
            OnPropertyChanged(nameof(Is3Button));
        }
    }

    public bool Is1Button => buttonCount == 1;
    public bool Is2Button => buttonCount == 2;
    public bool Is3Button => buttonCount == 3;

    private string button1Text = "1";

    public string Button1Text
    {
        get
        {
            return button1Text;
        }
        set
        {
            button1Text = value;
            OnPropertyChanged();
        }
    }
    
    private string button2Text = "2";

    public string Button2Text
    {
        get
        {
            return button2Text;
        }
        set
        {
            button2Text = value;
            OnPropertyChanged();
        }
    }
    
    private string button3Text = "3";

    public string Button3Text
    {
        get
        {
            return button3Text;
        }
        set
        {
            button3Text = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}