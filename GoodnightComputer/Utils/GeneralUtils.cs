using System;
using System.Collections.Generic;

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
                    // Try to get the file description (user-friendly app name)
                    string description = process.MainModule?.FileVersionInfo?.FileDescription;
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        appNames.Add(description);
                    }
                    else
                    {
                        // Fallback to process name
                        appNames.Add(process.ProcessName);
                    }
                }
                catch
                {
                    // Some system processes may not allow access to MainModule
                    Console.WriteLine($"Could not access process {process.ProcessName}: {process.Id}");
                    appNames.Add(process.ProcessName);
                }
            }

            if (appNames.Count == 0)
            {
                appNames.Add("No applications found");
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
    }
}
