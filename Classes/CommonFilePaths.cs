using System;
using System.IO;

namespace ShadowSXLauncher.Classes;

public static class CommonFilePaths
{
    public static string AppStart
    {
        get { return AppContext.BaseDirectory; }
    }

    public static string ConfigFileLocation
    {
        get { return Path.Combine(AppStart, "Config.xml"); }
    }

    public static string DolphinPath
    {
        get { return Configuration.Instance.DolphinLocation; }
    }
        
    public static string SavePath
    {
        get { return Path.Combine(DolphinPath, "User", "GC", "USA", "Card A"); }
    }
        
    public static string GameSettingsFilePath
    {
        get { return Path.Combine(DolphinPath, "User", "GameSettings", "GUPX8P.ini"); }
    }

    public static string CustomTexturesPath
    {
        get { return Path.Combine(DolphinPath ,"User","Load","Textures","GUPX8P"); }
    }

    public static string SxResourcesPath
    {
        get { return Path.Combine(AppStart, "ShadowSXResources"); }
    }
        
    public static string SxResourcesCustomTexturesPath
    {
        get { return Path.Combine(SxResourcesPath, @"CustomTextures","GUPX8P"); }
    }
        
    public static string SxResourcesISOPatchingPath
    {
        get { return Path.Combine(SxResourcesPath, @"PatchingFiles"); }
    }
    
    public static string SxResourcesPatchDataFolderPath
    {
        get { return Path.Combine(SxResourcesISOPatchingPath, "Patches"); }
    }
    
    public static string SxResourcesPatchBinsFolderPath
    {
        get { return Path.Combine(SxResourcesISOPatchingPath, "PatchingScriptsAndBins"); }
    }
    
    public static string xdeltaBinPath
    {
        get
        {
            if(OperatingSystem.IsWindows())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "xdelta-3.1.0-x86_64.exe");
            }

            return string.Empty;
        }
    }
    
    public static string PatchingScriptPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "Patch ISO.bat");
            }

            return string.Empty;
        }
    }
}