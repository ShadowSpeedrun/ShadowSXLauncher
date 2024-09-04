using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingSetDolphinPathsLinux : OnboardingWindow
{
    public OnboardingSetDolphinPathsLinux() : base()
    {
        InitializeComponent();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        SetFlatpakPathButton.Click += SetFlatpakPathButtonOnClick;
        SetNativePathButton.Click += SetNativeButtonOnClick;
        SetCustomPathButton.Click += SetCustomPathButtonOnClick;
        BackButton.Click += (sender, args) => { SetOnboardingPage(0); };
    }

    private void SetFlatpakPathButtonOnClick(object? sender, RoutedEventArgs e)
    {
        Configuration.Instance.SetDolphinPathsForFlatpakAndPortable();
        
        //We can skip setting the user path in this case.
        SetOnboardingPage(3);
    }

    private void SetNativeButtonOnClick(object? sender, RoutedEventArgs e)
    {
        Configuration.Instance.SetDolphinPathsForNativeAndGlobal();
        
        //We can skip setting the user path in this case.
        SetOnboardingPage(3);
    }
    
    private async void SetCustomPathButtonOnClick(object? sender, RoutedEventArgs e)
    {
        await CommonUtils.OpenSetDolphinBinDialog(this);

        if (!string.IsNullOrEmpty(Configuration.Instance.DolphinBinLocation))
        {
            SetOnboardingPage(2);
        }
    }
}