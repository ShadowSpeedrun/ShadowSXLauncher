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
    //private List<string> availablePatches;
    public ChooseROMPatchWindow()
    {
        InitializeComponent();
        //Scan "Patches" folder for patch data files.
        //This will allow for new patches to be added without
        //releasing a new build of the launcher.

        List<PatchData> patches = LoadPatchDataXmls();
        
        //Temp test options.
        List<PatchData> options = new List<PatchData>
        {
            new PatchData("Shadow SX - 1.0", "sx1.xdelta", "Expects CleanRip of the original Shadow the Hedgehog NTSC ROM. CRC32 - XXXXXXX"),
            new PatchData("Shadow SX - 1.1", "sx1-1.xdelta", "Expects CleanRip of the original Shadow the Hedgehog NTSC ROM. CRC32 - XXXXXXX")
        };
        
        //Apply item list to the combo box.
        PatchComboBox.ItemsSource = patches;
        PatchComboBox.SelectionChanged += PatchComboBoxOnSelectionChanged;
        ApplyButton.Click += ApplyButtonOnClick;
        ApplyButton.IsEnabled = false;
        CloseButton.Click += CloseButtonOnClick;
    }

    private List<PatchData> LoadPatchDataXmls()
    {
        List<PatchData> patches = new List<PatchData>();
        var patchFilePaths = Directory.GetFiles(CommonFilePaths.SxResourcesPatchDataFolderPath, "*.xPatchData");
        foreach (var patchFilePath in patchFilePaths)
        {
            var patchDataXml = new XmlDocument();
            patchDataXml.Load(patchFilePath);

            //TODO: Look into using XmlSerializer
            string? patchDisplayName = null;
            string? patchFileName = null;
            string? patchDescription = null;
            foreach (XmlElement node in patchDataXml.DocumentElement)
            {
                if (node.Name == "DisplayName")
                {
                    patchDisplayName = node.InnerText;
                }

                if (node.Name == "PatchFileName")
                {
                    patchFileName = node.InnerText;
                }

                if (node.Name == "Description")
                {
                    patchDescription = node.InnerText;
                }
            }
            
            if (patchDisplayName != null && patchFileName != null && patchDescription != null)
            {
                patches.Add(new PatchData(patchDisplayName, patchFileName, patchDescription));
            }
        }

        return patches;
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