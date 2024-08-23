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
    //private List<string> availablePatches;
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
                var patchDataXml = new XmlDocument();
                patchDataXml.Load(patchFilePath);

                //TODO: Look into using XmlSerializer
                string? patchDisplayName = null;
                string? patchFileName = null;
                string? patchDescription = null;
                string? patchBaseID = null;
                string? patchNewID = null;
                string? patchBaseCRC = null;

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
                        patchDescription = node.InnerXml.Replace("<br />", Environment.NewLine);
                    }

                    if (node.Name == "OriginalGameID")
                    {
                        patchBaseID = node.InnerText;
                    }

                    if (node.Name == "NewGameID")
                    {
                        patchNewID = node.InnerText;
                    }

                    if (node.Name == "ExpectedBaseCRC")
                    {
                        patchBaseCRC = node.InnerText;
                    }
                }

                if (patchDisplayName != null && patchFileName != null && patchDescription != null)
                    if (patchBaseID != null && patchNewID != null && patchBaseCRC != null)
                    {
                        patches.Add(new PatchData(patchDisplayName, patchFileName, patchDescription,
                            patchBaseID, patchNewID, patchBaseCRC,
                            Path.GetDirectoryName(patchFilePath)));
                    }
            }
        }

        return patches;
    }

    private void PatchComboBoxOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (PatchComboBox.SelectedItem != null)
        {
            DescriptionTextBox.Text = ((PatchData)PatchComboBox.SelectedItem).Description 
                                      + Environment.NewLine + Environment.NewLine + "Expected CRC32 checksum values" +
                                      Environment.NewLine + "Base ROM - " + ((PatchData)PatchComboBox.SelectedItem).BaseCRC
                                      + Environment.NewLine + "Output ROM - " + ((PatchData)PatchComboBox.SelectedItem).BaseCRC;
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