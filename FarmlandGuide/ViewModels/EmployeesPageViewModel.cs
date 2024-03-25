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
using FarmlandGuide.Helpers.Validators;
using MathNet.Numerics.Interpolation;
using System.ComponentModel.DataAnnotations;
using NPOI.Util;
using System.Diagnostics;
using FarmlandGuide.Helpers;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using FarmlandGuide.Helpers.Messages;

namespace FarmlandGuide.ViewModels
{
    public partial class EmployeesPageViewModel : ObservableValidator, IRecipient<WorkSessionReassigned>
    {
        public EmployeesPageViewModel()
        {
            using var db = new ApplicationDbContext();
            Employees = new(db.Employees.Include(e => e.WorkSessions).ToList());
            Enterprises = new(db.Enterprises.ToList());
            Roles = new(db.Roles.ToList());
            this.PropertyChanged += EmployeesPageViewModel_PropertyChanged;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }
        public void Receive(WorkSessionReassigned message)
        {
            using var db = new ApplicationDbContext();
            Employees = new(db.Employees.AsNoTracking().Include(e => e.WorkSessions).ToList());

        }

        private void EmployeesPageViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedEmployee) && SelectedEmployee is not null)
                WeakReferenceMessenger.Default.Send(new SelectedEmployeeMessage(SelectedEmployee.Copy()));
        }

        [ObservableProperty]
        bool _isEdit = false;

        [ObservableProperty]
        string _titleText;

        [ObservableProperty]
        string _buttonApplyText;


        #region Employee
        [ObservableProperty]
        ObservableCollection<Employee> _employees;
        [ObservableProperty]
        ObservableCollection<Enterprise> _enterprises;
        [ObservableProperty]
        ObservableCollection<Role> _roles;
        [ObservableProperty]
        Employee? _selectedEmployee;


        string _name;
        string _surname;
        string _patronymic;
        string _residentialAddress;
        Enterprise? _selectedEnterprise;
        string _position;
        string _workSchedule;
        decimal _salary;
        string _employeeName;
        string _password;
        Role? _selectedRole;

        [NotEmpty]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value, true);
            }
        }

        [NotEmpty]
        public string Surname
        {
            get { return _surname; }
            set
            {
                SetProperty(ref _surname, value, true);
            }
        }

        [NotEmpty]
        public string Patronymic
        {
            get { return _patronymic; }
            set
            {
                SetProperty(ref _patronymic, value, true);
            }
        }

        [NotEmpty]
        public string ResidentialAddress
        {
            get { return _residentialAddress; }
            set
            {
                SetProperty(ref _residentialAddress, value, true);
            }
        }

        [NotEmpty]
        public string Position
        {
            get { return _position; }
            set
            {
                SetProperty(ref _position, value, true);
            }
        }

        [NotEmpty]
        public string WorkSchedule
        {
            get { return _workSchedule; }
            set
            {
                SetProperty(ref _workSchedule, value, true);
            }
        }

        [LessOrEqualThenValidation(0)]
        public decimal Salary
        {
            get { return _salary; }
            set
            {
                SetProperty(ref _salary, value, true);
            }
        }

        [NotEmpty]
        [MinLengthWithCustomMessage(4, "Длина логина не может быть меньше 4")]
        public string EmployeeName
        {
            get { return _employeeName; }
            set
            {
                SetProperty(ref _employeeName, value, true);
            }
        }

        [Password(@"^(?=.*\p{Ll})(?=.*\p{Lu})(?=.*\p{N}).{6,}$")]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value, true);
            }
        }


        [ShouldBeSelected("Необходимо выбрать пердприятие")]
        public Enterprise? SelectedEnterprise
        {
            get { return _selectedEnterprise; }
            set
            {
                SetProperty(ref _selectedEnterprise, value, true);
            }
        }

        [ShouldBeSelected("Необходимо выбрать роль пользователя в системе")]
        public Role? SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                SetProperty(ref _selectedRole, value, true);
            }
        }
        [RelayCommand]
        private void OnApplayChangesAtEmployees()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            if (IsEdit)
                OnEditEmployee();
            else
                OnAddEmployee();
            OnCloseDialogAndClearProps();
        }
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            IsEdit = false;
            Name = string.Empty;
            Surname = string.Empty;
            Patronymic = string.Empty;
            ResidentialAddress = string.Empty;
            Position = string.Empty;
            WorkSchedule = string.Empty;
            Salary = 0;
            EmployeeName = string.Empty;
            Password = string.Empty;
            SelectedEnterprise = null;
            SelectedRole = null;
            ClearErrors();
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);
        }
        private void OnAddEmployee()
        {
            using var ctx = new ApplicationDbContext();
            var salt = PasswordManager.GenerateSalt();
            var passwordHash = PasswordManager.HashPassword(Password, salt);
            var employee = new Employee(Name.Trim(), Surname.Trim(), Patronymic.Trim(), WorkSchedule.Trim(), Position.Trim(), Salary,
                ResidentialAddress.Trim(), EmployeeName.Trim(), passwordHash, salt, SelectedEnterprise?.EnterpriseID ?? 1, SelectedRole?.RoleID ?? 1)
            {
                Enterprise = ctx.Enterprises.First(e => e.EnterpriseID == SelectedEnterprise.EnterpriseID),
                Role = ctx.Roles.First(r => r.RoleID == SelectedRole.RoleID)
            };
            ctx.Employees.Add(employee);
            ctx.SaveChanges();
            Employees.Add(employee);
            WeakReferenceMessenger.Default.Send(new EmployeeTableUpdateMessage(employee));
        }
        private void OnEditEmployee()
        {
            using var ctx = new ApplicationDbContext();
            SelectedEmployee.Name = Name.Trim();
            SelectedEmployee.Surname = Surname.Trim();
            SelectedEmployee.Patronymic = Patronymic.Trim();
            SelectedEmployee.ResidentialAddress = ResidentialAddress.Trim();
            SelectedEmployee.Position = Position.Trim();
            SelectedEmployee.WorkSchedule = WorkSchedule.Trim();
            SelectedEmployee.Salary = Salary;
            SelectedEmployee.EmployeeName = EmployeeName.Trim();
            var salt = PasswordManager.GenerateSalt();
            var passwordHash = PasswordManager.HashPassword(Password, salt);

            SelectedEmployee.PasswordHash = passwordHash;
            SelectedEmployee.PasswordSalt = salt;
            SelectedEmployee.Enterprise = SelectedEnterprise;
            SelectedEmployee.EnterpriseID = SelectedEnterprise.EnterpriseID;
            SelectedEmployee.Role = SelectedRole;
            SelectedEmployee.RoleID = SelectedRole.RoleID;

            ctx.Employees.Update(SelectedEmployee);
            ctx.SaveChanges();
            WeakReferenceMessenger.Default.Send(new EmployeeTableUpdateMessage(SelectedEmployee));
        }

        [RelayCommand]
        public void OnDeleteEmployee()
        {
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            if (SelectedEmployee is null)
            {
                closeDialogCommand.Execute(null, null);
                return;
            }
            using var ctx = new ApplicationDbContext();
            ctx.Employees.Remove(SelectedEmployee);
            ctx.SaveChanges();
            WeakReferenceMessenger.Default.Send(new EmployeeTableUpdateMessage(SelectedEmployee));
            Employees.Remove(SelectedEmployee);
            closeDialogCommand.Execute(null, null);
        }

        [RelayCommand]
        private void OnOpenEditDialog()
        {
            if (SelectedEmployee is null)
                return;
            TitleText = "Редактирование информации о сотруднике";
            ButtonApplyText = "Сохранить";
            Name = SelectedEmployee.Name;
            Surname = SelectedEmployee.Surname;
            Patronymic = SelectedEmployee.Patronymic;
            ResidentialAddress = SelectedEmployee.ResidentialAddress;
            Position = SelectedEmployee.Position;
            WorkSchedule = SelectedEmployee.WorkSchedule;
            Salary = SelectedEmployee.Salary;
            EmployeeName = SelectedEmployee.EmployeeName;
            SelectedEnterprise = SelectedEmployee.Enterprise;
            SelectedRole = SelectedEmployee.Role;
            IsEdit = true;
        }

        [RelayCommand]
        private void OnOpenAddDialog()
        {
            TitleText = "Добавление нового сотрудника";
            ButtonApplyText = "Добавить";
            IsEdit = false;
            Name = string.Empty;
            Surname = string.Empty;
            Patronymic = string.Empty;
            ResidentialAddress = string.Empty;
            Position = string.Empty;
            WorkSchedule = string.Empty;
            Salary = 0;
            EmployeeName = string.Empty;
            Password = string.Empty;
            SelectedEnterprise = null;
            SelectedRole = null;
            ClearErrors();
        }

        #endregion

    }
}
