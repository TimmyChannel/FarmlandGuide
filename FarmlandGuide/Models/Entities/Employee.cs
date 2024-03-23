﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        [ObservableProperty]
        string _employeeName;
        [ObservableProperty]
        string _passwordHash;
        [ObservableProperty]
        string _name;
        [ObservableProperty]
        string _fIO;
        [ObservableProperty]
        string _surname;
        [ObservableProperty]
        string _patronymic;
        [ObservableProperty]
        string _residentialAddress;
        [ObservableProperty]
        string _workSchedule;
        [ObservableProperty]
        string _position;
        [ObservableProperty]
        decimal _salary;
        [ObservableProperty]
        Role _role;
        [ObservableProperty]
        Enterprise _enterprise;
        public ICollection<WorkSession> WorkSessions { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public Employee(string name, string surname, string patronymic, string position, string workSchedule)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            FIO = name + ' ' + surname + ' ' + patronymic;
            Position = position;
            WorkSchedule = workSchedule;
        }
    }
}
