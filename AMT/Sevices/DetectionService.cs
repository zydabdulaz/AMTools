using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ArdysaModsTools.Services
{
    public class DetectionService
    {
        public OperationResult TryAutoDetect()
        {
            try
            {
                // Registry lookup for Steam
                string? steamExe = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamExe", null) as string;
                if (!string.IsNullOrEmpty(steamExe))
                {
                    string baseDir = Path.GetDirectoryName(steamExe)!;
                    string dotaPath = Path.Combine(baseDir, "steamapps", "common", "dota 2 beta");
                    if (File.Exists(Path.Combine(dotaPath, "game", "bin", "win64", "dota2.exe")))
                        return OperationResult.Ok(dotaPath);
                }

                // HKLM uninstall path
                string? uninstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 570", "InstallLocation", null) as string;
                if (!string.IsNullOrEmpty(uninstallPath) &&
                    File.Exists(Path.Combine(uninstallPath, "game", "bin", "win64", "dota2.exe")))
                    return OperationResult.Ok(uninstallPath);

                return OperationResult.Fail("Dota 2 not found automatically.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail($"Auto-detect failed: {ex.Message}");
            }
        }
    }
}
