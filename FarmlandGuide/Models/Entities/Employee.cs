using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FarmlandGuide.Models.Entities
{
    public partial class Employee : ObservableObject
    {
        public int EmployeeId { get; set; }
        public int EnterpriseId { get; set; }
        public int RoleId { get; set; }
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
             string residentialAddress, string employeeName, string passwordHash, string passwordSalt, int enterpriseId, int roleId)
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
            EnterpriseId = enterpriseId;
            RoleId = roleId;
            PropertyChanging += Employee_PropertyChanging;
            PropertyChanged += Employee_PropertyChanged;
        }

        private void Employee_PropertyChanged(object? sender, PropertyChangedEventArgs e)
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
            }
        }

        private void Employee_PropertyChanging(object? sender, PropertyChangingEventArgs e)
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
            }
        }

        private void Role_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Role));
        }

        private void Enterprise_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Enterprise));
        }
        public override string ToString()
        {
            return Surname + " " + Name + " " + Patronymic;
        }
        public string GetShortFio()
        {
            return Surname + " " + Name.First() + ". " + Patronymic.First() + '.';
        }
    }
}
