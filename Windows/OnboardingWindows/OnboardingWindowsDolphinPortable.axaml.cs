using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingWindowsDolphinPortable : OnboardingWindow
{
    public OnboardingWindowsDolphinPortable() : base()
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
            Configuration.Instance.DolphinUserLocation =
                Path.Combine(Configuration.Instance.DolphinBinLocation, "User");
        }
    }

    private void RegisterEvents()
    {
        ContinueButton.Click += (sender, args) =>
        {
            if ((bool)PortableCheckBox.IsChecked!)
            {
                CreatePortableFile();
                Configuration.Instance.DolphinUserLocation = Path.Combine(Configuration.Instance.DolphinBinLocation, "User");
            }
            else
            {
                //Assume Global File path
                Configuration.Instance.DolphinUserLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator");
            }
            SetOnboardingPage(3);
        };
        BackButton.Click += (sender, args) => { SetOnboardingPage(1); };
    }

    private void CreatePortableFile()
    {
        //Create the portable.txt file in the bin location.
        File.Create(Path.Combine(CommonFilePaths.DolphinBinPath, "portable.txt"));
    }
}