using System.IO;
using System.Xml;

namespace ShadowSXLauncher.Classes;

public class PatchData
{
    private string patchDisplayName;
    private string patchFileName;
    private string patchDescription;
    
    /// <summary>
    /// Game ID to display when picking the file needed for the patch.
    /// </summary>
    private string orginalGameId;
    
    /// <summary>
    /// Game ID that the final ROM output will have.
    /// </summary>
    private string newGameId;

    /// <summary>
    /// Expected ROM CRC to display in the description / error message.
    /// </summary>
    private string expectedBaseCRC;

    /// <summary>
    /// Location of where the xPatchData file was found.
    /// </summary>
    private string directoryLocation;

    public string OriginalGameId => orginalGameId;

    public string NewGameId => newGameId;

    public string PatchFilePath => Path.Combine(directoryLocation, patchFileName);
    
    public string Description => patchDescription;

    public string BaseCRC => expectedBaseCRC;
    
    public PatchData(string PatchDisplayName, string PatchFileName, string PatchDescription,
        string patchBaseID, string patchNewID, string patchBaseCRC, string directoryLocation)
    {
        patchDisplayName = PatchDisplayName;
        patchFileName = PatchFileName;
        patchDescription = PatchDescription;
        orginalGameId = patchBaseID;
        newGameId = patchNewID;
        expectedBaseCRC = patchBaseCRC;
        this.directoryLocation = directoryLocation;
    }

    public override string ToString()
    {
        return patchDisplayName;
    }
}