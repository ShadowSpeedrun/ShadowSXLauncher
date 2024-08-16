using System.IO;
using System.Xml;

namespace ShadowSXLauncher.Classes;

public class PatchData
{
    private string patchDisplayName;
    private string patchFileName;
    private string patchDescription;

    public string Description
    {
        get
        {
            return patchDescription; 
        }
    }

    public PatchData(XmlDocument PatchDataXml)
    {
        patchDisplayName = PatchDataXml.DocumentElement.GetAttribute("displayName");
        patchFileName = PatchDataXml.DocumentElement.GetAttribute("patchFileName");
        patchDescription = PatchDataXml.DocumentElement.GetAttribute("patchDescription");
    }
    
    public PatchData(string PatchDisplayName, string PatchFileName, string PatchDescription)
    {
        patchDisplayName = PatchDisplayName;
        patchFileName = PatchFileName;
        patchDescription = PatchDescription;
    }

    public override string ToString()
    {
        return patchDisplayName;
    }
}