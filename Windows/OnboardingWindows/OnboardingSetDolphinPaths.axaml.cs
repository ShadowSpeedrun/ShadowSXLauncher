using Avalonia.Interactivity;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingSetDolphinPaths : OnboardingWindow
{
    public OnboardingSetDolphinPaths() : base()
    {
        InitializeComponent();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        SetDolphinPathButton.Click += SetDolphinPathButtonOnClick;
        BackButton.Click += (sender, args) => { SetOnboardingPage(0); };
    }

    private async void SetDolphinPathButtonOnClick(object? sender, RoutedEventArgs e)
    {
        await CommonUtils.OpenSetDolphinBinDialog(this);

        if (!string.IsNullOrEmpty(Configuration.Instance.DolphinBinLocation))
        {
            SetOnboardingPage(2);
        }
    }
}