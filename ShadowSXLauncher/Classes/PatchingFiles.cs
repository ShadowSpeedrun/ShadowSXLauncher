using System.IO;
using Avalonia;
using Avalonia.Platform;

namespace ShadowSXLauncher.Classes;

public static class PatchingFiles
{
    
    public static string Bin
    {
        get
        {
            switch (Configuration.Instance.CurrentOS)
            {
                case OperatingSystemType.WinNT:
                    return "xdelta-3.1 .0 - x86_64.exe";
            }

            return string.Empty;
        }
    }
    
    public static string PatchFile
    {
        get
        {
            return Path.Combine("vcdiff", "ShadowSX.vcdiff");
        }
    }
    
    public static string PathScript
    {
        get
        {
            switch (Configuration.Instance.CurrentOS)
            {
                case OperatingSystemType.WinNT:
                    return "Patch ISO.bat";
            }

            return string.Empty;
        }
    }
}