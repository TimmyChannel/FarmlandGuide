using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Collections.ObjectModel;

namespace FarmlandGuide.ViewModels
{
    public partial class EmployeesPageViewModel : ObservableObject
    {
        public EmployeesPageViewModel()
        {
            using var db = new ApplicationDbContext();
            Employees = new(db.Employees.ToList());
            Enterprises = new(db.Enterprises.ToList());
            Roles = new(db.Roles.ToList());
        }

        [ObservableProperty]
        ObservableCollection<Employee> _employees;
        [ObservableProperty]
        ObservableCollection<Enterprise> _enterprises;
        [ObservableProperty]
        ObservableCollection<Role> _roles;
        [ObservableProperty]
        string _selectedRole;

        [RelayCommand]
        public void OnDeleteEmployee(object employeeObj)
        {
            if (employeeObj is not Employee employee) return;
            using var db = new ApplicationDbContext();
            db.Employees.Remove(employee);
            db.SaveChangesAsync();
        }
    }
}
