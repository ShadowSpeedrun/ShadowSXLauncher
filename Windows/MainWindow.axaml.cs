using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows;

public partial class MainWindow : Window
{
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
        
        if (string.IsNullOrEmpty(Configuration.Instance.DolphinBinLocation))
        {
            await OpenSetDolphinBinDialog();
        }
        
        if (string.IsNullOrEmpty(Configuration.Instance.DolphinUserLocation))
        {
            await OpenSetDolphinUserDialog();
        }

        if (!string.IsNullOrEmpty(Configuration.Instance.DolphinBinLocation))
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
                    var message = MessageBoxManager
                        .GetMessageBoxStandard("ROM not found", "ROM file not found. Please provide ROM location again.");
                    var result = await message.ShowAsync();
                    //OpenRomDialog();
                }

                //At this point assume there is a correct ROM. Technically nothing stopping a user from
                //choosing whatever ROM they want to launch, but trying to account for that without additional
                //annoying checks and processes is not worth it.

                UpdateCustomAssets();

                //Double check the .exe is found before attempting to run it.
                if (File.Exists(Path.Combine(CommonFilePaths.DolphinBinPath, CommonFilePaths.DolphinBinFile)))
                {
                    Process.Start("\"" + Path.Combine(CommonFilePaths.DolphinBinPath, CommonFilePaths.DolphinBinFile) + "\"",
                        @" -b " + "\"" + Configuration.Instance.RomLocation + "\"");
                    Close();
                }
                else
                {
                    var message = MessageBoxManager
                        .GetMessageBoxStandard("Dolphin not found", "Could not find Dolphin. Please double check directory files.");
                    var result = await message.ShowAsync();
                }
            }
        }
        EnableButtons(true);
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
        Configuration.Instance.SaveSettings();
    }
    
    private async Task<string[]?> SetOpenFilePath(string title, FileDialogFilter filter)
    {
        var ofd = new OpenFileDialog();
        ofd.Title = title;
        ofd.Filters = new List<FileDialogFilter>() { filter };
        ofd.Directory = CommonFilePaths.AppStart;
        ofd.AllowMultiple = false;
        return await ofd.ShowAsync(this);
    }
    
    private async Task<string?> SetSaveFilePath(string title, FileDialogFilter filter)
    {
        var sfd = new SaveFileDialog();
        sfd.Title = title;
        sfd.Filters = new List<FileDialogFilter>() { filter };
        sfd.Directory = CommonFilePaths.AppStart;
        return await sfd.ShowAsync(this);
    }

    private async Task OpenSetDolphinBinDialog()
    {
        var result = await SetFolderPath("Set Path to Dolphin Executable");
        Configuration.Instance.DolphinBinLocation = String.IsNullOrEmpty(result) ? "" : result;
        Configuration.Instance.SaveSettings();
    }
    
    private async Task OpenSetDolphinUserDialog()
    {
        var result = await SetFolderPath("Set Path to Dolphin User Folder");
        Configuration.Instance.DolphinUserLocation = String.IsNullOrEmpty(result) ? "" : result;
        Configuration.Instance.SaveSettings();
    }

    private async Task<string?> SetFolderPath(string title)
    {
        var ofd = new OpenFolderDialog();
        ofd.Title = title;
        ofd.Directory = CommonFilePaths.AppStart; 
        return await ofd.ShowAsync(this);
    }

    private void OpenGameLocationButtonPressed(object? sender, RoutedEventArgs e)
    {
        OpenFolder(CommonFilePaths.AppStart);
    }

    private async void OnSaveFileButtonPressed(object? sender, RoutedEventArgs e)
    {
        var success = OpenFolder(CommonFilePaths.SavePath);
        if (!success)
        {
            var message = MessageBoxManager
                .GetMessageBoxStandard("Save folder not found", "Please launch game to generate the save directory.");
            var result = await message.ShowAsync();
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
                FileName = CommonFilePaths.GetExplorerPath,
                Arguments = folderPath,
                UseShellExecute = true
            };
            Process.Start(psi);
            return true;
        }

        return false;
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
        var patchScriptPath = CommonFilePaths.PatchingScriptPath;
                
        var allPatchFilesFound = File.Exists(xdeltaBinPath);
        allPatchFilesFound &= File.Exists(vcdiffPath);
        allPatchFilesFound &= File.Exists(patchScriptPath);
        
        if (allPatchFilesFound)
        {
            var baseIdLocation = "";
            var patchedRomDestination = "";

            var resultBaseId = await SetOpenFilePath("Select Original ROM (" + patch.SelectedVariant.OriginalGameId + ")", 
                new FileDialogFilter()
                {
                    Name = "ROM File",
                    Extensions = new List<string>() {"iso"}
                });
        
            baseIdLocation = resultBaseId == null ? "" : resultBaseId.First();

            if (!string.IsNullOrEmpty(baseIdLocation))
            {
                var resultNewId = await SetSaveFilePath("Save Patched ROM (" + patch.SelectedVariant.NewGameId + ")", 
                    new FileDialogFilter()
                    {
                        Name = "ROM File",
                        Extensions = new List<string>() {"iso"}
                    });
                patchedRomDestination = resultNewId ?? "";
            }
            else
            {
                var message = MessageBoxManager
                    .GetMessageBoxStandard("Operation Cancelled", "Operation Cancelled.");
                await message.ShowAsync();
                return;
            }
            
            //We can assume that gupe8pLocation is not empty or null. 
            if (!string.IsNullOrEmpty(patchedRomDestination))
            {
                var batArguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"", baseIdLocation, patchedRomDestination,
                        xdeltaBinPath, vcdiffPath);
                    
                var processResult = Process.Start(patchScriptPath, batArguments);
                if (processResult != null)
                {
                    processResult.WaitForExit();

                    switch (processResult.ExitCode)
                    {
                        case 0:
                            //MessageBox by default doesn't have alignment options. Hack it to look nice to avoid needing to create a new control dialog.
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
                            //MessageBox by default doesnt have alignment options. Hack it to look nice to avoid needing to create a new control dialog.
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

        if (Directory.Exists(CommonFilePaths.CustomTexturesPath + @"\Buttons"))
        {
            Directory.Delete(CommonFilePaths.CustomTexturesPath + @"\Buttons", true);
        }

        //TODO: Update to use folder name instead of index as the number of folders will now be dynamic.
        var uiButtonOptions = new List<string>();
        uiButtonOptions.Add("Default (GC)");
        Directory.GetDirectories(CommonFilePaths.SxResourcesCustomTexturesPath + @"\Buttons\").ToList()
            .ForEach(folderPath => uiButtonOptions.Add(Path.GetFileName(folderPath)));
        
        var buttonAssetsFolder = uiButtonOptions[Configuration.Instance.UiButtonDisplayIndex];
        if (!string.IsNullOrEmpty(buttonAssetsFolder))
        {
            var newButtonFilePath = CommonFilePaths.SxResourcesCustomTexturesPath + @"\Buttons\" + buttonAssetsFolder;
            if (Directory.Exists(newButtonFilePath))
            {
                var newButtonUiFiles = Directory.EnumerateFiles(newButtonFilePath);

                Directory.CreateDirectory(CommonFilePaths.CustomTexturesPath + @"\Buttons");

                foreach (var buttonFile in newButtonUiFiles)
                {
                    File.Copy(buttonFile,
                        CommonFilePaths.CustomTexturesPath + @"\Buttons" + buttonFile.Replace(newButtonFilePath, ""));
                }
            }
        }

        #endregion
        
        #region Gloss Removal

        if (Directory.Exists(CommonFilePaths.CustomTexturesPath + @"\GlossAdjustment"))
        {
            Directory.Delete(CommonFilePaths.CustomTexturesPath + @"\GlossAdjustment", true);
        }

        var glossAssetsFolder = 
            Configuration.GlossAdjustmentOptions.Keys.ToArray()[Configuration.Instance.GlossAdjustmentIndex];
        if (!string.IsNullOrEmpty(glossAssetsFolder))
        {
            var removeGlossFilePath = CommonFilePaths.SxResourcesCustomTexturesPath + @"\GlossAdjustment\" + glossAssetsFolder;
            var removeGlossFiles = Directory.EnumerateFiles(removeGlossFilePath);
            
            Directory.CreateDirectory(CommonFilePaths.CustomTexturesPath + @"\GlossAdjustment");
            
            foreach (var removeGlossFile in removeGlossFiles)
            {
                File.Copy(removeGlossFile, CommonFilePaths.CustomTexturesPath + @"\GlossAdjustment" + removeGlossFile.Replace(removeGlossFilePath, ""));
            }
        }

        #endregion
    }
}