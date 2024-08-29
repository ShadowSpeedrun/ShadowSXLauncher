using System;
using System.Diagnostics;
using System.IO;
using MsBox.Avalonia;

namespace ShadowSXLauncher.Classes;

public static class CommonFilePaths
{
    public static string AppStart
    {
        get { return AppContext.BaseDirectory; }
    }

    public static string ConfigFileLocation
    {
        get { return Path.Combine(AppStart, "Config.xml"); }
    }

    public static string DolphinBinPath
    {
        get { return Configuration.Instance.DolphinBinLocation; }
    }
    
    public static string DolphinUserPath
    {
        get { return Configuration.Instance.DolphinUserLocation; }
    }
        
    public static string SavePath
    {
        get { return Path.Combine(DolphinUserPath, "GC", "USA", "Card A"); }
    }
        
    public static string GameSettingsFilePath
    {
        get { return Path.Combine(DolphinUserPath, "GameSettings", "GUPX8P.ini"); }
    }

    public static string CustomTexturesPath
    {
        get { return Path.Combine(DolphinUserPath, "Load", "Textures", "GUPX8P"); }
    }

    public static string SxResourcesPath
    {
        get { return Path.Combine(AppStart, "ShadowSXResources"); }
    }
        
    public static string SxResourcesCustomTexturesPath
    {
        get { return Path.Combine(SxResourcesPath, @"CustomTextures", "GUPX8P"); }
    }
        
    public static string SxResourcesISOPatchingPath
    {
        get { return Path.Combine(SxResourcesPath, @"PatchingFiles"); }
    }
    
    public static string SxResourcesPatchDataFolderPath
    {
        get { return Path.Combine(SxResourcesISOPatchingPath, "Patches"); }
    }
    
    public static string SxResourcesPatchBinsFolderPath
    {
        get { return Path.Combine(SxResourcesISOPatchingPath, "PatchingScriptsAndBins"); }
    }

    public static string DolphinBinFile
    {
        get
        {   
            if(OperatingSystem.IsWindows())
            {
                return "Dolphin.exe";
            }
            
            if (OperatingSystem.IsLinux())
            {
                return "dolphin-emu";
            }
            
            if (OperatingSystem.IsMacOS())
            {
                //TODO: What extension is used once it's extracted?
                return "Dolphin";
            }

            return string.Empty;
        }
    }
    
    public static string xdeltaBinPath
    {
        get
        {
            if(OperatingSystem.IsWindows())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "xdelta-3.1.0-x86_64.exe");
            }
            
            if (OperatingSystem.IsLinux())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "xdelta3");
            }
            
            if (OperatingSystem.IsMacOS())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "xdelta3_mac");
            }

            return string.Empty;
        }
    }
    
    public static string PatchingScriptPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "Patch-ISO.bat");
            }
            
            if (OperatingSystem.IsLinux())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "Patch-ISO.sh");
            }
            
            if (OperatingSystem.IsMacOS())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "Patch-ISO.command");
            }

            return string.Empty;
        }
    }
    
    public static string GetExplorerPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return "explorer.exe";
            }
            
            if (OperatingSystem.IsLinux())
            {
                return "xdg-open";
            }
            
            if (OperatingSystem.IsMacOS())
            {
                return "open";
            }
            
            throw new Exception("Unsupported Operating System");
        }
    }
    
    // TODO: Probably move these to a new util file?
    /*private static async Task<string> GetFlatpakBinPath()
    {
        var checkFlatpak = new Process();
        checkFlatpak.StartInfo.FileName = "which";
        checkFlatpak.StartInfo.Arguments = "flatpak";
        checkFlatpak.StartInfo.RedirectStandardOutput = true;
        checkFlatpak.Start();
        await checkFlatpak.WaitForExitAsync();
        var flatpakDirectory = await checkFlatpak.StandardOutput.ReadToEndAsync();
        return flatpakDirectory.Trim('\n');
    }*/
    
    // TODO: Probably move these to a new util file?
    public static async void LaunchDolphin(bool showInterface = false)
    {
        if (File.Exists(Path.Combine(CommonFilePaths.DolphinBinPath, CommonFilePaths.DolphinBinFile)))
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = $"{Path.Combine(CommonFilePaths.DolphinBinPath, CommonFilePaths.DolphinBinFile)}"
            };

            if (!showInterface)
            {
                processInfo.Arguments = $@" -b {Configuration.Instance.RomLocation}";
            }

            if (OperatingSystem.IsLinux())
            {
                processInfo.EnvironmentVariables["QT_QPA_PLATFORM"] = "xcb"; // Workaround until Dolphin runs on Wayland
                processInfo.UseShellExecute = false; // required for wayland override
            }

            Process.Start(processInfo);
        }
        else
        {
            // A bit of a hack, if the user is configured for the flatpak the above 'If' will fail
            // since we would check /usr/bin/flatpak/dolphin-emu, which won't ever exist.
            if (OperatingSystem.IsLinux()) 
            {
                if (!File.Exists("/usr/bin/flatpak"))
                {
                    var flatpakWarning = MessageBoxManager
                        .GetMessageBoxStandard("Flatpak Not Found",
                            "Flatpak not detected. Please check Flatpak is installed.\nOtherwise specify the paths to Dolphin");
                    await flatpakWarning.ShowAsync();
                    return;
                }

                Process.Start("/usr/bin/flatpak",
                    showInterface
                        ? "run org.DolphinEmu.dolphin-emu"
                        : $"run org.DolphinEmu.dolphin-emu -b {Configuration.Instance.RomLocation}");
                return;
            }
           
            var message = MessageBoxManager
                .GetMessageBoxStandard("Dolphin not found", "Could not find Dolphin. Please double check directory files.");
            var result = await message.ShowAsync();
        }
    }
}