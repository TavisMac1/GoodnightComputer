using GoodnightComputer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace GoodnightComputer.Utils;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}
        
    public DbSet<PlaylistRecord> PlaylistEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GoodnightComputer",
            "GoodnightComputer.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        Console.WriteLine($"Database Path: {dbPath}");
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
