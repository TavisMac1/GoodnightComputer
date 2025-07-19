using GoodnightComputer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodnightComputer.Models
{
    public partial class PlaylistRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Descriptor { get; set; }

        public int ApplicationCount { get; set; }

        public bool IsActive { get; set; } = false;

        public string CreatedOn { get; set; } = DateTime.Now.ToString("yyyy-mm-dd");

        private string Applications { get; set; } = "";

        public PlaylistRecord
        (
            string name,
            string descriptor,
            List<ApplicationRecord> applicationRecords
         )
        {
            Id = Guid.NewGuid();
            Applications = PrepareApplicationRecords(applicationRecords);
            Name = name;
            Descriptor = descriptor;
        }

        public void Save()
        {
            using (var context = new AppDbContext())
            {
                context.Set<PlaylistRecord>().AddRange(this);
                context.SaveChanges();
            }
        }

        public void Delete(AppDbContext dbContext)
        {
            // for deleting records
        }

        private string PrepareApplicationRecords(List<ApplicationRecord> applicationRecords)
        {
            if (ApplicationCount > 0)
            {
                List<string> appNames = new();
                applicationRecords.ForEach(elem => appNames.Add(elem.Name));
                return string.Join(",", appNames);
            }
            return string.Empty;
        }

        public PlaylistResult Validate()
        {
            if (!string.IsNullOrEmpty(Name)) return new("Name is empty.");
            if (!string.IsNullOrEmpty(Descriptor)) return new("Description is empty");
            if (ApplicationCount == 0) return new("There are no applications added.");

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
