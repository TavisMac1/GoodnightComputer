using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodnightComputer.Models.ValidationModels
{
    public class DbSave(bool success, string errorMessage = "")
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
    }
}
