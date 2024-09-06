using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingIntroWindow : OnboardingWindow
{
    public OnboardingIntroWindow() : base()
    {
        InitializeComponent();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        ContinueButton.Click += (sender, args) => { SetOnboardingPage(1); };
        CloseButton.Click += (sender, args) => { Close(); };
    }
}