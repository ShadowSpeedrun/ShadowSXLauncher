using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Avalonia;
using Avalonia.Platform;

namespace ShadowSXLauncher.Classes;

public class Configuration
{
    private static Configuration instance = null;

    private XmlDocument configurationXml;

    public string DolphinBinLocation;
    public string DolphinUserLocation;
    public string RomLocation;
    public string UiButtonDisplayAssetFolderName;
    public int GlossAdjustmentIndex;
    
    public static readonly Dictionary<string, string> GlossAdjustmentOptions = new Dictionary<string, string>()
    {
        {"", "Original"},
        {"ReducedGloss", "Reduced"},
        {"RemovedGloss", "Removed"},
    };

    private Configuration()
    {
        configurationXml = null;

        // data/dolphin-emu/
        // config/dolphin-emu/
        // sudo flatpak override org.DolphinEmu.dolphin-emu --filesystem host
        // flatpak run
        
        DolphinBinLocation = OperatingSystem.IsLinux() ? "flatpak run org.DolphinEmu.dolphin-emu" : "";
        DolphinUserLocation = OperatingSystem.IsLinux() ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.var/app/org.DolphinEmu.dolphin-emu/data/dolphin-emu/"
            : OperatingSystem.IsMacOS() ? "mac path"
            : "";
        RomLocation = "";
        UiButtonDisplayAssetFolderName = "";
        GlossAdjustmentIndex = 0;
    }

    public static Configuration Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Configuration();
            }
            return instance;
        }
    }

    /// <summary>
    /// Load data from XML file to commands.
    /// </summary>
    public void LoadSettings()
    {
        if (configurationXml == null)
        {
            configurationXml = new XmlDocument();
            try
            {
                configurationXml.Load(CommonFilePaths.ConfigFileLocation);
            }
            catch(IOException e)
            {
                // Create the file if it does not exist.
                SaveSettings();
            }
        }

        foreach (XmlElement node in configurationXml.DocumentElement)
        {
            if (node.Name == "DolphinBinLocation")
            {
                DolphinBinLocation = node.InnerText;
            }
            
            if (node.Name == "DolphinUserLocation")
            {
                DolphinUserLocation = node.InnerText;
            }
            
            if (node.Name == "RomLocation")
            {
                RomLocation = node.InnerText;
            }

            if (node.Name == "UiButtonDisplayAssetFolderName")
            {
                UiButtonDisplayAssetFolderName = node.InnerText;
            }
            
            if (node.Name == "GlossAdjustment")
            {
                GlossAdjustmentIndex = int.Parse(node.InnerText);
            }
        }
    }

    public void SaveSettings()
    {
        configurationXml = new XmlDocument();

        var mainNode = configurationXml.CreateElement("Settings");

        var xmlElementDolphinBinLocation = configurationXml.CreateElement("DolphinBinLocation");
        xmlElementDolphinBinLocation.InnerText = DolphinBinLocation;
        
        var xmlElementDolphinUserLocation = configurationXml.CreateElement("DolphinUserLocation");
        xmlElementDolphinUserLocation.InnerText = DolphinUserLocation;
        
        var xmlElementRomLocation = configurationXml.CreateElement("RomLocation");
        xmlElementRomLocation.InnerText = RomLocation;

        var xmlElementUiButtonDisplayIndex = configurationXml.CreateElement("UiButtonDisplayAssetFolderName");
        xmlElementUiButtonDisplayIndex.InnerText = UiButtonDisplayAssetFolderName;
        
        var xmlElementGlossAdjustment = configurationXml.CreateElement("GlossAdjustment");
        xmlElementGlossAdjustment.InnerText = GlossAdjustmentIndex.ToString();

        configurationXml.AppendChild(mainNode);
        mainNode.AppendChild(xmlElementDolphinBinLocation);
        mainNode.AppendChild(xmlElementDolphinUserLocation);
        mainNode.AppendChild(xmlElementRomLocation);
        mainNode.AppendChild(xmlElementUiButtonDisplayIndex);
        mainNode.AppendChild(xmlElementGlossAdjustment);
        
        configurationXml.Save(CommonFilePaths.ConfigFileLocation);
    }
}