using System.IO;
using System.Xml;

namespace ShadowSXLauncher.Classes;

public class xShadowColorsFile
{
    private XmlDocument xml;
    public string MainColorHexString = "0x311C10";
    public string AccentColorHexString = "0xDE0000";
    
    public void LoadSettings(string filePath)
    {
        if (xml == null)
        {
            xml = new XmlDocument();
            try
            {
                xml.Load(filePath);
            }
            catch(IOException e)
            {
                // Create the file if it does not exist.
                SaveSettings(filePath);
            }
        }

        foreach (XmlElement node in xml.DocumentElement)
        {
            if (node.Name == "MainColor")
            {
                MainColorHexString = node.InnerText;
            }
            
            if (node.Name == "AccentColor")
            {
                AccentColorHexString = node.InnerText;
            }
        }
    }
    
    public void SaveSettings(string savePath)
    {
        xml = new XmlDocument();

        var mainNode = xml.CreateElement("Settings");

        var xmlElementOnboardingComplete = xml.CreateElement("MainColor");
        xmlElementOnboardingComplete.InnerText = MainColorHexString;
        
        var xmlElementDolphinBinLocation = xml.CreateElement("AccentColor");
        xmlElementDolphinBinLocation.InnerText = AccentColorHexString;

        xml.AppendChild(mainNode);
        mainNode.AppendChild(xmlElementOnboardingComplete);
        mainNode.AppendChild(xmlElementDolphinBinLocation);
        
        xml.Save(savePath);
    }
}