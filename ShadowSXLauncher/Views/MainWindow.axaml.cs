using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Views;

public partial class MainWindow : Window
{
    private string dolphinPath
    {
        get { return Configuration.Instance.DolphinLocation; }
    }
        
    private string savePath
    {
        get { return Path.Combine(dolphinPath, "User", "GC", "USA", "Card A"); }
    }
        
    private string gameSettingsFilePath
    {
        get { return Path.Combine(dolphinPath, "User", "GameSettings", "GUPX8P.ini"); }
    }

    private string customTexturesPath
    {
        get { return Path.Combine(dolphinPath ,"User","Load","Textures","GUPX8P"); }
    }

    private string sxResourcesPath
    {
        get { return Path.Combine(Configuration.AppStart, "ShadowSXResources"); }
    }
        
    private string sxResourcesCustomTexturesPath
    {
        get { return Path.Combine(sxResourcesPath + @"CustomTextures","GUPX8P"); }
    }
        
    private string sxResourcesISOPatchingPath
    {
        get { return Path.Combine(sxResourcesPath + @"PatchingFiles"); }
    }

    public MainWindow()
    {
        InitializeComponent();
        
#if DEBUG
        this.AttachDevTools();
#endif
        
        Configuration.Instance.LoadSettings();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        PlayButton.Click += OnPlayButtonPressed;
        CreateROMButton.Click += CreateRomButton_Click;
        OpenGameLocationButton.Click += OpenGameLocationButtonPressed;
        OpenSaveFileLocationButton.Click += OnSaveFileButtonPressed;
        SettingsButton.Click += SettingsButton_Click;
    }

    private void EnableButtons(bool enable)
    {
        PlayButton.IsEnabled = enable;
        CreateROMButton.IsEnabled = enable;
        OpenGameLocationButton.IsEnabled = enable;
        OpenSaveFileLocationButton.IsEnabled = enable;
        SettingsButton.IsEnabled = enable;
    }
    
    private async void OnPlayButtonPressed(object? sender, RoutedEventArgs e)
    {
        EnableButtons(false);
        
        if (string.IsNullOrEmpty(Configuration.Instance.DolphinLocation))
        {
            await OpenSetDolphinDialog();
        }

        if (!string.IsNullOrEmpty(Configuration.Instance.DolphinLocation))
        {
            //Check if Rom Location has been set at all.
            if (string.IsNullOrEmpty(Configuration.Instance.RomLocation))
            {
                await OpenSetRomDialog();
            }

            //Only continue if Rom Location has been set, in case it was not in the above code. 
            if (!string.IsNullOrEmpty(Configuration.Instance.RomLocation))
            {
                //Double check if the provided path has a file, if not re-prompt for a ROM.
                if (!File.Exists(Configuration.Instance.RomLocation))
                {
                    ShowMessageBox("ROM not found", "ROM file not found. Please provide ROM location again.", new []{"OK"});
                    //OpenRomDialog();
                }

                //At this point assume there is a correct ROM. Technically nothing stopping a user from
                //choosing whatever ROM they want to launch, but trying to account for that without additional
                //annoying checks and processes is not worth it.

                UpdateCustomAssets();

                //Double check the .exe is found before attempting to run it.
                if (File.Exists(dolphinPath + @"\Dolphin.exe"))
                {
                    Process.Start("\"" + dolphinPath + @"\Dolphin.exe" + "\"",
                        @" -b " + "\"" + Configuration.Instance.RomLocation + "\"");
                    Close();
                }
                else
                {
                    ShowMessageBox("Dolphin not found","Could not find dolphin.exe. Please double check directory files.", new []{"OK"});
                }
            }
        }
        
        EnableButtons(true);
    }

    private void ShowMessageBox(string title, string message, string?[] options, 
        Action<object?, EventArgs> Button1ClickedAction = null,
        Action<object?, EventArgs> Button2ClickedAction = null,
        Action<object?, EventArgs> Button3ClickedAction = null)
    {
        var mb = new MessageBox(title, message, options);
        if (Button1ClickedAction != null)
        {
            mb.Button1Clicked += Button1ClickedAction;
        }
        if (Button2ClickedAction != null)
        {
            mb.Button2Clicked += Button2ClickedAction;
        }
        if (Button3ClickedAction != null)
        {
            mb.Button3Clicked += Button3ClickedAction;
        }
        mb.ShowDialog(this);
    }

    private async Task OpenSetRomDialog()
    {
        var result = await SetOpenFilePath("Set Path to SX ROM", 
        new FileDialogFilter()
        {
            Name = "ROM File",
            Extensions = new List<string>() {"iso"}
        });
        
        Configuration.Instance.RomLocation = result == null ? "" : result.First();
    }
    
    private async Task<string[]?> SetOpenFilePath(string title, FileDialogFilter filter)
    {
        var ofd = new OpenFileDialog();
        ofd.Title = title;
        ofd.Filters = new List<FileDialogFilter>() { filter };
        ofd.Directory = Configuration.AppStart;
        ofd.AllowMultiple = false;
        return await ofd.ShowAsync(this);
    }
    
    private async Task<string?> SetSaveFilePath(string title, FileDialogFilter filter)
    {
        var sfd = new SaveFileDialog();
        sfd.Title = title;
        sfd.Filters = new List<FileDialogFilter>() { filter };
        sfd.Directory = Configuration.AppStart;
        return await sfd.ShowAsync(this);
    }

    private async Task OpenSetDolphinDialog()
    {
        var result = await SetFolderPath("Set Path to Dolphin");
        Configuration.Instance.DolphinLocation = String.IsNullOrEmpty(result) ? "" : result;
    }

    private async Task<string?> SetFolderPath(string title)
    {
        var ofd = new OpenFolderDialog();
        ofd.Title = "Set Path to Dolphin";
        ofd.Directory = Configuration.AppStart; 
        return await ofd.ShowAsync(this);
    }

    private void OpenGameLocationButtonPressed(object? sender, RoutedEventArgs e)
    {
        OpenFolder(Configuration.AppStart);
    }

    private void OnSaveFileButtonPressed(object? sender, RoutedEventArgs e)
    {
        var success = OpenFolder(savePath);
        if (!success)
        {
            ShowMessageBox("Save folder not found", "Please launch game to generate the save directory.",
                new[] { "OK" });
        }
    }

    /// <summary>
    /// Open the provided folder path in the file explorer of the current operating system.
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns>Returns True if File Path Exists.</returns>
    private bool OpenFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = GetExplorerPath(),
                Arguments = folderPath,
                UseShellExecute = true
            };
            Process.Start(psi);
            return true;
        }

        return false;
    }
    
    private string GetExplorerPath()
    {
        switch (Configuration.Instance.CurrentOS)
        {
            case OperatingSystemType.WinNT:
                return "explorer.exe";
            case OperatingSystemType.Linux:
                return "xdg-open";
            case OperatingSystemType.OSX:
                return "open";
            default:
                throw new Exception("Unsupported Operating System");
        }
    }

    private void SettingsButton_Click(object? sender, EventArgs e)
    {
        EnableButtons(false);
        var settingsDialog = new SettingsWindow();
        settingsDialog.ShowDialog(this);
        EnableButtons(true);
    }
    
    private async void CreateRomButton_Click(object? sender, EventArgs e)
    {
        var xdeltaBinPath = Path.Combine(sxResourcesISOPatchingPath, PatchingFiles.Bin);
        var vcdiffPath = Path.Combine(sxResourcesISOPatchingPath, PatchingFiles.PatchFile);
        var patchScriptPath = Path.Combine(sxResourcesISOPatchingPath, PatchingFiles.PathScript);
                
        var allPatchFilesFound = File.Exists(xdeltaBinPath);
        allPatchFilesFound &= File.Exists(vcdiffPath);
        allPatchFilesFound &= File.Exists(patchScriptPath);
        
        if (allPatchFilesFound)
        {
            var gupe8pLocation = "";
            var patchedRomDestination = "";

            var resultGUPE = await SetOpenFilePath("Select Original ROM (GUPE8P)", 
                new FileDialogFilter()
                {
                    Name = "ROM File",
                    Extensions = new List<string>() {"iso"}
                });
        
            gupe8pLocation = resultGUPE == null ? "" : resultGUPE.First();

            if (!string.IsNullOrEmpty(gupe8pLocation))
            {
                var resultGUPX = await SetSaveFilePath("Save Patched ROM (GUPX8P)", 
                    new FileDialogFilter()
                    {
                        Name = "ROM File",
                        Extensions = new List<string>() {"iso"}
                    });
                patchedRomDestination = resultGUPX ?? "";
            }
            else
            {
                ShowMessageBox("Operation Cancelled", "Operation Cancelled", new []{"OK"});
                return;
            }
            
            //We can assume that gupe8pLocation is not empty or null. 
            if (!string.IsNullOrEmpty(patchedRomDestination))
            {
                var batArguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"", gupe8pLocation, patchedRomDestination,
                        xdeltaBinPath, vcdiffPath);
                    
                var processResult = Process.Start("\"" + patchScriptPath + "\"", batArguments);
                if (processResult != null)
                {
                    processResult.WaitForExit();

                    switch (processResult.ExitCode)
                    {
                        case 0:
                            //MessageBox by default doesnt have alignment options. Hack it to look nice to avoid needing to create a new control dialog.
                            //var messageResult = 
                                ShowMessageBox("ROM Patch Successful",
                                "ROM Created Successfully." + Environment.NewLine + Environment.NewLine
                                + "Would you like to set the location of this ROM as the " + Environment.NewLine
                                + "location this launcher will use to launch the game?", new[] { "Yes", "No" },
                                delegate(object? o, EventArgs args)
                                {
                                    Configuration.Instance.RomLocation = patchedRomDestination;
                                    Configuration.Instance.SaveSettings();
                                });
                            break;
                        default:
                            //MessageBox by default doesnt have alignment options. Hack it to look nice to avoid needing to create a new control dialog.
                            ShowMessageBox("ROM Patch Failed",
                                "ROM Patching Failed." + Environment.NewLine + Environment.NewLine
                                + "Please ensure that provided paths are valid and that " + Environment.NewLine
                                + "the Shadow ROM provided is a full size clean rip. " + Environment.NewLine + Environment.NewLine
                                + "Expected ROM CRC32: F582CF1E", new []{"OK"});
                            break;
                    }
                }
                else
                {
                    ShowMessageBox("Patching Failed","ROM Patching failed to launch.", new []{"OK"});
                }
            }
            else
            {
                ShowMessageBox("Operation Cancelled", "Operation Cancelled", new []{"OK"});
            }
        }
        else
        {
            ShowMessageBox("Missing Files",
                "One or more files needed to complete" + Environment.NewLine +
                       "the ROM patching were missing." + Environment.NewLine + Environment.NewLine +
                       "Please double check directory files.", new []{"OK"});
        }
    }
    
    private void UpdateCustomAssets()
    {
        #region UI Display Textures

        if (Directory.Exists(customTexturesPath + @"\Buttons"))
        {
            Directory.Delete(customTexturesPath + @"\Buttons", true);
        }

        var buttonAssetsFolder =
            Configuration.UiButtonStyles.Keys.ToArray()[Configuration.Instance.UiButtonDisplayIndex];
        if (!string.IsNullOrEmpty(buttonAssetsFolder))
        {
            var newButtonFilePath = sxResourcesCustomTexturesPath + @"\Buttons\" + buttonAssetsFolder;
            var newButtonUiFiles = Directory.EnumerateFiles(newButtonFilePath);
            
            Directory.CreateDirectory(customTexturesPath + @"\Buttons");
            
            foreach (var buttonFile in newButtonUiFiles)
            {
                File.Copy(buttonFile, customTexturesPath + @"\Buttons" + buttonFile.Replace(newButtonFilePath, ""));
            }
        }

        #endregion
        
        #region Gloss Removal

        if (Directory.Exists(customTexturesPath + @"\GlossAdjustment"))
        {
            Directory.Delete(customTexturesPath + @"\GlossAdjustment", true);
        }

        var glossAssetsFolder = 
            Configuration.GlossAdjustmentOptions.Keys.ToArray()[Configuration.Instance.GlossAdjustmentIndex];
        if (!string.IsNullOrEmpty(glossAssetsFolder))
        {
            var removeGlossFilePath = sxResourcesCustomTexturesPath + @"\GlossAdjustment\" + glossAssetsFolder;
            var removeGlossFiles = Directory.EnumerateFiles(removeGlossFilePath);
            
            Directory.CreateDirectory(customTexturesPath + @"\GlossAdjustment");
            
            foreach (var removeGlossFile in removeGlossFiles)
            {
                File.Copy(removeGlossFile, customTexturesPath + @"\GlossAdjustment" + removeGlossFile.Replace(removeGlossFilePath, ""));
            }
        }

        #endregion
    }
}