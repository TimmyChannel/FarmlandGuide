using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using FarmlandTask = FarmlandGuide.Models.Task;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using System.Windows.Navigation;
using FarmlandGuide.Helpers;
using System.ComponentModel.DataAnnotations;
using FarmlandGuide.Helpers.Validators;
using NPOI.Util;
using FarmlandGuide.Models.Entities;
using FarmlandGuide.Models.Reports;

namespace FarmlandGuide.ViewModels
{
    public partial class EnterprisesTasksPageViewModel : ObservableValidator, IRecipient<EnterpriseTableUpdateMessage>, IRecipient<ProductionProcessTableUpdate>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public EnterprisesTasksPageViewModel()
        {
            _logger.Trace("EnterprisesTasksPageViewModel creating");
            using var ctx = new ApplicationDbContext();
            Statuses = new(ctx.Statuses.AsNoTracking().ToList());
            Enterprises = new(ctx.Enterprises.AsNoTracking().ToList());
            Employees = new(ctx.Employees.AsNoTracking().ToList());
            Processes = new(ctx.ProductionProcesses.AsNoTracking().ToList());
            this.PropertyChanged += EnterprisesTasksPageViewModel_PropertyChanged;
            WeakReferenceMessenger.Default.RegisterAll(this);
            _logger.Trace("EnterprisesTasksPageViewModel created");
        }
        public void Receive(EnterpriseTableUpdateMessage message)
        {
            using var ctx = new ApplicationDbContext();
            _logger.Trace("Receiving EnterpriseTableUpdateMessage {0}", message.Value);
            Enterprises = new(ctx.Enterprises.ToList());
        }
        public void Receive(ProductionProcessTableUpdate message)
        {
            _logger.Trace("Receiving ProductionProcessTableUpdate {0}", message.Value);
            if (SelectedEnterprise is not null)
                IsEnterpriseSelected = true;
            else
            {
                IsEnterpriseSelected = false;
                Tasks = new();
                Employees = new();
                Processes = new();
                _logger.Warn("Selected enterprise is null");
                return;
            }
            using var ctx = new ApplicationDbContext();
            Tasks = new(ctx.Tasks.AsNoTracking().Where(t => t.ProductionProcess.EnterpriseID == SelectedEnterprise.EnterpriseID)
                .Include(t => t.Employee).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());
            Employees = new(ctx.Employees.AsNoTracking().Where(e => e.EnterpriseID == SelectedEnterprise.EnterpriseID).ToList());
            Processes = new(ctx.ProductionProcesses.AsNoTracking().Where(pp => pp.EnterpriseID == SelectedEnterprise.EnterpriseID).ToList());
            SortAndFilterTasks();
        }


        private void EnterprisesTasksPageViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(SelectedEnterprise))
                {
                    if (SelectedEnterprise is not null)
                        IsEnterpriseSelected = true;
                    else
                    {
                        IsEnterpriseSelected = false;
                        Tasks = new();
                        Employees = new();
                        Processes = new();
                        _logger.Warn("Selected enterprise is null");
                        return;
                    }
                    using var ctx = new ApplicationDbContext();
                    Tasks = new(ctx.Tasks.AsNoTracking().Where(t => t.ProductionProcess.EnterpriseID == SelectedEnterprise.EnterpriseID)
                        .Include(t => t.Employee).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());
                    Employees = new(ctx.Employees.AsNoTracking().Where(e => e.EnterpriseID == SelectedEnterprise.EnterpriseID).ToList());
                    Processes = new(ctx.ProductionProcesses.AsNoTracking().Where(pp => pp.EnterpriseID == SelectedEnterprise.EnterpriseID).ToList());
                    SortAndFilterTasks();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [ObservableProperty]
        bool _isEnterpriseSelected = false;
        [ObservableProperty]
        private ObservableCollection<FarmlandTask> _tasks;
        [ObservableProperty]
        private ObservableCollection<Employee> _employees;
        [ObservableProperty]
        private ObservableCollection<Status> _statuses;
        [ObservableProperty]
        private ObservableCollection<Enterprise> _enterprises;
        [ObservableProperty]
        private ObservableCollection<ProductionProcess> _processes;
        [ObservableProperty]
        private Enterprise? _selectedEnterprise;
        [ObservableProperty]
        private FarmlandTask? _selectedTask;
        [ObservableProperty]
        bool _isEdit = false;
        [ObservableProperty]
        string _titleText;
        [ObservableProperty]
        string _buttonApplyText;
        DateTime _startDate;
        DateTime _endDate;
        Employee? _selectedEmployee;
        ProductionProcess? _selectedProductionProcess;
        Status? _selectedStatus;
        string _status;
        string _description;
        [CustomValidation(typeof(EnterprisesTasksPageViewModel), nameof(ValidatStartDateTime))]
        public DateTime AssignmentDate
        {
            get { return _startDate; }
            set
            {
                SetProperty(ref _startDate, value, true);
            }
        }
        [CustomValidation(typeof(EnterprisesTasksPageViewModel), nameof(ValidateEndDateTime))]
        public DateTime DueDate
        {
            get { return _endDate; }
            set
            {
                SetProperty(ref _endDate, value, true);
            }
        }
        [NotEmpty]
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value, true);
            }
        }
        [ShouldBeSelected("Необходимо выбрать сотрудника")]
        public Employee? SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                SetProperty(ref _selectedEmployee, value, true);
            }
        }
        [ShouldBeSelected("Необходимо выбрать тип процесса")]
        public ProductionProcess? SelectedProductionProcess
        {
            get { return _selectedProductionProcess; }
            set
            {
                SetProperty(ref _selectedProductionProcess, value, true);
            }
        }
        [ShouldBeSelected("Необходимо выбрать статус задачи")]
        public Status? SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                SetProperty(ref _selectedStatus, value, true);
            }
        }


        #region TimeValidators

        public static ValidationResult ValidateEndDateTime(DateTime endDateTime, ValidationContext context)
        {
            EnterprisesTasksPageViewModel instance = (EnterprisesTasksPageViewModel)context.ObjectInstance;
            if (endDateTime == instance.DueDate)
            {
                if (endDateTime >= instance.AssignmentDate)
                    return ValidationResult.Success;
                else
                    return new("Дата окончания не может быть раньше даты начала");
            }
            return new("Ошибка!");
        }
        public static ValidationResult ValidatStartDateTime(DateTime startDateTime, ValidationContext context)
        {
            EnterprisesTasksPageViewModel instance = (EnterprisesTasksPageViewModel)context.ObjectInstance;
            if (startDateTime == instance.AssignmentDate)
            {
                if (startDateTime <= instance.DueDate)
                    return ValidationResult.Success;
                else
                    return new("Дата назначения не может быть позже даты окончания");
            }
            return new("Ошибка!");
        }

        #endregion

        private void SortAndFilterTasks()
        {
            try
            {
                _logger.Trace("Start tasks sorting");
                var notCompletedTasks = Tasks.Where(t => t.Status.Number != 1).OrderBy(t => t.AssignmentDate).ToList();
                var completedTasks = Tasks.Where(t => t.Status.Number == 1).OrderBy(t => t.AssignmentDate).ToList();
                Tasks.Clear();
                foreach (var task in notCompletedTasks.Concat(completedTasks))
                {
                    Tasks.Add(task);
                }
                _logger.Trace("End tasks sorting");

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
        [RelayCommand]
        private void OnApplayChangesAtTasks()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            if (IsEdit)
                OnEditTask();
            else
                OnAddTask();
            SortAndFilterTasks();
            OnCloseDialogAndClearProps();
        }
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            try
            {
                _logger.Trace("Closing dialog and clear all props");
                AssignmentDate = DateTime.Now;
                DueDate = DateTime.Now.AddDays(1);
                Description = string.Empty;
                SelectedEmployee = null;
                SelectedStatus = null;
                SelectedProductionProcess = null;
                SelectedTask = null;
                ClearErrors();
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                closeDialogCommand.Execute(null, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
        private void OnAddTask()
        {
            try
            {
                _logger.Info("Addition new task");
                using var ctx = new ApplicationDbContext();
                var task = new FarmlandTask()
                {
                    Employee = ctx.Employees.Find(SelectedEmployee.EmployeeID),
                    ProductionProcess = ctx.ProductionProcesses.Find(SelectedProductionProcess.ProcessID),
                    Status = ctx.Statuses.Find(SelectedStatus.StatusID),
                    AssignmentDate = AssignmentDate.Copy(),
                    DueDate = DueDate.Copy(),
                    Description = Description.Trim()
                };
                ctx.Tasks.Add(task);
                ctx.SaveChanges();
                Tasks.Add(task);
                _logger.Info("Added new task: {0}", task.ToString());
                WeakReferenceMessenger.Default.Send(new TaskAddMessage(task));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
        private void OnEditTask()
        {
            try
            {
                _logger.Info("Editing task");
                using var ctx = new ApplicationDbContext();
                var task = ctx.Tasks.AsNoTracking().First(t => t.TaskID == SelectedTask.TaskID);
                task.AssignmentDate = AssignmentDate.Copy();
                task.DueDate = DueDate.Copy();
                task.Status = SelectedStatus.Copy();
                task.Description = Description.Trim();
                task.Employee = SelectedEmployee.Copy();
                task.ProductionProcess = SelectedProductionProcess.Copy();
                ctx.Tasks.Update(task);
                ctx.SaveChanges();
                Tasks = new(ctx.Tasks.AsNoTracking().Where(t => t.ProductionProcess.EnterpriseID == SelectedEnterprise.EnterpriseID)
                    .Include(t => t.Employee).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());
                _logger.Info("Edited task: {0}", task.ToString());
                WeakReferenceMessenger.Default.Send(new TaskEditMessage(task));

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [RelayCommand]
        public void OnDeleteTask()
        {
            try
            {
                _logger.Info("Deleting task");
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedTask is null)
                {
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.Tasks.Remove(SelectedTask);
                ctx.SaveChanges();
                _logger.Info("Deleted task: {0}", SelectedTask.ToString());
                Tasks.Remove(SelectedTask);
                closeDialogCommand.Execute(null, null);
                WeakReferenceMessenger.Default.Send(new TaskDeleteMessage(SelectedTask));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [RelayCommand]
        private void OnOpenEditDialog()
        {
            try
            {
                if (SelectedTask is null)
                {
                    IsEdit = false;
                    return;
                }
                TitleText = "Редактирование задания";
                ButtonApplyText = "Сохранить";
                DueDate = SelectedTask.DueDate;
                AssignmentDate = SelectedTask.AssignmentDate;
                Description = SelectedTask.Description;
                SelectedEmployee = SelectedTask.Employee.Copy();
                SelectedProductionProcess = SelectedTask.ProductionProcess.Copy();
                SelectedStatus = SelectedTask.Status;
                IsEdit = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [RelayCommand]
        private void OnOpenAddDialog()
        {
            try
            {
                TitleText = "Добавление нового задания";
                ButtonApplyText = "Добавить";
                IsEdit = false;
                AssignmentDate = DateTime.Now;
                DueDate = DateTime.Now.AddMinutes(1);
                Description = string.Empty;
                SelectedEmployee = null;
                SelectedProductionProcess = null;
                SelectedStatus = null;
                ClearErrors();

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [RelayCommand]
        private void OnPrintReport()
        {
            if (SelectedEnterprise is null)
            {
                _logger.Warn("Selected enterprise is null");
                return;
            }
            EnterpriseTaskReportGenerator.GenerateReportToExcel("file.xlsx", SelectedEnterprise.Copy());
        }
    }
}
