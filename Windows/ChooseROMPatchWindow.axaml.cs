using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows;

public partial class ChooseROMPatchWindow : Window
{
    //private List<string> availablePatches;
    public ChooseROMPatchWindow()
    {
        InitializeComponent();
        //Scan "Patches" folder for patch data files.
        //This will allow for new patches to be added without
        //releasing a new build of the launcher.
        
        //Temp test options.
        List<PatchData> options = new List<PatchData>
        {
            new PatchData("Shadow SX - 1.0", "sx1.xdelta", "Expects CleanRip of the original Shadow the Hedgehog NTSC ROM. CRC32 - XXXXXXX"),
            new PatchData("Shadow SX - 1.1", "sx1-1.xdelta", "Expects CleanRip of the original Shadow the Hedgehog NTSC ROM. CRC32 - XXXXXXX")
        };
        
        //Apply item list to the combo box.
        PatchComboBox.ItemsSource = options;
        PatchComboBox.SelectionChanged += PatchComboBoxOnSelectionChanged;
        ApplyButton.Click += ApplyButtonOnClick;
        ApplyButton.IsEnabled = false;
        CloseButton.Click += CloseButtonOnClick;
    }

    private void PatchComboBoxOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (PatchComboBox.SelectedItem != null)
        {
            DescriptionTextBlock.Text = ((PatchData)PatchComboBox.SelectedItem).Description;
            ApplyButton.IsEnabled = true;
        }
        else
        {
            DescriptionTextBlock.Text = string.Empty;
            ApplyButton.IsEnabled = false;
        }
    }

    private void ApplyButtonOnClick(object? sender, RoutedEventArgs e)
    {
        Close(PatchComboBox.SelectedItem);
    }
    
    private void CloseButtonOnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}