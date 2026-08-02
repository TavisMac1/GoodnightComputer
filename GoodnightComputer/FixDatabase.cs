using System;
using GoodnightComputer.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GoodnightComputer;

public static class DatabaseFix
{
    public static void DropApplicationsListColumn()
    {
        using var context = new AppDbContext();
        var connection = context.Database.GetDbConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        // Check if the column exists
        command.CommandText = "PRAGMA table_info(PlaylistEntities);";
        var hasColumn = false;

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var columnName = reader.GetString(1);
                if (columnName == "ApplicationsList")
                {
                    hasColumn = true;
                    break;
                }
            }
        }

        if (!hasColumn)
        {
            Console.WriteLine("Column 'ApplicationsList' does not exist. Nothing to do.");
            return;
        }

        Console.WriteLine("Found 'ApplicationsList' column. Removing it...");

        // SQLite doesn't support DROP COLUMN directly, so we need to recreate the table
        command.CommandText = @"
            BEGIN TRANSACTION;

            -- Create new table without ApplicationsList
            CREATE TABLE PlaylistEntities_new (
                Id TEXT NOT NULL PRIMARY KEY,
                Name TEXT NOT NULL,
                Descriptor TEXT,
                IsActive INTEGER NOT NULL,
                CreatedOn TEXT NOT NULL,
                Applications TEXT NOT NULL
            );

            -- Copy data from old table to new table
            INSERT INTO PlaylistEntities_new (Id, Name, Descriptor, IsActive, CreatedOn, Applications)
            SELECT Id, Name, Descriptor, IsActive, CreatedOn, Applications
            FROM PlaylistEntities;

            -- Drop old table
            DROP TABLE PlaylistEntities;

            -- Rename new table to original name
            ALTER TABLE PlaylistEntities_new RENAME TO PlaylistEntities;

            COMMIT;
        ";

        command.ExecuteNonQuery();

        Console.WriteLine("Successfully removed 'ApplicationsList' column.");
        Console.WriteLine("Database has been fixed!");
    }
}
