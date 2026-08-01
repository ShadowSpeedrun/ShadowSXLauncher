using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using ShadowSXLauncher.Windows.OnboardingWindows;

namespace ShadowSXLauncher.Classes;

public static class OnboardingManager
{
    public static async Task RunOnboarding(Window parentWindow)
    {
        Configuration.Instance.OnboardingCompleted = false;
        var currentOnboardingPage = 0;
        List<Func<OnboardingWindow>> onboardingPages = null;
        if (OperatingSystem.IsWindows())
        {
            onboardingPages = new List<Func<OnboardingWindow>>()
            {
                () => new OnboardingIntroWindow(),
                () => new OnboardingSetDolphinPaths(),
                () => new OnboardingWindowsDolphinPortable(),
                () => new OnboardingQuickLaunchDolphin(),
                () => new OnboardingApplyChangesToDolphinWindow(),
                () => new OnboardingCompleteWindow()
            };
        }
        else if(OperatingSystem.IsLinux())
        {
            onboardingPages = new List<Func<OnboardingWindow>>()
            {
                () => new OnboardingIntroWindow(),
                () => new OnboardingSetDolphinPathsLinux(),
                () => new OnboardingLinuxDolphinPortable(),
                () => new OnboardingQuickLaunchDolphin(),
                () => new OnboardingApplyChangesToDolphinWindow(),
                () => new OnboardingCompleteWindow()
            };
        }
        else
        {
            //Skip for untested OS.
            Configuration.Instance.OnboardingCompleted = true;
        }

        //Loop until the OnboardingComplete flag has been set.
        //OnboardingComplete flag is set when completed, or exiting early.
        while (onboardingPages != null && onboardingPages.Count > 0 && !Configuration.Instance.OnboardingCompleted)
        {
            var pageToShow = onboardingPages[currentOnboardingPage]();
            try
            {
                //Register callback event to change pages or exit onboarding.
                pageToShow.OnboardingChanged += (sender, args) =>
                {
                    Configuration.Instance.OnboardingCompleted = args.OnboardingComplete;
                    currentOnboardingPage = args.NextOnboardingPage;
                };
                await pageToShow.ShowDialog(parentWindow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        Configuration.Instance.SaveSettings();
    }
}