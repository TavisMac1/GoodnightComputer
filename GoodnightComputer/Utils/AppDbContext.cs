using GoodnightComputer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace GoodnightComputer.Utils
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}
        
        public DbSet<PlaylistRecord> PlaylistEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Environment.CurrentDirectory, "GoodnightComputer.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
}
