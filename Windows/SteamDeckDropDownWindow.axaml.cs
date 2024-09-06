using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace ShadowSXLauncher.Windows;

public partial class SteamDeckDropDownWindow : Window
{
    private List<Button> optionButtons = new List<Button>();

    public SteamDeckDropDownWindow() : this(new List<string>())
    {
        
    }
    public SteamDeckDropDownWindow(List<string> options)
    {
        InitializeComponent();
        MakeOptionButtons(options);
        RegisterEvents();
    }

    private void MakeOptionButtons(List<string> options)
    {
        foreach (var option in options)
        {
            var button = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            button.Content = option;
            optionButtons.Add(button);
        }

        foreach (var optionButton in optionButtons)
        {
            OptionsStackPanel.Children.Insert(OptionsStackPanel.Children.Count-1, optionButton);
        }
    }

    private void RegisterEvents()
    {
        CloseButton.Click += (sender, args) => { Close(); };
        for (var i = 0; i < optionButtons.Count; i++)
        {
            var button = optionButtons[i];
            button.Click += (sender, args) => { Close(optionButtons.FindIndex(b => b.Content.Equals(button.Content))); };
        }
    }
}