using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ShadowSXLauncher.Classes;

public class PatchData
{
    /// <summary>
    /// 
    /// </summary>
    private string patchDisplayName;
    
    /// <summary>
    /// 
    /// </summary>
    private string patchDescription;
    
    /// <summary>
    /// Location of where the xPatchData file was found.
    /// </summary>
    private string directoryLocation;

    /// <summary>
    /// 
    /// </summary>
    public int VariantIndex = 0;
    
    public class Variant
    {
        /// <summary>
        /// 
        /// </summary>
        private string variantName;
        
        /// <summary>
        /// 
        /// </summary>
        private string patchFileName;
        
        /// <summary>
        /// Game ID to display when picking the file needed for the patch.
        /// </summary>
        private string originalGameId;
    
        /// <summary>
        /// Game ID that the final ROM output will have.
        /// </summary>
        private string newGameId;

        /// <summary>
        /// Expected ROM CRC to display in the description / error message.
        /// </summary>
        private string expectedBaseCRC;
        
        /// <summary>
        /// Expected ROM CRC to display in the description / error message.
        /// </summary>
        private string expectedOutputCRC;

        public string PatchFileName => patchFileName;
        
        public string OriginalGameId => originalGameId;

        public string NewGameId => newGameId;
        
        public string BaseCRC => expectedBaseCRC;
        
        public string OutputCRC => expectedOutputCRC;

        public Variant(string variantName, string patchFileName, string originalGameId, string newGameId, string baseCRC, string outputCRC)
        {
            this.variantName = variantName;
            this.patchFileName = patchFileName;
            this.originalGameId = originalGameId;
            this.newGameId = newGameId;
            expectedBaseCRC = baseCRC;
            expectedOutputCRC = outputCRC;
        }

        public override string ToString()
        {
            return variantName;
        }
    }
    
    public string Description => patchDescription;
    
    public List<Variant> Variants { get; private set; }
    
    public Variant SelectedVariant => Variants[VariantIndex];
    
    public PatchData(string directoryPath)
    {
        var patchDataXml = new XmlDocument();
        patchDataXml.Load(directoryPath);
        
        string? patchDisplayName = patchDataXml.DocumentElement.Attributes["Name"].Value;
        string? patchDescription = null;
        
        foreach (XmlElement node in patchDataXml.DocumentElement)
        {
            if (node.Name == "Description")
            {
                patchDescription = node.InnerXml.Replace("<br />", Environment.NewLine);
            }

            if (node.Name == "Variants")
            {
                Variants = new List<Variant>();
                foreach (XmlElement variantNode in node.ChildNodes)
                {
                    string variantName = variantNode.GetAttribute("Name");
                    string? patchFileName = null;
                    string? patchBaseID = null;
                    string? patchNewID = null;
                    string? patchBaseCRC = null;
                    string? patchNewCRC = null;
                    
                    foreach (XmlElement variantChildNode in variantNode.ChildNodes)
                    {
                        if (variantChildNode.Name == "PatchFileName")
                        {
                            patchFileName = variantChildNode.InnerText;
                        }
                        
                        if (variantChildNode.Name == "OriginalGameID")
                        {
                            patchBaseID = variantChildNode.InnerText;
                        }

                        if (variantChildNode.Name == "NewGameID")
                        {
                            patchNewID = variantChildNode.InnerText;
                        }

                        if (variantChildNode.Name == "ExpectedBaseCRC")
                        {
                            patchBaseCRC = variantChildNode.InnerText;
                        }

                        if (variantChildNode.Name == "ExpectedBaseCRC")
                        {
                            patchNewCRC = variantChildNode.InnerText;
                        }
                    }
                    
                    if (variantName != null && patchFileName != null && patchBaseID != null && patchNewID != null 
                        && patchBaseCRC != null && patchNewCRC != null)
                    {
                        Variants.Add(new Variant(variantName, patchFileName, patchBaseID, 
                            patchNewID, patchBaseCRC, patchNewCRC));
                    }
                }
            }
        }

        if (patchDisplayName != null && patchDescription != null && Variants.Count > 0)
        {
            this.patchDisplayName = patchDisplayName;
            this.patchDescription = patchDescription;
        }
        else
        {
            throw new Exception("Invalid PatchData XML");
        }
    }
    
    public string SelectedPatchFilePath => Path.Combine(directoryLocation, SelectedVariant.PatchFileName);

    public override string ToString()
    {
        return patchDisplayName;
    }
}