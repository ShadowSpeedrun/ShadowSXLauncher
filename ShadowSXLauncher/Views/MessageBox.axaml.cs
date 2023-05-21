using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.ViewModels;
using System.Linq;
using Avalonia.Interactivity;

namespace ShadowSXLauncher.Views;

public partial class MessageBox : Window
{
    private MessageBoxViewModel viewModel
    {
        get { return DataContext as MessageBoxViewModel; }
    }
    
    public MessageBox()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        RegisterEvents();
    }
    
    private void RegisterEvents()
    {
        this.FindControl<Button>("Button1").Click += ButtonClicked;
    }

    private void ButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (sender != null && ((Button)sender) != null)
        {
            var button = ((Button)sender);

            switch (button.Name)
            {
                case "Button1":
                case "Button2_1": 
                case "Button3_1":
                    if (Button1Clicked != null)
                    {
                        Button1Clicked.Invoke(sender, e);
                    }
                    break;
                case "Button2_2": 
                case "Button3_2":
                    if (Button2Clicked != null)
                    {
                        Button2Clicked.Invoke(sender, e);
                    }
                    break;
                case "Button3_3":
                    if (Button3Clicked != null)
                    {
                        Button3Clicked.Invoke(sender, e);
                    }
                    break;
            }

            Close();
        }
    }

    public MessageBox(string title, string message, string?[] options):this()
    {
        viewModel.Title = title;
        viewModel.MessageText = message;
        
        viewModel.ButtonCount = options.Count(o => !string.IsNullOrEmpty(o));
        viewModel.Button1Text = (options.Length >= 1 && !string.IsNullOrEmpty(options[0])) ? options[0] : "";
        viewModel.Button2Text = (options.Length >= 2 && !string.IsNullOrEmpty(options[1])) ? options[1] : "";
        viewModel.Button3Text = (options.Length >= 3 && !string.IsNullOrEmpty(options[2])) ? options[2] : "";
    }

    public Action<object?, EventArgs> Button1Clicked;
    public Action<object?, EventArgs> Button2Clicked;
    public Action<object?, EventArgs> Button3Clicked;

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        DataContext = new MessageBoxViewModel();
    }
}