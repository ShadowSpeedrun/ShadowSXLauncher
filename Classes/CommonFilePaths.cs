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

    public static string DolphinBinPath
    {
        get { return Configuration.Instance.DolphinBinLocation; }
    }
    
    public static string DolphinUserPath
    {
        get { return Configuration.Instance.DolphinUserLocation; }
    }
        
    public static string SavePath
    {
        get { return Path.Combine(DolphinUserPath, "GC", "USA", "Card A"); }
    }
        
    public static string GameSettingsFilePath
    {
        get { return Path.Combine(DolphinUserPath, "GameSettings", "GUPX8P.ini"); }
    }

    public static string CustomTexturesPath
    {
        get { return Path.Combine(DolphinUserPath ,"Load","Textures","GUPX8P"); }
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

    public static string DolphinBinFile
    {
        get
        {
            if(OperatingSystem.IsWindows())
            {
                return "Dolphin.exe";
            }
            
            if (OperatingSystem.IsLinux())
            {
                return "Dolphin";
            }
            
            if (OperatingSystem.IsMacOS())
            {
                //TODO: What extension is used once it's extracted?
                return "Dolphin";
            }

            return string.Empty;
        }
    }
    
    public static string xdeltaBinPath
    {
        get
        {
            if(OperatingSystem.IsWindows())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "xdelta-3.1.0-x86_64.exe");
            }
            
            if (OperatingSystem.IsLinux())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "xdelta3");
            }
            
            if (OperatingSystem.IsMacOS())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "xdelta3_mac");
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
            
            if (OperatingSystem.IsLinux())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "Patch ISO.sh");
            }
            
            if (OperatingSystem.IsMacOS())
            {
                return Path.Combine(SxResourcesPatchBinsFolderPath, "Patch ISO.command");
            }

            return string.Empty;
        }
    }
    
    public static string GetExplorerPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return "explorer.exe";
            }
            
            if (OperatingSystem.IsLinux())
            {
                return "xdg-open";
            }
            
            if (OperatingSystem.IsMacOS())
            {
                return "open";
            }
            
            throw new Exception("Unsupported Operating System");
        }
    }
}