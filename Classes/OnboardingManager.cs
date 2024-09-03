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
        List<Type> onboardingPages = null;
        if (OperatingSystem.IsWindows())
        {
            onboardingPages = new List<Type>()
            {
                typeof(OnboardingIntroWindow),
                typeof(OnboardingSetDolphinPaths),
                typeof(OnboardingWindowsDolphinPortable),
                typeof(OnboardingQuickLaunchDolphin),
                typeof(OnboardingApplyChangesToDolphinWindow),
                typeof(OnboardingCompleteWindow)
            };
        }
        else if(OperatingSystem.IsLinux())
        {
            //TODO: Adjust for Linux
            onboardingPages = new List<Type>()
            {
                typeof(OnboardingIntroWindow),
                typeof(OnboardingSetDolphinPathsLinux),
                typeof(OnboardingLinuxDolphinPortable),
                typeof(OnboardingQuickLaunchDolphin),
                typeof(OnboardingApplyChangesToDolphinWindow),
                typeof(OnboardingCompleteWindow)
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
            //To avoid the "Cannot re-show a closed window." Error, we are going to get fancy with
            //how we create and show our dialogs.
            var pageToShow = Activator.CreateInstance(onboardingPages[currentOnboardingPage]) as OnboardingWindow;
            
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