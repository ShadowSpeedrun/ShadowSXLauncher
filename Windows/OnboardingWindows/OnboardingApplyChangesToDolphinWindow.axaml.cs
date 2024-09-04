using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows.OnboardingWindows;

public partial class OnboardingApplyChangesToDolphinWindow : OnboardingWindow
{
    public OnboardingApplyChangesToDolphinWindow() : base()
    {
        InitializeComponent();
        RegisterEvents();
        CoreCheckBox.IsChecked = true;
        GraphicsCheckBox.IsChecked = true;
        HotkeyCheckBox.IsChecked = true;
        GameCheckBox.IsChecked = true;
    }

    private void RegisterEvents()
    {
        BackButton.Click += (sender, args) => { SetOnboardingPage(3); };
        ApplyButton.Click += ApplyButtonOnClick;
    }

    private void ApplyButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var coreSettingsPath = Path.Combine(CommonFilePaths.DolphinUserPath, "Config", "Dolphin.ini");
        var graphicsSettingsPath = Path.Combine(CommonFilePaths.DolphinUserPath, "Config", "GFX.ini");
        var hotkeySettingsPath = Path.Combine(CommonFilePaths.DolphinUserPath, "Config", "Hotkeys.ini");
        var gameSettingsPath = Path.Combine(CommonFilePaths.DolphinUserPath, "GameSettings", "GUPX8P.ini");
        
        if (OperatingSystem.IsLinux() && !CommonUtils.isDolphinPortable())
        {
            var linuxConfigPath = Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}",".config", "dolphin-emu");
            var linuxLocalPath = Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}",".local", "share", "dolphin-emu");
            coreSettingsPath = Path.Combine(linuxConfigPath, "Dolphin.ini");
            graphicsSettingsPath = Path.Combine(linuxConfigPath, "GFX.ini");
            hotkeySettingsPath = Path.Combine(linuxConfigPath, "Hotkeys.ini");
            gameSettingsPath = Path.Combine(linuxLocalPath, "GameSettings", "GUPX8P.ini");
        }
        
        if(CoreCheckBox.IsChecked!.Value)
        {
            var coreSettings = new DolphinSettingsAdjuster(coreSettingsPath);
            if (coreSettings.Sections.Count > 0)
            {
                //Data was found, so we will modify the file.
                coreSettings.SetSetting("Core", "CPUThread", false);
                coreSettings.SetSetting("Core", "EnableCheats", true);
                coreSettings.SetSetting("Core", "MMU", true);
                coreSettings.SetSetting("Core", "Overclock", 1.5);
                coreSettings.SetSetting("Core", "OverclockEnable", true);
                coreSettings.SetSetting("Interface", "OnScreenDisplayMessages", false);
                coreSettings.SetSetting("Interface", "PauseOnFocusLost", false);
                coreSettings.SaveSettings();
            }
            else
            {
                //No data was found, we will simply copy the file from our resources folder.
                if (File.Exists(coreSettingsPath))
                {
                    File.Delete(coreSettingsPath);
                }
                File.Copy(Path.Combine(CommonFilePaths.SxResourcesDolphinConfigFilesFolderPath, "Dolphin.ini"),coreSettingsPath);
            }
        }
        if(GraphicsCheckBox.IsChecked!.Value)
        {
            var graphicsSettings = new DolphinSettingsAdjuster(graphicsSettingsPath);
            if (graphicsSettings.Sections.Count > 0)
            {
                //Data was found, so we will modify the file.
                graphicsSettings.SetSetting("Hacks", "BBoxEnable", false);
                graphicsSettings.SetSetting("Hacks", "DeferEFBCopies", true);
                graphicsSettings.SetSetting("Hacks", "EFBEmulateFormatChanges", false);
                graphicsSettings.SetSetting("Hacks", "EFBScaledCopy", true);
                graphicsSettings.SetSetting("Hacks", "EFBToTextureEnable", true);
                graphicsSettings.SetSetting("Hacks", "SkipDuplicateXFBs", true);
                graphicsSettings.SetSetting("Hacks", "XFBToTextureEnable", true);
                graphicsSettings.SetSetting("Hacks", "EFBAccessEnable", false);
                graphicsSettings.SetSetting("Hacks", "ImmediateXFBEnable", true);

                graphicsSettings.SetSetting("Settings", "BackendMultithreading", true);
                graphicsSettings.SetSetting("Settings", "FastDepthCalc", true);
                graphicsSettings.SetSetting("Settings", "SaveTextureCacheToState", true);
                graphicsSettings.SetSetting("Settings", "CacheHiresTextures", true);
                graphicsSettings.SetSetting("Settings", "HiresTextures", true);
                graphicsSettings.SetSetting("Settings", "EnableGPUTextureDecoding", true);
                graphicsSettings.SetSetting("Settings", "SafeTextureCacheColorSamples", 512);
                graphicsSettings.SetSetting("Settings", "WaitForShadersBeforeStarting", true);

                graphicsSettings.SetSetting("Hardware", "VSync", true);
                graphicsSettings.SaveSettings();
            }
            else
            {
                //No data was found, we will simply copy the file from our resources folder.
                if (File.Exists(graphicsSettingsPath))
                {
                    File.Delete(graphicsSettingsPath);
                }
                File.Copy(Path.Combine(CommonFilePaths.SxResourcesDolphinConfigFilesFolderPath, "GFX.ini"),graphicsSettingsPath);
            }
            
            //Apply UI Fix Textures
            var dolphinCustomTexturePath = Path.Combine(CommonFilePaths.CustomTexturesPath, "UI Fix");
            if (Directory.Exists(dolphinCustomTexturePath))
            {
                Directory.Delete(dolphinCustomTexturePath, true);
            }

            var uiFixFilePath = Path.Combine(CommonFilePaths.SxResourcesCustomTexturesPath, "UI Fix");
            var uiFixFiles = Directory.EnumerateFiles(uiFixFilePath);
            
            Directory.CreateDirectory(dolphinCustomTexturePath);
            
            foreach (var uiFixFile in uiFixFiles)
            {
                var dest = Path.Combine(dolphinCustomTexturePath, uiFixFile.Replace(uiFixFilePath + Path.DirectorySeparatorChar, ""));
                File.Copy(uiFixFile, dest);
            }
        }
        if(HotkeyCheckBox.IsChecked!.Value)
        {
            var hotkeySettings = new DolphinSettingsAdjuster(hotkeySettingsPath);
            if (hotkeySettings.Sections.Count > 0)
            {
                //Data was found, so we will modify the file.
                hotkeySettings.SetSetting("Hotkeys", "General/Reset", "PAUSE");
                hotkeySettings.SetSetting("Hotkeys", "Graphics Toggles/Toggle Aspect Ratio", "F12");
                //TODO: Test preventing settings for savestates and speedup.
                hotkeySettings.RemoveSetting("Hotkeys", "Emulation Speed/Disable Emulation Speed Limit");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 1");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 2");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 3");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 4");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 5");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 6");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 7");
                hotkeySettings.RemoveSetting("Hotkeys", "Load State/Load State Slot 8");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 1");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 2");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 3");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 4");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 5");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 6");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 7");
                hotkeySettings.RemoveSetting("Hotkeys", "Save State/Save State Slot 8");
                hotkeySettings.RemoveSetting("Hotkeys", "Other State Hotkeys/Undo Load State");
                hotkeySettings.RemoveSetting("Hotkeys", "Other State Hotkeys/Undo Save State");
                hotkeySettings.SaveSettings();
            }
            else
            {
                //No data was found, we will simply copy the file from our resources folder.
                if (File.Exists(hotkeySettingsPath))
                {
                    File.Delete(hotkeySettingsPath);
                }
                File.Copy(Path.Combine(CommonFilePaths.SxResourcesDolphinConfigFilesFolderPath, "Hotkeys.ini"),hotkeySettingsPath);
            }
        }
        if(GameCheckBox.IsChecked!.Value)
        {
            var gameSettings = new DolphinSettingsAdjuster(gameSettingsPath);
            if (gameSettings.Sections.Count > 0)
            {
                //Data was found, so we will modify the file.
                gameSettings.SetSetting("Core", "FPRF", true);
                gameSettings.SetSetting("Core", "CPUThread", true);
                gameSettings.SetSetting("Core", "MMU", true);
                gameSettings.SetSetting("Core", "FastDiscSpeed", true);
                gameSettings.InstallGeckoCode("Bloom Reduction", "TheHatedGravity", "0456ac30 3f2b851f",
                    "Bloom intensity multiplier. Lessens the bloom effect to a more reasonable level.\n" +
                    "If you'd like a custom amount, then replace the second half of this code with a floating point number, in hexadecimal.\n" +
                    "Examples:\n" +
                    "0.25 : 3e800000\n" +
                    "0.375: 3ec00000\n" +
                    "0.5  : 3f000000\n" +
                    "0.95 : (default)\n*" +
                    "I choose 67% (3f2b851f) as makes rings on Prison rivers visible like on console.  May need to make a custom code to change per stage? -Zzetti");
                gameSettings.EnableGeckoCode("Bloom Reduction", true);
                gameSettings.SaveSettings();
            }
            else
            {
                //No data was found, we will simply copy the file from our resources folder.
                if (File.Exists(gameSettingsPath))
                {
                    File.Delete(gameSettingsPath);
                }
                File.Copy(Path.Combine(CommonFilePaths.SxResourcesDolphinConfigFilesFolderPath, "GUPX8P.ini"),gameSettingsPath);
            }
        }
        SetOnboardingPage(5);
    }
}