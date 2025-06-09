using System;
using System.Collections.Generic;

namespace GoodnightComputer.Models
{
    public partial class PlaylistRecord
    {
        public Guid Id { get; set; }

        public string Name { get; set;  }

        public string Descriptor { get; set; }

        public int ApplicationCount { get; set; } 

        public List<ApplicationRecord> ApplicationRecords { get; set; }

        public bool IsActive { get; set; } = false;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public PlaylistRecord(
            string name, 
            string descriptor,
            List<ApplicationRecord> applicationRecords
         )
        {
            Id = Guid.NewGuid();
            ApplicationRecords = applicationRecords;
            Name = name;
            ApplicationCount = applicationRecords.Count;
            Descriptor = descriptor;
        }
    }
}
