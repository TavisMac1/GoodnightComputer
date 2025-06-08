using System;
using System.Collections.Generic;

namespace GoodnightComputer.Models
{
    public partial class ScheduleRecord
    {
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Name { get; set;  }

        public List<ApplicationRecord> ApplicationRecords { get; set; }

        public bool ManualRun { get; set; } = false;

        public bool IsActive => DateTime.Now >= StartTime && DateTime.Now <= EndTime;

        public ScheduleRecord(
            DateTime start, 
            DateTime end, 
            string name, 
            List<ApplicationRecord> applicationRecords
         )
        {
            Id = Guid.NewGuid();
            ApplicationRecords = applicationRecords;
            StartTime = start;
            EndTime = end;
            Name = name;
        }
    }
}
