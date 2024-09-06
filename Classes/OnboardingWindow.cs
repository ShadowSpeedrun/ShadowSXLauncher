using System;
using Avalonia.Controls;

namespace ShadowSXLauncher.Classes;

public class OnboardingWindow : Window
{
    public OnboardingWindow() : base()
    {
        Closing += OnboardingClose;
    }
    
    public class OnboardingEventArgs : EventArgs
    {
        public bool OnboardingComplete = false;
        public int NextOnboardingPage = 0;
    }

    public EventHandler<OnboardingEventArgs> OnboardingChanged;
    
    private void OnboardingClose(object? sender, WindowClosingEventArgs e)
    {
        OnboardingChanged?.Invoke(this, new OnboardingEventArgs { OnboardingComplete = true });
    }

    /// <summary>
    /// This will set the next page and close the window, preventing an onboarding complete message.
    /// </summary>
    /// <param name="pageNumber"></param>
    protected void SetOnboardingPage(int pageNumber)
    {
        Closing -= OnboardingClose;
        OnboardingChanged?.Invoke(this, new OnboardingEventArgs { NextOnboardingPage = pageNumber });
        Close();
    }
}