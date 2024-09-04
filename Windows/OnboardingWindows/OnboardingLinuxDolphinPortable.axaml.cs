using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingLinuxDolphinPortable : OnboardingWindow
{
    public OnboardingLinuxDolphinPortable() : base()
    {
        InitializeComponent();
        RegisterEvents();
        
        if (!CommonUtils.isDolphinPortable())
        {
            SetPortableStackPanel.IsVisible = true;
            PortableFoundTextBlock.IsVisible = false;
        }
        else
        {
            SetPortableStackPanel.IsVisible = false;
            PortableFoundTextBlock.IsVisible = true;
            Width = 300; 
            PortableCheckBox.IsChecked = true;
        }
    }

    private void RegisterEvents()
    {
        ContinueButton.Click += ContinueButtonOnClick;
        BackButton.Click += (sender, args) => { SetOnboardingPage(1); };
    }

    private void ContinueButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if ((bool)PortableCheckBox.IsChecked!)
        {
            CreatePortableFile();
            Configuration.Instance.DolphinUserLocation = Path.Combine(Configuration.Instance.DolphinBinLocation, "user");
        }
        else
        {
            //Assume Global File path
            Configuration.Instance.DolphinUserLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.local/share/dolphin-emu/";
        }
        SetOnboardingPage(3);
    }

    private void CreatePortableFile()
    {
        //Create the portable.txt file in the bin location.
        var portableFilePath = Path.Combine(CommonFilePaths.DolphinBinPath, "portable.txt");
        if (!CommonUtils.isDolphinPortable())
        {
            File.Create(portableFilePath);
        }
    }
}