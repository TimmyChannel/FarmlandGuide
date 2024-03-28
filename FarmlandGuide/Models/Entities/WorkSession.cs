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
            PropertyChanged += WorkSession_PropertyChanged;
            PropertyChanging += WorkSession_PropertyChanging;
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
            PropertyChanged += WorkSession_PropertyChanged;
            PropertyChanging += WorkSession_PropertyChanging;
        }

        private void WorkSession_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName == nameof(Employee))
            {
                Employee.PropertyChanged -= Employee_PropertyChanged;
            }
        }

        private void WorkSession_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Employee) && Employee is not null)
            {
                Employee.PropertyChanged += Employee_PropertyChanged;
            }
        }

        private void Employee_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Employee));
        }
        public override string ToString()
        {
            return $"StartDateTime: {StartDateTime} EndDateTime: {EndDateTime} Type: {Type}"; 
        }
    }
}
