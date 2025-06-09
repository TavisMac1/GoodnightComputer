using GoodnightComputer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodnightComputer.Utils
{
    public static class PlaylistUtils
    {
        public static bool ValidatePlaylist
         (
            string name, 
            string description, 
            List<ApplicationRecord> applicationRecords
         )
        {
            int is_valid = 0;

            if (!string.IsNullOrEmpty(name)) is_valid += 1;
            if (!string.IsNullOrEmpty(description)) is_valid += 1;
            if (applicationRecords != null && applicationRecords.Count > 0) is_valid += 1;

            if (is_valid < 3)
            {
                return false;
            }

            return true;
        }
    }
}
