using System;
using System.Diagnostics;
using System.IO;

namespace GoodnightComputer.Utils;

public static class ProcessClassifier
{
    private static readonly string WindowsRoot =
        Environment.GetFolderPath(Environment.SpecialFolder.Windows)
                   .TrimEnd(Path.DirectorySeparatorChar)
                   .ToLowerInvariant();

    // Hardware / driver vendors whose processes are never user-facing apps.
    private static readonly string[] HardwareVendors =
    {
        "intel corporation",
        "intel(r) corporation",
        "advanced micro devices",
        "amd",
        "nvidia corporation",
        "realtek semiconductor",
        "qualcomm",
        "broadcom",
        //"marvell",
        //"creative technology",
        //"synaptics",
        "elan microelectronics",
        //"wacom",
        //"logitech",
        //"corsair",
        //"razer",
        //"steelseries",
        //"asus",
        "gigabyte",
    };

    /// <summary>
    /// Returns <c>true</c> when the process is an OS or hardware-driver process
    /// that should always be hidden from the user.
    /// </summary>
    public static bool IsSystemOrHardwareProcess(Process p)
    {
        try
        {
            string? path = p.MainModule?.FileName;
            if (path != null)
            {
                // Any executable that lives under C:\Windows\ is OS-owned.
                if (path.ToLowerInvariant().StartsWith(WindowsRoot, StringComparison.Ordinal))
                    return true;
            }

            var fvi = p.MainModule?.FileVersionInfo;
            if (fvi != null)
            {
                string company = (fvi.CompanyName ?? string.Empty).Trim().ToLowerInvariant();

                // Hardware-vendor processes (drivers, control panels, etc.).
                foreach (var vendor in HardwareVendors)
                {
                    if (company.Contains(vendor))
                        return true;
                }
            }
        }
        catch
        {
            // If we can't inspect the process it's almost certainly a protected
            // OS process — treat it as system.
            return true;
        }

        return false;
    }
}
