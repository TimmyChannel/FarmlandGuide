using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FarmlandGuide.Models.Entities
{
    public partial class WorkSession : ObservableObject
    {
        public int SessionId { get; set; }
        public int EmployeeId { get; set; }
        [ObservableProperty] private DateTime _startDateTime;
        [ObservableProperty] private DateTime _endDateTime;
        [ObservableProperty] private string _type;
        [ObservableProperty] private Employee _employee;
        public WorkSession()
        {
        }
        public WorkSession(DateTime startDateTime, DateTime endDateTime, string type)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Type = type;
        }
        public WorkSession(DateTime startDateTime, DateTime endDateTime, string type, int employeeId)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Type = type;
            EmployeeId = employeeId;
        }

        public override string ToString()
        {
            return $"StartDateTime: {StartDateTime} EndDateTime: {EndDateTime} Type: {Type}"; 
        }
    }
}
