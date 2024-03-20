using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public int EmployeeID { get; set; }
        public int ProcessID { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Employee Employee { get; set; }
        public ProductionProcess ProductionProcess { get; set; }
        //public Task(int taskID, int employeeID, int processID, DateTime assignmentDate, DateTime dueDate, string status, string description)
        //{
        //    TaskID = taskID;
        //    EmployeeID = employeeID;
        //    ProcessID = processID;
        //    AssignmentDate = assignmentDate;
        //    DueDate = dueDate;
        //    Status = status;
        //    Description = description;
        //}
        public Task(int taskID, int employeeID, DateTime assignmentDate, DateTime dueDate, string status, string description)
        {
            TaskID = taskID;
            EmployeeID = employeeID;
            AssignmentDate = assignmentDate;
            DueDate = dueDate;
            Status = status;
            Description = description;
        }
    }
}
