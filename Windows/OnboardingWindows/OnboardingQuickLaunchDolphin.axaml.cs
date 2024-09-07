using System;
using Avalonia.Interactivity;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingQuickLaunchDolphin : OnboardingWindow
{

    private int continueTimesPressed = 0;
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
        if (continueTimesPressed == 0)
        {
            _ = CommonUtils.LaunchDolphin(true);
            QuickLaunchDolphinStepsTextBlock.Text = $"Dolphin has been launched successfully.{Environment.NewLine}You need to close Dolphin before you continue.";
            continueTimesPressed++;
            return;
        }
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