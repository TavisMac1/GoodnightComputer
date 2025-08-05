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
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Descriptor { get; set; }

        public bool IsActive { get; set; } = false;

        public string CreatedOn { get; set; } = DateTime.Now.ToString("yyyy-mm-dd");

        public string Applications { get; set; } = "";

        public PlaylistRecord() { }

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
                context.Set<PlaylistRecord>().AddRange(this);
                context.SaveChanges();
                /*
                if (context.Database.CanConnect())
                {
                    context.Set<PlaylistRecord>().AddRange(this);
                    context.SaveChanges();
                }
                else throw new Exception("Unable to connect to the Database.");
                */
            }
        }

        public void Delete(AppDbContext dbContext)
        {
            // for deleting records
        }

        public PlaylistResult Validate(int applicationCount)
        {
            if (!string.IsNullOrEmpty(Name)) return new("Name is empty.");
            if (!string.IsNullOrEmpty(Descriptor)) return new("Description is empty");
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
