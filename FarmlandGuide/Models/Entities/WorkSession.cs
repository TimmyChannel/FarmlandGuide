using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public class WorkSession
    {
        public int SessionID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Employee Employee { get; set; }
        public WorkSession(DateTime startDateTime, DateTime endDateTime, string type)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Type = type;
        }
    }
}
