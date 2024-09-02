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
        BackButton.Click += (sender, args) => { SetOnboardingPage(2); };
    }

    private void ContinueButtonOnClick(object? sender, RoutedEventArgs e)
    {
        CommonUtils.LaunchDolphin(true);
        SetOnboardingPage(4);
    }
}