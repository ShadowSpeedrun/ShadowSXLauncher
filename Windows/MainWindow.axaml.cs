using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ShadowSXLauncher.Classes;
using ShadowSXLauncher.Windows.OnboardingWindows;

namespace ShadowSXLauncher.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
#if DEBUG
        this.AttachDevTools();
#endif
        RegisterEvents();
        EnableButtons(true);
        CreateROMButton.IsVisible = !Configuration.Instance.SteamDeckMode;
        OpenGameLocationButton.IsVisible = !Configuration.Instance.SteamDeckMode;
        OpenLauncherLocationButton.IsVisible = !Configuration.Instance.SteamDeckMode;
        OpenSaveFileLocationButton.IsVisible = !Configuration.Instance.SteamDeckMode;
    }

    private void RegisterEvents()
    {
        PlayButton.Click += OnPlayButtonPressed;
        CreateROMButton.Click += CreateRomButton_Click;
        OpenGameLocationButton.Click += OpenGameLocationButtonPressed;
        OpenLauncherLocationButton.Click += OpenLauncherLocationButtonPressed;
        OpenSaveFileLocationButton.Click += OnSaveFileButtonPressed;
        SettingsButton.Click += SettingsButton_Click;
        ExitButton.Click += (sender, args) => { Close(); };
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Configuration.Instance.LoadSettings();
        if (!Configuration.Instance.OnboardingCompleted)
        {
            RunOnboading();
        }
    }

    private async void RunOnboading()
    {
        EnableButtons(false);
        await OnboardingManager.RunOnboarding(this);
        EnableButtons(true);
    }

    private void EnableButtons(bool enable)
    {
        PlayButton.IsEnabled = enable;
        CreateROMButton.IsEnabled = enable;
        OpenGameLocationButton.IsEnabled = enable && !string.IsNullOrEmpty(Configuration.Instance.RomLocation);
        OpenLauncherLocationButton.IsEnabled = enable;
        OpenSaveFileLocationButton.IsEnabled = enable;
        SettingsButton.IsEnabled = enable;
        ExitButton.IsEnabled = enable;
    }
    
    private async void OnPlayButtonPressed(object? sender, RoutedEventArgs e)
    {
        EnableButtons(false);

        // If Dolphin Paths have not been set, we assume Portable on Windows and Flatpak on Linux
        if (string.IsNullOrEmpty(Configuration.Instance.DolphinBinLocation))
        {
            Configuration.Instance.SetDolphinPathsForFlatpakAndPortable();
        }

        // On Windows only, if the paths don't exist, we prompt for manual paths. We don't do this on Linux due to differences in bin handling
        if (OperatingSystem.IsWindows() && !Directory.Exists(Configuration.Instance.DolphinBinLocation))
        {
            await CommonUtils.OpenSetDolphinBinDialog(this);
        }
        
        if (OperatingSystem.IsWindows() && (string.IsNullOrEmpty(Configuration.Instance.DolphinUserLocation) || !Directory.Exists(Configuration.Instance.DolphinUserLocation)))
        {
            await CommonUtils.OpenSetDolphinUserDialog(this);
        }

        // Check if Rom Location has been set, and if the file is accessible.
        if (string.IsNullOrEmpty(Configuration.Instance.RomLocation) || !File.Exists(Configuration.Instance.RomLocation))
        {
            await OpenSetRomDialog();
        }

        // Only continue if Rom Location has been set
        if (!string.IsNullOrEmpty(Configuration.Instance.RomLocation))
        {
            // Double-check if the provided path has a file, if not re-prompt for a ROM.
            if (!File.Exists(Configuration.Instance.RomLocation))
            {
                var message = MessageBoxManager
                    .GetMessageBoxStandard("ROM not found", "ROM file not found. Please provide ROM location again.");
                var result = await message.ShowAsync();
                return;
            }

            // At this point assume there is a correct ROM. Technically nothing stopping a user from
            // choosing whatever ROM they want to launch, but trying to account for that without additional
            // annoying checks and processes is not worth it.

            UpdateCustomAssets();
            var launchedSuccessfully = await CommonUtils.LaunchDolphin(showInterface: false);

            if (OperatingSystem.IsWindows() && launchedSuccessfully)
            {
                Close(); // Not working with Linux (child process issue)?
            }
        }
        EnableButtons(true);
    }
    
    private async Task OpenSetRomDialog()
    {
        var result = await CommonUtils.SetOpenFilePath("Set Path to SX ROM", 
        new FileDialogFilter()
        {
            Name = "ROM File",
            Extensions = new List<string>() {"iso", "rvz"}
        }, this);
        
        Configuration.Instance.RomLocation = (result == null || result.Length == 0) ? "" : result.First();
        Configuration.Instance.SaveSettings();
    }

    private void OpenGameLocationButtonPressed(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Configuration.Instance.RomLocation))
        {
            var directoryName = Path.GetDirectoryName(Configuration.Instance.RomLocation);
            if (!string.IsNullOrEmpty(directoryName))
            {
                CommonUtils.OpenFolder(directoryName);
            }
        }
    }
    
    private void OpenLauncherLocationButtonPressed(object? sender, RoutedEventArgs e)
    {
        CommonUtils.OpenFolder(CommonFilePaths.AppStart);
    }

    private async void OnSaveFileButtonPressed(object? sender, RoutedEventArgs e)
    {
        var success = CommonUtils.OpenFolder(CommonFilePaths.SavePath);
        if (!success)
        {
            var message = MessageBoxManager
                .GetMessageBoxStandard("Save folder not found", "Please launch game to generate the save directory.");
            var result = await message.ShowAsync();
        }
    }

    private async void SettingsButton_Click(object? sender, EventArgs e)
    {
        EnableButtons(false);
        var settingsDialog = new SettingsWindow();
        await settingsDialog.ShowDialog(this);
        EnableButtons(true);
    }
    
    private async void CreateRomButton_Click(object? sender, EventArgs e)
    {
        EnableButtons(false);
        var chooseROMPatchDialog = new ChooseROMPatchWindow();
        var result = await chooseROMPatchDialog.ShowDialog<PatchData?>(this);
        if (result != null)
        {
            await CreateROMWindows(result);
        }
        EnableButtons(true);
    }

    private async Task CreateROMWindows(PatchData patch)
    {
        var vcdiffPath = patch.SelectedPatchFilePath;
        var xdeltaBinPath = CommonFilePaths.xdeltaBinPath;
                
        var allPatchFilesFound = File.Exists(xdeltaBinPath);
        allPatchFilesFound &= File.Exists(vcdiffPath);
        
        if (allPatchFilesFound)
        {
            var baseIdLocation = "";
            var patchedRomDestination = "";

            var resultBaseId = await CommonUtils.SetOpenFilePath("Select Original ROM (" + patch.SelectedVariant.OriginalGameId + ")", 
                new FileDialogFilter()
                {
                    Name = "ROM File",
                    Extensions = new List<string> {"iso"}
                }, this);
        
            baseIdLocation = resultBaseId == null ? "" : resultBaseId.First();

            if (!string.IsNullOrEmpty(baseIdLocation))
            {
                var resultNewId = await CommonUtils.SetSaveFilePath("Save Patched ROM (" + patch.SelectedVariant.NewGameId + ")", 
                    new FileDialogFilter()
                    {
                        Name = "ROM File",
                        Extensions = new List<string> {"iso"}
                    }, this);
                patchedRomDestination = resultNewId ?? "";
            }
            else
            {
                var message = MessageBoxManager
                    .GetMessageBoxStandard("Operation Cancelled", "Operation Cancelled.");
                await message.ShowAsync();
                return;
            }
            
            if (!string.IsNullOrEmpty(patchedRomDestination))
            {
                var processResult = new Process();
                processResult.StartInfo.FileName = xdeltaBinPath;
                processResult.StartInfo.Arguments = $"-v -d -s \"{baseIdLocation}\" \"{vcdiffPath}\" \"{patchedRomDestination}\"";
                processResult.StartInfo.RedirectStandardOutput = true;
                processResult.Start();
                
                if (processResult != null)
                {
                    await processResult.WaitForExitAsync();

                    switch (processResult.ExitCode)
                    {
                        case 0:
                            //MessageBox by default does not have alignment options. Hack it to look nice to avoid needing to create a new control dialog.
                            var messageSuccess = MessageBoxManager
                                .GetMessageBoxStandard("ROM Patch Successful",
                                    "ROM Created Successfully." + Environment.NewLine + Environment.NewLine
                                       + "Would you like to set the location of this ROM as the " + Environment.NewLine
                                       + "location this launcher will use to launch the game?", ButtonEnum.YesNo);
                            var result = await messageSuccess.ShowAsync();
                            if (result == ButtonResult.Yes)
                            {
                                Configuration.Instance.RomLocation = patchedRomDestination;
                                Configuration.Instance.SaveSettings();
                            }
                            break;
                        default:
                            //MessageBox by default does not have alignment options. Hack it to look nice to avoid needing to create a new control dialog.
                            var messageFailed = MessageBoxManager
                                .GetMessageBoxStandard("ROM Patch Failed",
                                    "ROM Patching Failed." + Environment.NewLine + Environment.NewLine
                                       + "Please ensure that provided paths are valid and that " + Environment.NewLine
                                       + "the Shadow ROM provided is a full size clean rip. " + Environment.NewLine + Environment.NewLine
                                       + "Expected ROM CRC32 for " + patch.SelectedVariant.OriginalGameId + ": " + patch.SelectedVariant.BaseCRC);
                            await messageFailed.ShowAsync();
                            break;
                    }
                }
                else
                {
                    var message = MessageBoxManager
                        .GetMessageBoxStandard("Patching Failed", "ROM Patching failed to launch.");
                    await message.ShowAsync();
                }
            }
            else
            {
                var message = MessageBoxManager
                    .GetMessageBoxStandard("Operation Cancelled", "Operation Cancelled");
                await message.ShowAsync();
            }
        }
        else
        {
            var message = MessageBoxManager
                .GetMessageBoxStandard("Missing Files", 
                    "One or more files needed to complete" + Environment.NewLine +
                        "the ROM patching were missing." + Environment.NewLine + Environment.NewLine +
                        "Please double check directory files.");
            await message.ShowAsync();
        }
    }

    private void UpdateCustomAssets()
    {
        #region UI Display Textures

        if (Directory.Exists(Path.Combine(CommonFilePaths.CustomTexturesPath, "Buttons")))
        {
            Directory.Delete(Path.Combine(CommonFilePaths.CustomTexturesPath, "Buttons"), true);
        }

        var uiButtonOptions = new List<string>();
        uiButtonOptions.Add("Default (GC)");
        Directory.GetDirectories(Path.Combine(CommonFilePaths.SxResourcesCustomTexturesPath, "Buttons")).ToList()
            .ForEach(folderPath => uiButtonOptions.Add(Path.GetFileName(folderPath)));
        
        var buttonAssetsFolder = Configuration.Instance.UiButtonDisplayAssetFolderName;
        if (!string.IsNullOrEmpty(buttonAssetsFolder))
        {
            var newButtonFilePath = Path.Combine(CommonFilePaths.SxResourcesCustomTexturesPath, "Buttons", buttonAssetsFolder);
            if (Directory.Exists(newButtonFilePath))
            {
                var newButtonUiFiles = Directory.EnumerateFiles(newButtonFilePath);

                Directory.CreateDirectory(Path.Combine(CommonFilePaths.CustomTexturesPath, "Buttons"));

                foreach (var buttonFile in newButtonUiFiles)
                {
                    File.Copy(buttonFile, Path.Combine(CommonFilePaths.CustomTexturesPath, "Buttons") + buttonFile.Replace(newButtonFilePath, ""));
                }
            }
        }

        #endregion
        
        #region Gloss Removal

        if (Directory.Exists(Path.Combine(CommonFilePaths.CustomTexturesPath, "GlossAdjustment")))
        {
            Directory.Delete(Path.Combine(CommonFilePaths.CustomTexturesPath, "GlossAdjustment"), true);
        }

        var glossAssetsFolder = 
            Configuration.GlossAdjustmentOptions.Keys.ToArray()[Configuration.Instance.GlossAdjustmentIndex];
        if (!string.IsNullOrEmpty(glossAssetsFolder))
        {
            var removeGlossFilePath = Path.Combine(CommonFilePaths.SxResourcesCustomTexturesPath, "GlossAdjustment", glossAssetsFolder);
            var removeGlossFiles = Directory.EnumerateFiles(removeGlossFilePath);
            
            Directory.CreateDirectory(Path.Combine(CommonFilePaths.CustomTexturesPath, "GlossAdjustment"));
            
            foreach (var removeGlossFile in removeGlossFiles)
            {
                File.Copy(removeGlossFile, Path.Combine(CommonFilePaths.CustomTexturesPath, "GlossAdjustment") + removeGlossFile.Replace(removeGlossFilePath, ""));
            }
        }

        #endregion
    }
}