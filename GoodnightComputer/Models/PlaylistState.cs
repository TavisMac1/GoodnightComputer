using GoodnightComputer.Utils;
using System;
using System.Collections.ObjectModel;
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

                    foreach (var record in records) record.DeleteFromDb = DeleteFromDb;

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

    private void AddToDb
    (
        PlaylistRecord record
    )
    {
        using (var context = new AppDbContext())
        {
            if (context.Database.CanConnect())
            {
                context.Set<PlaylistRecord>().Add(record);
                try
                {
                    context.SaveChanges();
                    record.DeleteFromDb = DeleteFromDb;
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

    private void DeleteFromDb
    (
        Guid id
    )
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
                foreach (var app in GlobalPlaylistRecords) if (app.Id == id) GlobalPlaylistRecords.Remove(app);
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting playlist: {e.Message}");
            }
        }
        else throw new Exception("Unable to connect to the Database.");
    }

    private void DeleteFromDb(PlaylistRecord record)
    {
        using var context = new AppDbContext();
        if (context.Database.CanConnect())
        {
            context.Set<PlaylistRecord>().Remove(record);
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting playlist: {e.Message}");
            }
        }
        else throw new Exception("Unable to connect to the Database.");
    }
}
