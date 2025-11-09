using GoodnightComputer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GoodnightComputer.Utils
{
    public static class GeneralUtils
    {
        public static List<string> ListApplicationNames()
        {
            var appNames = new List<string>();
            foreach (var process in System.Diagnostics.Process.GetProcesses())
            {
                try
                {
                    string description = process.MainModule?.FileVersionInfo?.FileDescription;
                    if (!string.IsNullOrWhiteSpace(description)) appNames.Add(description);
                    else appNames.Add(process.ProcessName);
                }
                catch
                {
                    // Some system processes may not allow access to MainModule
                    Console.WriteLine($"Could not access process {process.ProcessName}: {process.Id}");
                    appNames.Add(process.ProcessName);
                }
            }

            if (appNames.Count == 0) appNames.Add("No applications found");

            Constants.Constants.EXCLUSION_APPS.ForEach(app =>
            {
                appNames.ForEach(appName =>
                {
                    if (app == appName) appNames.Remove(appName);
                });
            });

            for (int i1 = 0; i1 < appNames.Count; i1++)
            {
                int preserveFirstOccurance = 0;
                for (int i2 = 0; i2 < appNames.Count; i2++)
                { 
                    if (preserveFirstOccurance != 1 && appNames[i1] == appNames[i2]) preserveFirstOccurance++;
                    if (preserveFirstOccurance == 1 && appNames[i1] == appNames[i2])
                    {
                        appNames.RemoveAt(i2);
                        i2--;
                    }                
                }
            }

            return appNames;
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
}
