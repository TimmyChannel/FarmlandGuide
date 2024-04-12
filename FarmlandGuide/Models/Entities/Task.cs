using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FarmlandGuide.Models.Entities
{
    public partial class Task : ObservableObject
    {
        public int TaskId { get; set; }
        public int EmployeeId { get; set; }
        public int ProcessId { get; set; }
        public int StatusId { get; set; }
        [ObservableProperty] private DateTime _assignmentDate;
        [ObservableProperty] private DateTime _dueDate;
        [ObservableProperty] private Status _status;
        [ObservableProperty] private string _description;
        [ObservableProperty] private Employee _employee;
        [ObservableProperty] private ProductionProcess _productionProcess;
        public Task()
        {
        }
        public Task(int employeeId, int processId, int statusId, DateTime assignmentDate, DateTime dueDate, string description)
        {
            EmployeeId = employeeId;
            ProcessId = processId;
            StatusId = statusId;
            AssignmentDate = assignmentDate;
            DueDate = dueDate;
            Description = description;
        }

        public override string ToString()
        {
            return $"AssignmentDate: {AssignmentDate} DueDate: {DueDate} Description: {Description}" ;
        }
    }
}
