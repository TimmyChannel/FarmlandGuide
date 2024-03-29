using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmlandGuide.Models;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FarmlandGuide.Helpers.Validators;
using NPOI.Util;
using FarmlandGuide.Helpers;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Models.Reports;
using Microsoft.Win32;
using NLog;
using NLog.Targets;
using Employee = FarmlandGuide.Models.Entities.Employee;
using Enterprise = FarmlandGuide.Models.Entities.Enterprise;
using Role = FarmlandGuide.Models.Entities.Role;

namespace FarmlandGuide.ViewModels
{
    public partial class EmployeesPageViewModel : ObservableValidator, IRecipient<EnterpriseTableUpdateMessage>, IRecipient<WorkSessionEditMessage>, IRecipient<WorkSessionDeleteMessage>, IRecipient<WorkSessionAddMessage>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public EmployeesPageViewModel()
        {
            Logger.Trace("EmployeesPageViewModel creating");
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Employees = new ObservableCollection<Employee>(ctx.Employees.Include(e => e.WorkSessions).Include(e => e.Enterprise).Include(e => e.Role).ToList());
            Enterprises = new ObservableCollection<Enterprise>(ctx.Enterprises.ToList());
            Roles = new ObservableCollection<Role>(ctx.Roles.ToList());
            PropertyChanged += EmployeesPageViewModel_PropertyChanged;
            WeakReferenceMessenger.Default.RegisterAll(this);
            Logger.Trace("EmployeesPageViewModel created");
        }
        public void Receive(WorkSessionEditMessage message)
        {
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Logger.Trace("Receiving WorkSessionEditMessage {0}", message.Value);
            Employees = new ObservableCollection<Employee>(ctx.Employees.Include(e => e.WorkSessions).Include(e => e.Enterprise).Include(e => e.Role).ToList());

        }
        public void Receive(WorkSessionAddMessage message)
        {
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Logger.Trace("Receiving WorkSessionAddMessage {0}", message.Value);
            Employees = new ObservableCollection<Employee>(ctx.Employees.Include(e => e.WorkSessions).Include(e => e.Enterprise).Include(e => e.Role).ToList());
        }

        public void Receive(WorkSessionDeleteMessage message)
        {
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Logger.Trace("Receiving WorkSessionDeleteMessage {0}", message.Value);
            Employees = new ObservableCollection<Employee>(ctx.Employees.Include(e => e.WorkSessions).Include(e => e.Enterprise).Include(e => e.Role).ToList());
        }

        public void Receive(EnterpriseTableUpdateMessage message)
        {
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Logger.Trace("Receiving EnterpriseTableUpdateMessage {0}", message.Value);
            Enterprises = new ObservableCollection<Enterprise>(ctx.Enterprises.ToList());
        }


        private void EmployeesPageViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedEmployee) && SelectedEmployee is not null)
                WeakReferenceMessenger.Default.Send(new SelectedEmployeeMessage(SelectedEmployee.Copy()));
        }

        [ObservableProperty] private bool _isEdit;

        [ObservableProperty] private string _titleText;

        [ObservableProperty] private string _buttonApplyText;


        #region Employee
        [ObservableProperty] private ObservableCollection<Employee> _employees;
        [ObservableProperty] private ObservableCollection<Enterprise> _enterprises;
        [ObservableProperty] private ObservableCollection<Role> _roles;
        [ObservableProperty] private Employee? _selectedEmployee;


        private string _name;
        private string _surname;
        private string _patronymic;
        private string _residentialAddress;
        private Enterprise? _selectedEnterprise;
        private string _position;
        private string _workSchedule;
        private decimal _salary;
        private string _employeeName;
        private string _password;
        private Role? _selectedRole;

        [NotEmpty]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, true);
        }

        [NotEmpty]
        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value, true);
        }

        [NotEmpty]
        public string Patronymic
        {
            get => _patronymic;
            set => SetProperty(ref _patronymic, value, true);
        }

        [NotEmpty]
        public string ResidentialAddress
        {
            get => _residentialAddress;
            set => SetProperty(ref _residentialAddress, value, true);
        }

        [NotEmpty]
        public string Position
        {
            get => _position;
            set => SetProperty(ref _position, value, true);
        }

        [NotEmpty]
        public string WorkSchedule
        {
            get => _workSchedule;
            set => SetProperty(ref _workSchedule, value, true);
        }

        [LessOrEqualThenValidation(0)]
        public decimal Salary
        {
            get => _salary;
            set => SetProperty(ref _salary, value, true);
        }

        [NotEmpty]
        [MinLengthWithCustomMessage(4, "Длина логина не может быть меньше 4")]
        public string EmployeeName
        {
            get => _employeeName;
            set => SetProperty(ref _employeeName, value, true);
        }

        [Password(@"^(?=.*\p{Ll})(?=.*\p{Lu})(?=.*\p{N}).{6,}$")]
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value, true);
        }


        [ShouldBeSelected("Необходимо выбрать предприятие")]
        public Enterprise? SelectedEnterprise
        {
            get => _selectedEnterprise;
            set => SetProperty(ref _selectedEnterprise, value, true);
        }

        [ShouldBeSelected("Необходимо выбрать роль пользователя в системе")]
        public Role? SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value, true);
        }
        [RelayCommand]
        private void OnApplyChangesAtEmployees()
        {
            Logger.Debug("Initiated change application attempt");
            ValidateAllProperties();
            if (HasErrors)
            {
                Logger.Warn("Failed change application attempt. Data errors.");
                return;
            }

            if (IsEdit)
                OnEditEmployee();
            else
                OnAddEmployee();
            OnCloseDialogAndClearProps();
        }
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            try
            {
                Logger.Trace("Closing dialog and clear all props");
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
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnAddEmployee()
        {
            try
            {
                Logger.Info("Addition new employee");
                if (SelectedEmployee is null)
                {
                    Logger.Warn("Selected employee is null");
                    return;
                }
                using var ctx = new ApplicationDbContext();
                var salt = PasswordManager.GenerateSalt();
                var passwordHash = PasswordManager.HashPassword(Password, salt);
                var employee = new Employee(Name.Trim(), Surname.Trim(), Patronymic.Trim(), WorkSchedule.Trim(), Position.Trim(), Salary,
                    ResidentialAddress.Trim(), EmployeeName.Trim(), passwordHash, salt, SelectedEnterprise?.EnterpriseId ?? 1, SelectedRole?.RoleId ?? 1)
                {
                    Enterprise = ctx.Enterprises.First(e => e.EnterpriseId == SelectedEnterprise.EnterpriseId),
                    Role = ctx.Roles.First(r => r.RoleId == SelectedRole.RoleId)
                };
                ctx.Employees.Add(employee);
                ctx.SaveChanges();
                Employees.Add(employee);
                Logger.Info("Added new employee: {0}", employee.ToString());
                WeakReferenceMessenger.Default.Send(new EmployeeTableUpdateMessage(employee));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnEditEmployee()
        {
            try
            {
                Logger.Info("Editing employee");
                if (SelectedEmployee is null)
                {
                    Logger.Warn("Selected employee is null");
                    return;
                }
                using var ctx = new ApplicationDbContext();
                var editedEmployee = ctx.Employees.First(e => e.EmployeeId == SelectedEmployee.EmployeeId);
                editedEmployee.Name = Name.Trim();
                editedEmployee.Surname = Surname.Trim();
                editedEmployee.Patronymic = Patronymic.Trim();
                editedEmployee.ResidentialAddress = ResidentialAddress.Trim();
                editedEmployee.Position = Position.Trim();
                editedEmployee.WorkSchedule = WorkSchedule.Trim();
                editedEmployee.Salary = Salary;
                editedEmployee.EmployeeName = EmployeeName.Trim();
                if (!string.IsNullOrEmpty(Password))
                {
                    var salt = PasswordManager.GenerateSalt();
                    var passwordHash = PasswordManager.HashPassword(Password, salt);

                    editedEmployee.PasswordHash = passwordHash;
                    editedEmployee.PasswordSalt = salt;
                }
                editedEmployee.Enterprise = SelectedEnterprise.Copy();
                editedEmployee.Role = SelectedRole.Copy();

                ctx.Employees.Update(editedEmployee);
                ctx.SaveChanges();
                Logger.Info("Edited employee: {0}", editedEmployee.ToString());
                WeakReferenceMessenger.Default.Send(new EmployeeTableUpdateMessage(editedEmployee.Copy()));
                Employees = new ObservableCollection<Employee>(ctx.Employees.Include(e => e.WorkSessions).Include(e => e.Enterprise).Include(e => e.Role).ToList());

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        [RelayCommand]
        public void OnDeleteEmployee()
        {
            try
            {
                Logger.Info("Deleting employee");
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedEmployee is null)
                {
                    Logger.Warn("Employee is unselected");
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.Employees.Remove(SelectedEmployee);
                ctx.SaveChanges();
                WeakReferenceMessenger.Default.Send(new EmployeeTableUpdateMessage(SelectedEmployee));
                Logger.Info("Deleted employee: {0}", SelectedEmployee.ToString());
                Employees.Remove(SelectedEmployee);
                closeDialogCommand.Execute(null, null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        [RelayCommand]
        private void OnOpenEditDialog()
        {
            try
            {
                if (SelectedEmployee is null)
                {
                    IsEdit = false;
                    return;
                }
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
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));

            }
        }

        [RelayCommand]
        private void OnOpenAddDialog()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));

            }
        }

        #endregion

        [RelayCommand]
        private async Task OnPrintReport() => await Task.Run(() =>
        {
            WeakReferenceMessenger.Default.Send(new WaitProcessMessage(true));
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Файл Excel (*.xlsx)|*.xlsx",
                Title = "Выберите куда сохранить файл"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                Logger.Trace("The dialog box is closed. The path is received: {0}", path);
                EmployeesWorkSessionsReportGenerator.GenerateReportToExcel(path);
                Logger.Info("Report on enterprise tasks saved to {path}", path);
            }
            else
            {
                Logger.Trace("The dialog box is closed. Path not received");
            }
            WeakReferenceMessenger.Default.Send(new WaitProcessMessage(false));
        });
    }
}
