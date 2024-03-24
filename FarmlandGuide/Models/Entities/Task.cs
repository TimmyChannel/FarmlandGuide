using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public partial class Task : ObservableObject
    {
        public int TaskID { get; set; }
        public int EmployeeID { get; set; }
        public int ProcessID { get; set; }

        [ObservableProperty]
        DateTime _assignmentDate;
        [ObservableProperty]
        DateTime _dueDate;
        [ObservableProperty]
        string _status;
        [ObservableProperty]
        string _description;
        [ObservableProperty]
        Employee _employee;
        [ObservableProperty]
        ProductionProcess _productionProcess;
        public Task(int taskID, int employeeID, int processID, DateTime assignmentDate, DateTime dueDate, string status, string description)
        {
            TaskID = taskID;
            EmployeeID = employeeID;
            ProcessID = processID;
            AssignmentDate = assignmentDate;
            DueDate = dueDate;
            Status = status;
            Description = description;
            this.PropertyChanging += Task_PropertyChanging;
            this.PropertyChanged += Task_PropertyChanged;
        }

        private void Task_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Employee):
                    if (Employee is null) break;
                    Employee.PropertyChanged -= Employee_PropertyChanged;
                    break;
                case nameof(ProductionProcess):
                    if (ProductionProcess is null) break;
                    ProductionProcess.PropertyChanged -= ProductionProcess_PropertyChanged;
                    break;
                default:
                    break;
            }
        }

        private void Task_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Employee):
                    if (Employee is null) break;
                    Employee.PropertyChanged += Employee_PropertyChanged;
                    break;
                case nameof(ProductionProcess):
                    if (ProductionProcess is null) break;
                    ProductionProcess.PropertyChanged += ProductionProcess_PropertyChanged;
                    break;
                default:
                    break;
            }
        }

        private void ProductionProcess_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ProductionProcess));
        }

        private void Employee_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Employee));
        }
    }
}
