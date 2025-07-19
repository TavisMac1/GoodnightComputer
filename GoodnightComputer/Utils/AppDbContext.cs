using GoodnightComputer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace GoodnightComputer.Utils
{
    public class AppDbContext : DbContext
    {
        // table
        public DbSet<PlaylistRecord> PlaylistEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=GoodnightComputer.db");
        }
    }
}
