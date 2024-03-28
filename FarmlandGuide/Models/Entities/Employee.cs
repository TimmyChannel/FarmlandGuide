using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public partial class Employee : ObservableObject
    {
        public int EmployeeID { get; set; }
        public int EnterpriseID { get; set; }
        public int RoleID { get; set; }
        [ObservableProperty] private string _employeeName;
        [ObservableProperty] private string _passwordHash;
        [ObservableProperty] private string _passwordSalt;
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _surname;
        [ObservableProperty] private string _patronymic;
        [ObservableProperty] private string _residentialAddress;
        [ObservableProperty] private string _workSchedule;
        [ObservableProperty] private string _position;
        [ObservableProperty] private decimal _salary;
        [ObservableProperty] private Role _role;
        [ObservableProperty] private Enterprise _enterprise;
        public ICollection<WorkSession> WorkSessions { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public Employee(string name, string surname, string patronymic, string position, string workSchedule)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Position = position;
            WorkSchedule = workSchedule;
        }
        public Employee(string name, string surname, string patronymic, string workSchedule, string position, decimal salary,
             string residentialAddress, string employeeName, string passwordHash, string passwordSalt, int enterpriseID, int roleID)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Position = position;
            WorkSchedule = workSchedule;
            Salary = salary;
            ResidentialAddress = residentialAddress;
            EmployeeName = employeeName;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            EnterpriseID = enterpriseID;
            RoleID = roleID;
            this.PropertyChanging += Employee_PropertyChanging;
            this.PropertyChanged += Employee_PropertyChanged;
        }

        private void Employee_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Enterprise):
                    if (Enterprise is null) break;
                    Enterprise.PropertyChanged += Enterprise_PropertyChanged;
                    break;
                case nameof(Role):
                    if (Role is null) break;
                    Role.PropertyChanged += Role_PropertyChanged;
                    break;
                default:
                    break;
            }
        }

        private void Employee_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Enterprise):
                    if (Enterprise is null) break;
                    Enterprise.PropertyChanged -= Enterprise_PropertyChanged;
                    break;
                case nameof(Role):
                    if (Role is null) break;
                    Role.PropertyChanged -= Role_PropertyChanged;
                    break;
                default:
                    break;
            }
        }

        private void Role_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Role));
        }

        private void Enterprise_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Enterprise));
        }
        public override string ToString()
        {
            return Surname + " " + Name + " " + Patronymic;
        }
        public string GetShortFIO()
        {
            return Surname + " " + Name.First() + ". " + Patronymic.First() + '.';
        }
    }
}
