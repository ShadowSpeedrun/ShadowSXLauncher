using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingCompleteWindow : OnboardingWindow
{
    public OnboardingCompleteWindow() : base()
    {
        InitializeComponent();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        OpenDolphinButton.Click += (sender, args) =>
        {
            CommonUtils.LaunchDolphin(true);
            Close();
        };
        FinishButton.Click += (sender, args) => { Close(); };
    }
}