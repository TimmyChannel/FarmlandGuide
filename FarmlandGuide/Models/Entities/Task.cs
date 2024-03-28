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
            PropertyChanging += Task_PropertyChanging;
            PropertyChanged += Task_PropertyChanged;
        }
        public Task(int employeeId, int processId, int statusId, DateTime assignmentDate, DateTime dueDate, string description)
        {
            EmployeeId = employeeId;
            ProcessId = processId;
            StatusId = statusId;
            AssignmentDate = assignmentDate;
            DueDate = dueDate;
            Description = description;
            PropertyChanging += Task_PropertyChanging;
            PropertyChanged += Task_PropertyChanged;
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
