using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ShadowSXLauncher.Classes;

public class Configuration
{
    private static Configuration instance = null;

    private XmlDocument configurationXml;

    public bool OnboardingCompleted;
    public string DolphinBinLocation;
    public string DolphinUserLocation;
    public string RomLocation;
    public string UiButtonDisplayAssetFolderName;
    public int GlossAdjustmentIndex;
    
    // TODO: When the 'copy recommended config' option is implemented, we need to use these paths:
    // Flatpak: Flatpak Dir + data/dolphin-emu/ and config/dolphin-emu/
    // Non Flatpak: ~/.local/share/dolphin-emu/ and ~/.config/dolphin-emu
    
    public static readonly Dictionary<string, string> GlossAdjustmentOptions = new()
    {
        {"", "Original"},
        {"ReducedGloss", "Reduced"},
        {"RemovedGloss", "Removed"},
    };

    private Configuration()
    {
        configurationXml = null;
        OnboardingCompleted = false;
        DolphinBinLocation = "";
        DolphinUserLocation = "";
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
            if (node.Name == "OnboardingCompleted")
            {
                OnboardingCompleted = bool.Parse(node.InnerText);
            }
            
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

        var xmlElementOnboardingComplete = configurationXml.CreateElement("OnboardingCompleted");
        xmlElementOnboardingComplete.InnerText = OnboardingCompleted.ToString();
        
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
        mainNode.AppendChild(xmlElementOnboardingComplete);
        mainNode.AppendChild(xmlElementDolphinBinLocation);
        mainNode.AppendChild(xmlElementDolphinUserLocation);
        mainNode.AppendChild(xmlElementRomLocation);
        mainNode.AppendChild(xmlElementUiButtonDisplayIndex);
        mainNode.AppendChild(xmlElementGlossAdjustment);
        
        configurationXml.Save(CommonFilePaths.ConfigFileLocation);
    }

    public void SetDolphinPathsForFlatpakAndPortable()
    {
        if (OperatingSystem.IsWindows())
        {
            DolphinBinLocation = Path.Combine(CommonFilePaths.AppStart, "Dolphin-x64");
            DolphinUserLocation = Path.Combine(DolphinBinLocation, "User");
        } 
        else if (OperatingSystem.IsLinux())
        {
            DolphinBinLocation = "flatpak";
            DolphinUserLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.var/app/org.DolphinEmu.dolphin-emu/data/dolphin-emu/";
        }
        SaveSettings();
    }
    
    public void SetDolphinPathsForNativeAndGlobal()
    {
        if (OperatingSystem.IsWindows())
        {
            DolphinBinLocation = "SET ME"; // There is no such thing as a Global Dolphin for Windows yet, so make it junk for the user to be prompted if they forget to set it
            DolphinUserLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator");
        } 
        else if (OperatingSystem.IsLinux())
        {
            DolphinBinLocation = "/usr/bin";
            DolphinUserLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.local/share/dolphin-emu/";
        }
        SaveSettings();
    }
}