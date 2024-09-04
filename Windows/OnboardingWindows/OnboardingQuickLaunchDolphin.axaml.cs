using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingQuickLaunchDolphin : OnboardingWindow
{
    public OnboardingQuickLaunchDolphin() : base()
    {
        InitializeComponent();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        ContinueButton.Click += ContinueButtonOnClick;
        BackButton.Click += BackButtonOnClick;;
    }

    private void ContinueButtonOnClick(object? sender, RoutedEventArgs e)
    {
        CommonUtils.LaunchDolphin(true);
        SetOnboardingPage(4);
    }
    
    private void BackButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var previousPage = 2;
        if (OperatingSystem.IsLinux())
        {
            if (Configuration.Instance.DolphinBinLocation == "flatpak"
                || Configuration.Instance.DolphinBinLocation == "/usr/bin")
            {
                //Skip portable menu to go back to bin selection.
                previousPage = 1;
            }
        }
        SetOnboardingPage(previousPage);
    }
}