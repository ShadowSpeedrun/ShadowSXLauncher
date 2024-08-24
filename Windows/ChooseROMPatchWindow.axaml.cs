using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShadowSXLauncher.Classes;

namespace ShadowSXLauncher.Windows;

public partial class ChooseROMPatchWindow : Window
{
    public ChooseROMPatchWindow()
    {
        InitializeComponent();
        
        //Scan "Patches" folder for patch data files.
        //This will allow for new patches to be added without
        //releasing a new build of the launcher.
        List<PatchData> patches = LoadPatchDataXmls();
        
        //Apply item list to the combo box.
        PatchComboBox.ItemsSource = patches;
        PatchComboBox.SelectionChanged += PatchComboBoxOnSelectionChanged;
        VariantComboBox.SelectionChanged += VariantComboBoxOnSelectionChanged;
        ApplyButton.Click += ApplyButtonOnClick;
        ApplyButton.IsEnabled = false;
        CloseButton.Click += CloseButtonOnClick;
    }

    private List<PatchData> LoadPatchDataXmls()
    {
        List<PatchData> patches = new List<PatchData>();
        if (Directory.Exists(CommonFilePaths.SxResourcesPatchDataFolderPath))
        {
            var patchFilePaths = Directory.GetFiles(CommonFilePaths.SxResourcesPatchDataFolderPath, "*.xPatchData",
                SearchOption.AllDirectories);
            foreach (var patchFilePath in patchFilePaths)
            {
                patches.Add(new PatchData(patchFilePath));
            }
        }

        return patches;
    }

    private void PatchComboBoxOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedPatchData = (PatchData)PatchComboBox.SelectedItem;
        VariantComboBox.ItemsSource = selectedPatchData.Variants;
        selectedPatchData.VariantIndex = 0;
        VariantComboBox.SelectedIndex = 0;
        UpdateDescriptionTextBox();
    }
    
    private void VariantComboBoxOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedPatchData = (PatchData)PatchComboBox.SelectedItem;
        selectedPatchData.VariantIndex = VariantComboBox.SelectedIndex;
        UpdateDescriptionTextBox();
    }

    private void UpdateDescriptionTextBox()
    {
        if (PatchComboBox.SelectedItem != null && VariantComboBox.SelectedItem != null)
        {
            var currentPatchData = (PatchData)PatchComboBox.SelectedItem;
            var currentVariant = currentPatchData.SelectedVariant;
            DescriptionTextBox.Text = ((PatchData)PatchComboBox.SelectedItem).Description 
                                      + Environment.NewLine + Environment.NewLine + "Expected CRC32 checksum values"
                                      + Environment.NewLine + "Base ROM - " + currentVariant.BaseCRC
                                      + Environment.NewLine + "Output ROM - " + currentVariant.OutputCRC;
            ApplyButton.IsEnabled = true;
        }
        else
        {
            DescriptionTextBox.Text = string.Empty;
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