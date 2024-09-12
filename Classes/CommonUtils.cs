using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Win32;

namespace ShadowSXLauncher.Classes;

public static class CommonUtils
{
    public static async Task<bool> LaunchDolphin(bool showInterface = false)
    {
        if (File.Exists(Path.Combine(CommonFilePaths.DolphinBinPath, CommonFilePaths.DolphinBinFile)))
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = $"{Path.Combine(CommonFilePaths.DolphinBinPath, CommonFilePaths.DolphinBinFile)}"
            };

            if (!showInterface)
            {
                processInfo.Arguments = $" -b \"{Configuration.Instance.RomLocation}\"";
            }

            if (OperatingSystem.IsLinux())
            {
                processInfo.EnvironmentVariables["QT_QPA_PLATFORM"] = "xcb"; // Workaround until Dolphin runs on Wayland
                processInfo.UseShellExecute = false; // required for wayland override
            }

            Process.Start(processInfo);
            return true;
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
                            $"Flatpak not detected. Please check Flatpak is installed.{Environment.NewLine}Otherwise specify the paths to Dolphin");
                    await flatpakWarning.ShowAsync();
                    return false;
                }

                if (CommonFilePaths.DolphinUserPath !=
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.var/app/org.DolphinEmu.dolphin-emu/data/dolphin-emu/")
                {
                    var mismatchedConfig = MessageBoxManager
                        .GetMessageBoxStandard("Flatpak User Folder Mismatch",
                            $"Flatpak was detected, but your User Folder is set to an unexpected location.{Environment.NewLine}Press the Flatpak Button to automatically fix this in Settings.");
                    await mismatchedConfig.ShowAsync();
                    return false;
                }

                Process.Start("/usr/bin/flatpak",
                    showInterface
                        ? "run org.DolphinEmu.dolphin-emu"
                        : $"run org.DolphinEmu.dolphin-emu -b \"{Configuration.Instance.RomLocation}\"");
                return true;
            }

            var message = MessageBoxManager
                .GetMessageBoxStandard("Dolphin not found", "Could not find Dolphin. Please double check directory files.");
            var result = await message.ShowAsync();
            return false;
        }
    }
    
    public static async Task OpenSetDolphinBinDialog(Window parentWindow)
    {
        var result = await SetFolderPath("Set Path to Dolphin Executable", parentWindow);
        Configuration.Instance.DolphinBinLocation = string.IsNullOrEmpty(result) ? "" : result;
        Configuration.Instance.SaveSettings();
    }
    
    public static async Task OpenSetDolphinUserDialog(Window parentWindow)
    {
        var result = await SetFolderPath("Set Path to Dolphin User Folder", parentWindow);
        Configuration.Instance.DolphinUserLocation = string.IsNullOrEmpty(result) ? "" : result;
        Configuration.Instance.SaveSettings();
    }

    private static async Task<string?> SetFolderPath(string title, Window parentWindow)
    {
        var ofd = new OpenFolderDialog();
        ofd.Title = title;
        ofd.Directory = CommonFilePaths.AppStart; 
        return await ofd.ShowAsync(parentWindow);
    }
    
    /// <summary>
    /// Open the provided folder path in the file explorer of the current operating system.
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns>Returns True if File Path Exists.</returns>
    public static bool OpenFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = CommonFilePaths.GetExplorerPath,
                Arguments = "\"" + folderPath + "\"",
                UseShellExecute = true
            };
            Process.Start(psi);
            return true;
        }

        return false;
    }
    
    public static async Task<string[]?> SetOpenFilePath(string title, FileDialogFilter filter, Window parentWindow)
    {
        var ofd = new OpenFileDialog();
        ofd.Title = title;
        ofd.Filters = new List<FileDialogFilter>() { filter };
        ofd.Directory = CommonFilePaths.AppStart;
        ofd.AllowMultiple = false;
        return await ofd.ShowAsync(parentWindow);
    }
    
    public static async Task<string?> SetSaveFilePath(string title, FileDialogFilter filter, Window parentWindow)
    {
        var sfd = new SaveFileDialog();
        sfd.Title = title;
        sfd.Filters = new List<FileDialogFilter>() { filter };
        sfd.Directory = CommonFilePaths.AppStart;
        return await sfd.ShowAsync(parentWindow);
    }

    public static bool isDolphinPortable()
    {
        if (CommonFilePaths.DolphinBinPath == "flatpak")
        {
            //Hackaround as we assume flatpack is not portable.
            return false;
        }
        return Directory.GetFiles(CommonFilePaths.DolphinBinPath, "*.*", SearchOption.TopDirectoryOnly)
            .Any(file => Path.GetFileName(file).Equals("portable.txt", StringComparison.OrdinalIgnoreCase));
    }

    public static string SetWindowsUserGlobal()
    {
        if (OperatingSystem.IsWindows())
        {
            var docsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Dolphin Emulator");
            var appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Dolphin Emulator");
            
            var dolphinEmuKey = Registry.CurrentUser.OpenSubKey("Software", true)?.OpenSubKey("Dolphin Emulator", true);
            var dolphinUserPathRegistryValue = dolphinEmuKey.GetValue("UserConfigPath");
            
            if (dolphinUserPathRegistryValue != null)
            {
                return ((string)dolphinUserPathRegistryValue);
            }

            //Dolphin will first check if the docs path exists, if not, will then make a directory in AppData.
            return Directory.Exists(docsPath) ? docsPath : appdataPath;
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
}
