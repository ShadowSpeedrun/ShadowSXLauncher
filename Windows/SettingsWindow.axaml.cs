using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows;

public partial class SettingsWindow : Window
{
    private List<string> uiButtonOptions = new List<string>();
    
    public SettingsWindow()
    {
        InitializeComponent();
        
#if DEBUG
        this.AttachDevTools();
#endif
        
        Configuration.Instance.LoadSettings();
        InitializeOptions();
    }

    private void InitializeOptions()
    {
        RomLocationTextBox.Text = Configuration.Instance.RomLocation;
        DolphinBinLocationTextBox.Text = Configuration.Instance.DolphinBinLocation;
        DolphinUserLocationTextBox.Text = Configuration.Instance.DolphinUserLocation;
        InitializeUiButtonOptions();
        GlossLevelComboBox.SelectedIndex = Configuration.Instance.GlossAdjustmentIndex;

        FlatpakMessageBlocker.IsVisible = OperatingSystem.IsLinux();
        FlatpakMessageText.IsVisible = OperatingSystem.IsLinux();
        
        RegisterEvents();
    }

    private void InitializeUiButtonOptions()
    {
        uiButtonOptions = new List<string>();
        uiButtonOptions.Add("Default (GC)");
        Directory.GetDirectories(Path.Combine(CommonFilePaths.SxResourcesCustomTexturesPath,"Buttons")).ToList()
            .ForEach(folderPath => uiButtonOptions.Add(Path.GetFileName(folderPath)));
        CustomButtonComboBox.ItemsSource = uiButtonOptions;
        var initialUiButtonIndex = uiButtonOptions.FindIndex(u=> u == Configuration.Instance.UiButtonDisplayAssetFolderName);
        CustomButtonComboBox.SelectedIndex = initialUiButtonIndex != -1 ? initialUiButtonIndex : 0;
    }

    private void RegisterEvents()
    {
        SetRomLocationButton.Click += SetRomLocationButtonOnClick;
        SetDolphinBinLocationButton.Click += SetDolphinBinLocationButtonOnClick;
        SetDolphinUserLocationButton.Click += SetDolphinUserLocationButtonOnClick;
        OpenDolphinButton.Click += OpenDolphinButtonOnClick;
        CustomShadowColorButton.Click += CustomShadowColorButtonOnClick;
        SaveSettingsButton.Click += SaveSettingsButtonOnClick;
        HighRezUiFixButton.Click += HighRezUiFixButtonOnClick;
    }

    /// <summary>
    /// Needed to lock and unlock the UI when the current action doesn't do it for use (i.e. Dialogs)
    /// </summary>
    /// <param name="enable"></param>
    private void EnableUI(bool enable)
    {
        SetRomLocationButton.IsEnabled = enable;
        SetDolphinBinLocationButton.IsEnabled = enable;
        SetDolphinUserLocationButton.IsEnabled = enable;
        CustomButtonComboBox.IsEnabled = enable;
        GlossLevelComboBox.IsEnabled = enable;
        OpenDolphinButton.IsEnabled = enable && !string.IsNullOrEmpty(Configuration.Instance.DolphinBinLocation);
        CustomShadowColorButton.IsEnabled = enable && !string.IsNullOrEmpty(Configuration.Instance.DolphinUserLocation);
        SaveSettingsButton.IsEnabled = enable;
        HighRezUiFixButton.IsEnabled = enable && !string.IsNullOrEmpty(Configuration.Instance.DolphinUserLocation);
    }

    private async Task<string[]?> GetFilePath(string title, FileDialogFilter filter)
    {
        var ofd = new OpenFileDialog();
        ofd.Title = title;
        ofd.Filters = new List<FileDialogFilter>() { filter };
        ofd.Directory = CommonFilePaths.AppStart;
        ofd.AllowMultiple = false;
        return await ofd.ShowAsync(this);
    }
    
    private async Task<string?> SetFolderPath(string title)
    {
        var ofd = new OpenFolderDialog();
        ofd.Title = title;
        ofd.Directory = CommonFilePaths.AppStart; 
        return await ofd.ShowAsync(this);
    }

    private async void SetRomLocationButtonOnClick(object? sender, RoutedEventArgs e)
    {
        EnableUI(false);
        
        var result = await GetFilePath("Set SX ROM Location", new FileDialogFilter()
        {
            Name = "ROM File",
            Extensions = new List<string>() {"iso"}
        });
        RomLocationTextBox.Text = result != null && result.Length > 0 && !string.IsNullOrEmpty(result[0]) ? result[0] : string.Empty;
        
        EnableUI(true);
    }

    private async void SetDolphinBinLocationButtonOnClick(object? sender, RoutedEventArgs e)
    {
        EnableUI(false);
        
        var result = await SetFolderPath("Set Path to Dolphin Executable");
        DolphinBinLocationTextBox.Text = !string.IsNullOrEmpty(result) ? result : string.Empty;
        Configuration.Instance.DolphinBinLocation = result;
        Configuration.Instance.SaveSettings();
        
        EnableUI(true);
    }
    
    private async void SetDolphinUserLocationButtonOnClick(object? sender, RoutedEventArgs e)
    {
        EnableUI(false);
        
        var result = await SetFolderPath("Set Path to Dolphin User Folder");
        DolphinUserLocationTextBox.Text = !string.IsNullOrEmpty(result) ? result : string.Empty;
        Configuration.Instance.DolphinUserLocation = result;
        Configuration.Instance.SaveSettings();
        
        EnableUI(true);
    }

    private async void OpenDolphinButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (OperatingSystem.IsWindows())
        {
            if (Directory.Exists(CommonFilePaths.DolphinBinPath))
            {
                Process.Start(Path.Combine(CommonFilePaths.DolphinBinPath, CommonFilePaths.DolphinBinFile));
            }
            else
            {
                var message = MessageBoxManager
                    .GetMessageBoxStandard("Operation Cancelled",
                        "Could not find " + CommonFilePaths.DolphinBinFile + ". Please double check directory files.");
                await message.ShowAsync();
            }
        }
        else if (OperatingSystem.IsLinux())
        {
            var checkFlatpak = new Process();
            checkFlatpak.StartInfo.FileName = "which";
            checkFlatpak.StartInfo.Arguments = "flatpak";
            checkFlatpak.StartInfo.RedirectStandardOutput = true;
            checkFlatpak.Start();
            await checkFlatpak.WaitForExitAsync();
            var flatpakDirectory = await checkFlatpak.StandardOutput.ReadToEndAsync();
            if (flatpakDirectory.Length == 0)
            {
                var message = MessageBoxManager
                    .GetMessageBoxStandard("Operation Cancelled",
                        "Flatpak not detected.\nPlease check Flatpak is installed.");
                await message.ShowAsync();
                return;
            }
            Process.Start(flatpakDirectory.Trim('\n'), "run org.DolphinEmu.dolphin-emu");
        }
        else if (OperatingSystem.IsMacOS())
        {
            // TODO
        }
    }

    private void CustomShadowColorButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var shadowColorDialog = new ShadowColorAdjuster();
        shadowColorDialog.ShowDialog(this);
    }
    
    private void SaveSettingsButtonOnClick(object? sender, RoutedEventArgs e)
    {
        Configuration.Instance.RomLocation = !string.IsNullOrEmpty(RomLocationTextBox.Text) ? RomLocationTextBox.Text : string.Empty;
        Configuration.Instance.DolphinBinLocation = !string.IsNullOrEmpty(DolphinBinLocationTextBox.Text) ? DolphinBinLocationTextBox.Text : string.Empty;
        Configuration.Instance.DolphinUserLocation = !string.IsNullOrEmpty(DolphinUserLocationTextBox.Text) ? DolphinUserLocationTextBox.Text : string.Empty;
        Configuration.Instance.UiButtonDisplayAssetFolderName = CustomButtonComboBox.SelectedIndex != 0 ? CustomButtonComboBox.SelectedItem.ToString() : string.Empty;
        Configuration.Instance.GlossAdjustmentIndex = GlossLevelComboBox.SelectedIndex;
        Configuration.Instance.SaveSettings();
    }
    
    private void HighRezUiFixButtonOnClick(object? sender, RoutedEventArgs e)
    {
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
}