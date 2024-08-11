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

    public string DolphinLocation;
    public string RomLocation;
    public int UiButtonDisplayIndex;
    public int GlossAdjustmentIndex;
    
    public static string AppStart
    {
        get
        {
            return AppContext.BaseDirectory.Replace("net6.0\\", "").Replace("net6.0/", "");
        }
    }
    
    private string fileLocation = Path.Combine(AppStart, "Config.xml");

    public static readonly Dictionary<string, string> UiButtonStyles = new Dictionary<string, string>()
    {
        {"", "Default (GC)"},
        {"Xbox", "Xbox"},
        {"XboxOne", "Xbox One"},
        {"SteamDeck", "Steam Deck"},
        {"PS2", "PS2"},
        {"PS3", "PS3"},
    };
    
    public static readonly Dictionary<string, string> GlossAdjustmentOptions = new Dictionary<string, string>()
    {
        {"", "Original"},
        {"ReducedGloss", "Reduced"},
        {"RemovedGloss", "Removed"},
    };

    private Configuration()
    {
        configurationXml = null;

        DolphinLocation = "";
        RomLocation = "";
        UiButtonDisplayIndex = 0;
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
                configurationXml.Load(fileLocation);
            }
#pragma warning disable CS0168
            catch(IOException e)
#pragma warning restore CS0168
            {
                //Create the file if it doesnt exist.
                SaveSettings();
            }
        }

        foreach (XmlElement node in configurationXml.DocumentElement)
        {
            if (node.Name == "DolphinLocation")
            {
                DolphinLocation = node.InnerText;
            }
            
            if (node.Name == "RomLocation")
            {
                RomLocation = node.InnerText;
            }

            if (node.Name == "UiButtonDisplayIndex")
            {
                UiButtonDisplayIndex = int.Parse(node.InnerText);
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

        var xmlElementDolphinLocation = configurationXml.CreateElement("DolphinLocation");
        xmlElementDolphinLocation.InnerText = DolphinLocation;
        
        var xmlElementRomLocation = configurationXml.CreateElement("RomLocation");
        xmlElementRomLocation.InnerText = RomLocation;

        var xmlElementUiButtonDisplayIndex = configurationXml.CreateElement("UiButtonDisplayIndex");
        xmlElementUiButtonDisplayIndex.InnerText = UiButtonDisplayIndex.ToString();
        
        var xmlElementGlossAdjustment = configurationXml.CreateElement("GlossAdjustment");
        xmlElementGlossAdjustment.InnerText = GlossAdjustmentIndex.ToString();

        configurationXml.AppendChild(mainNode);
        mainNode.AppendChild(xmlElementDolphinLocation);
        mainNode.AppendChild(xmlElementRomLocation);
        mainNode.AppendChild(xmlElementUiButtonDisplayIndex);
        mainNode.AppendChild(xmlElementGlossAdjustment);
        
        configurationXml.Save(fileLocation);
    }
}