using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodnightComputer.Models
{
    public partial class PlaylistRecord
    {
        public void RemoveApplicationRecord(ApplicationRecord applicationRecord)
        {
            if (ApplicationRecords == null)
            {
                ApplicationRecords = new List<ApplicationRecord>();
            }
            ApplicationRecords.Remove(applicationRecord);
        }

        public void DeleteSchedule(Guid scheduleId)
        { 
            // Database Interaction

        }
    }
}
