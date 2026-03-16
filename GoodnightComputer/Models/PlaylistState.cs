using GoodnightComputer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace GoodnightComputer.Models;

public class PlaylistState
{
    public ObservableCollection<PlaylistRecord> GlobalPlaylistRecords { get; set; } = new();

    public PlaylistState() => GlobalPlaylistRecords = this.PopulateDataFromDb();
    
    private ObservableCollection<PlaylistRecord> PopulateDataFromDb()
    {
        using (var context = new AppDbContext())
        {
            if (context.Database.CanConnect())
            {
                try
                {
                    var records = context.Set<PlaylistRecord>().ToList();

                    foreach (var record in records)
                    {
                        record.DeleteFromDb = new RelayCommand(_ => DeleteFromDb((Guid)record.Id), _ => true);
                        record.SleepPlaylistApplications = new RelayCommand(_ => SleepPlaylistApplications(record), _ => true);
                    }

                    return new ObservableCollection<PlaylistRecord>(records);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error fetching playlists: {e.Message}");
                }
            }
            else
            {
                throw new Exception("Unable to connect to the Database.");
            }
        }
    }

    public void Insert (PlaylistRecord record) => AddToDb(record);

    public void Delete (Guid id) => DeleteFromDb(id);

    private void AddToDb(PlaylistRecord record)
    {
        using (var context = new AppDbContext())
        {
            if (context.Database.CanConnect())
            {
                context.Set<PlaylistRecord>().Add(record);
                try
                {
                    context.SaveChanges();
                    record.DeleteFromDb = new RelayCommand((id) => DeleteFromDb((Guid)id), _ => true);
                    record.SleepPlaylistApplications = new RelayCommand(_ => SleepPlaylistApplications(record), _ => true);
                    GlobalPlaylistRecords.Add(record);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error saving playlist: {e.Message}");
                }
            }
            else throw new Exception("Unable to connect to the Database.");
        }
    }

    private void DeleteFromDb(Guid id)
    {
        using var context = new AppDbContext();
        if (context.Database.CanConnect())
        {
            try
            {
                var record = context.Set<PlaylistRecord>().Find(id);
                if (record is null) throw new Exception("This record is null. This operation can not be completed.");

                context.Set<PlaylistRecord>().Remove(record);
                context.SaveChanges();
                
                var recordToRemove = GlobalPlaylistRecords.FirstOrDefault(app => app.Id == id);
                if (recordToRemove != null) GlobalPlaylistRecords.Remove(recordToRemove);
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting playlist: {e.Message}");
            }
        }
        else throw new Exception("Unable to connect to the Database.");
    }

    private static bool IsProcessAccessible(Process process, string appName)
    {
        try
        {
            return process.ProcessName.Equals(appName, StringComparison.OrdinalIgnoreCase) ||
                   process.MainModule?.FileVersionInfo?.FileDescription?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // Cannot access process module information (protected/system process)
            return false;
        }
    }

    public static void SleepPlaylistApplications(PlaylistRecord playlist)
    {
        if (playlist?.ApplicationsList == null || playlist.ApplicationsList.Count == 0)
        {
            Console.WriteLine("No applications in playlist to pause.");
            return;
        }

        Process[] runningProcesses;
        
        try
        {
            runningProcesses = Process.GetProcesses();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to access Process.GetProcesses: {ex.Message}");
            return;
        }

        foreach (var appName in playlist.ApplicationsList)
        {
            var matchingProcesses = runningProcesses
                .Where(p => IsProcessAccessible(p, appName))
                .ToList();

            foreach (var process in matchingProcesses)
            {
                try
                {
                    GeneralUtils.SuspendProcess(process);
                    Console.WriteLine($"Paused: {process.ProcessName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to pause {appName}: {ex.Message}");
                }
            }
        }
    }
}
