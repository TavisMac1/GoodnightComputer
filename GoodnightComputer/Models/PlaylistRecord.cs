using GoodnightComputer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodnightComputer.Models
{
    [Table("PlaylistEntities")]
    public partial class PlaylistRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = new Guid();

        public string Name { get; set; } = null;

        public string Descriptor { get; set; } = null;

        public bool IsActive { get; set; } = false;

        public string CreatedOn { get; set; } = DateTime.Now.ToString("yyyy-mm-dd");

        public string Applications { get; set; } = "";

        public PlaylistRecord
        (
            string name,
            string descriptor,
            string applicationRecords
         )
        {
            Applications = applicationRecords;
            Name = name;
            Descriptor = descriptor;
        }

        public void Save()
        {
            using (var context = new AppDbContext())
            {
                if (context.Database.CanConnect())
                {
                    context.Set<PlaylistRecord>().AddRange(this);
                    context.SaveChanges();
                }
                else throw new Exception("Unable to connect to the Database.");
            }
        }

        public void Delete(AppDbContext dbContext)
        {
            // for deleting records
        }

        public PlaylistResult Validate(int applicationCount)
        {
            Console.WriteLine($"Name: {this.Name}");
            Console.WriteLine($"Descriptor: {this.Descriptor}");

            if (string.IsNullOrEmpty(Name)) return new("Name is empty.");
            if (string.IsNullOrEmpty(Descriptor)) return new("Description is empty");
            if (applicationCount == 0) return new("There are no applications added.");

            return new(true);
        }
    }
    public class PlaylistResult
    {
        public string ErrorMessage { get; set; } = "";
        public bool IsValid { get; set; }

        public PlaylistResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public PlaylistResult(bool isValid)
        {
            IsValid = isValid;
        }
    }
}
