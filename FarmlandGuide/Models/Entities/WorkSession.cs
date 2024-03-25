using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public partial class WorkSession : ObservableObject
    {
        public int SessionID { get; set; }
        public int EmployeeID { get; set; }
        [ObservableProperty]
        DateTime _startDateTime;
        [ObservableProperty]
        DateTime _endDateTime;
        [ObservableProperty]
        string _type;
        [ObservableProperty]
        Employee _employee;
        public WorkSession()
        {
            this.PropertyChanged += WorkSession_PropertyChanged;
            this.PropertyChanging += WorkSession_PropertyChanging;
        }
        public WorkSession(DateTime startDateTime, DateTime endDateTime, string type)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Type = type;
        }
        public WorkSession(DateTime startDateTime, DateTime endDateTime, string type, int employeeID)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Type = type;
            EmployeeID = employeeID;
            this.PropertyChanged += WorkSession_PropertyChanged;
            this.PropertyChanging += WorkSession_PropertyChanging;
        }

        private void WorkSession_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName == nameof(Employee) && Employee is not null)
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
    }
}
