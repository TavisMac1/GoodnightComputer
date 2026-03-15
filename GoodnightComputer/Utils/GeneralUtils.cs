using GoodnightComputer.Models;
using GoodnightComputer.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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

    public static void PausePlaylistApplications(PlaylistRecord playlist)
    {
        if (playlist?.ApplicationsList == null || playlist.ApplicationsList.Count == 0)
        {
            Console.WriteLine("No applications in playlist to pause.");
            return;
        }

        var runningProcesses = Process.GetProcesses();

        foreach (var appName in playlist.ApplicationsList)
        {
            var matchingProcesses = runningProcesses
                .Where(p => p.ProcessName.Equals(appName, StringComparison.OrdinalIgnoreCase) ||
                           p.MainModule?.FileVersionInfo?.FileDescription?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            foreach (var process in matchingProcesses)
            {
                try
                {
                    SuspendProcess(process);
                    Console.WriteLine($"Paused: {process.ProcessName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to pause {appName}: {ex.Message}");
                }
            }
        }
    }

    public static void ResumePlaylistApplications()
    {
        if (SystemState.STATIC_APPLICATION_NAMES.Count == 0)
        {
            Console.WriteLine("There are no apps to 'resume'");
            return;
        }

        var runningProcesses = Process.GetProcesses();

        foreach (var appName in SystemState.STATIC_APPLICATION_NAMES)
        {
            var matchingProcesses = runningProcesses
                .Where(p => p.ProcessName.Equals(appName, StringComparison.OrdinalIgnoreCase) ||
                           p.MainModule?.FileVersionInfo?.FileDescription?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            foreach (var process in matchingProcesses)
            {
                try
                {
                    ResumeProcess(process);
                    Console.WriteLine($"Resumed: {process.ProcessName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to resume {appName}: {ex.Message}");
                }
            }
        }
    }

    private static void SuspendProcess(Process process)
    {
        foreach (ProcessThread pT in process.Threads)
        {
            IntPtr pOpenThread = NativeMethods.OpenThread(ThreadAccess.SuspendResume, false, (uint)pT.Id);

            if (pOpenThread == IntPtr.Zero)
                continue;

            NativeMethods.SuspendThread(pOpenThread);
            NativeMethods.CloseHandle(pOpenThread);
        }
    }

    private static void ResumeProcess(Process process)
    {
        foreach (ProcessThread pT in process.Threads)
        {
            IntPtr pOpenThread = NativeMethods.OpenThread(ThreadAccess.SuspendResume, false, (uint)pT.Id);

            if (pOpenThread == IntPtr.Zero)
                continue;

            NativeMethods.ResumeThread(pOpenThread);
            NativeMethods.CloseHandle(pOpenThread);
        }
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
