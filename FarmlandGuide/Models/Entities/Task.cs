using CommunityToolkit.Mvvm.ComponentModel;
using FarmlandGuide.Models.Entities;
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
        public int StatusID { get; set; }
        [ObservableProperty]
        DateTime _assignmentDate;
        [ObservableProperty]
        DateTime _dueDate;
        [ObservableProperty]
        Status _status;
        [ObservableProperty]
        string _description;
        [ObservableProperty]
        Employee _employee;
        [ObservableProperty]
        ProductionProcess _productionProcess;
        public Task()
        {
            this.PropertyChanging += Task_PropertyChanging;
            this.PropertyChanged += Task_PropertyChanged;
        }
        public Task(int employeeID, int processID, int statusID, DateTime assignmentDate, DateTime dueDate, string description)
        {
            EmployeeID = employeeID;
            ProcessID = processID;
            StatusID = statusID;
            AssignmentDate = assignmentDate;
            DueDate = dueDate;
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

        public override string ToString()
        {
            return $"AssignmentDate: {AssignmentDate} DueDate: {DueDate} Description: {Description}" ;
        }
    }
}
