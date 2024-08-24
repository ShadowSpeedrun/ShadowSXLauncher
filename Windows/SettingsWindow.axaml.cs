using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
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

    public TextBox DolphinLocationTextBox
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return DolphinLocationWindowsTextBox;
            }
            if (OperatingSystem.IsLinux())
            {
                return DolphinLocationLinuxTextBox;
            }

            return null;
        }
    }
    
    public Button SetDolphinLocationButton
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return SetDolphinLocationWindowsButton;
            }
            if (OperatingSystem.IsLinux())
            {
                return SetDolphinLocationLinuxButton;
            }

            return null;
        }
    }

    private void InitializeOptions()
    {
        RomLocationTextBox.Text = Configuration.Instance.RomLocation;
        DolphinLocationTextBox.Text = Configuration.Instance.DolphinLocation;
        InitializeUiButtonOptions();
        GlossLevelComboBox.SelectedIndex = Configuration.Instance.GlossAdjustmentIndex;
        RegisterEvents();
    }

    private void InitializeUiButtonOptions()
    {
        uiButtonOptions = new List<string>();
        uiButtonOptions.Add("Default (GC)");
        Directory.GetDirectories(Path.Combine(CommonFilePaths.SxResourcesCustomTexturesPath,"Buttons")).ToList()
            .ForEach(folderPath => uiButtonOptions.Add(Path.GetFileName(folderPath)));
        CustomButtonComboBox.ItemsSource = uiButtonOptions;
        CustomButtonComboBox.SelectedIndex = Configuration.Instance.UiButtonDisplayIndex;
    }

    private void RegisterEvents()
    {
        SetRomLocationButton.Click += SetRomLocationButtonOnClick;
        SetDolphinLocationButton.Click += SetDolphinLocationButtonOnClick;
        OpenDolphinButton.Click += OpenDolphinButtonOnClick;
        CustomShadowColorButton.Click += CustomShadowColorButtonOnClick;
        SaveSettingsButton.Click += SaveSettingsButtonOnClick;
    }

    /// <summary>
    /// Needed to lock and unlock the UI when the current action doesn't do it for use (i.e. Dialogs)
    /// </summary>
    /// <param name="enable"></param>
    private void EnableUI(bool enable)
    {
        SetRomLocationButton.IsEnabled = enable;
        SetDolphinLocationButton.IsEnabled = enable;
        CustomButtonComboBox.IsEnabled = enable;
        GlossLevelComboBox.IsEnabled = enable;
        OpenDolphinButton.IsEnabled = enable;
        CustomShadowColorButton.IsEnabled = enable;
        SaveSettingsButton.IsEnabled = enable;
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

    private async void SetDolphinLocationButtonOnClick(object? sender, RoutedEventArgs e)
    {
        EnableUI(false);
        
        var result = await SetFolderPath("Set Path to Dolphin");
        DolphinLocationTextBox.Text = !string.IsNullOrEmpty(result) ? result : string.Empty;
        
        EnableUI(true);
    }

    private async void OpenDolphinButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (Directory.Exists(CommonFilePaths.DolphinPath))
        {
            Process.Start(CommonFilePaths.DolphinPath + @"\Dolphin.exe");
        }
        else
        {
            var message = MessageBoxManager
                .GetMessageBoxStandard("Operation Cancelled", "Could not find dolphin.exe. Please double check directory files.");
            await message.ShowAsync();
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
        Configuration.Instance.DolphinLocation = !string.IsNullOrEmpty(DolphinLocationTextBox.Text) ? DolphinLocationTextBox.Text : string.Empty;
        Configuration.Instance.UiButtonDisplayIndex = CustomButtonComboBox.SelectedIndex;
        Configuration.Instance.GlossAdjustmentIndex = GlossLevelComboBox.SelectedIndex;
        Configuration.Instance.SaveSettings();
    }
}