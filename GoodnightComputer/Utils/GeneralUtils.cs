using GoodnightComputer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GoodnightComputer.Utils;

public static class GeneralUtils
{
    public static async Task<List<string>> ListApplicationNames()
    {
        var appNames = new List<string>();
        foreach (var process in System.Diagnostics.Process.GetProcesses())
        {
            try
            {
                string processNameLower = process.ProcessName.ToLower();
                
                // Skip if process is in exclusion list
                if (Constants.Constants.EXCLUSION_APPS.Contains(processNameLower))
                    continue;

                string description = process.MainModule?.FileVersionInfo?.FileDescription!;

                Console.WriteLine(process.ProcessName);

                if (!string.IsNullOrWhiteSpace(description)) 
                    appNames.Add(description);
                else 
                    appNames.Add(process.ProcessName);
            }
            catch
            {
                // Some system processes may not allow access to MainModule
                Console.WriteLine($"Could not access process {process.ProcessName}: {process.Id}");
                appNames.Add(process.ProcessName);
            }
        }

        if (appNames.Count == 0) appNames.Add("No applications found");

        return appNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public static void OnApplicationExit()
    {
        // Perform any necessary cleanup here
        // For example, save settings, close files, etc.
        // Resume any apps which are asleep
        // Show notification and confirmation message
        Console.WriteLine("Application is exiting. Performing cleanup...");
    }
    
    public static string PrepareApplicationRecords(List<ApplicationRecord> applicationRecords)
    {
        if (applicationRecords == null) throw new Exception("Application Records was null.");

        if (applicationRecords.Count > 0)
        {
            List<string> appNames = new();
            applicationRecords.ForEach(elem => appNames.Add(elem.Name));
            return string.Join(",", appNames);
        }
        return string.Empty;
    }
}
