using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmlandGuide.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using FarmlandTask = FarmlandGuide.Models.Entities.Task;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using System.ComponentModel.DataAnnotations;
using FarmlandGuide.Helpers.Validators;
using NPOI.Util;
using FarmlandGuide.Models.Entities;
using FarmlandGuide.Models.Reports;
using Microsoft.Win32;
using NLog;
using NLog.Targets;
using Employee = FarmlandGuide.Models.Entities.Employee;
using Enterprise = FarmlandGuide.Models.Entities.Enterprise;
using ProductionProcess = FarmlandGuide.Models.Entities.ProductionProcess;

namespace FarmlandGuide.ViewModels
{
    public partial class EnterprisesTasksPageViewModel : ObservableValidator, IRecipient<EnterpriseTableUpdateMessage>, IRecipient<ProductionProcessTableUpdate>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public EnterprisesTasksPageViewModel()
        {
            Logger.Trace("EnterprisesTasksPageViewModel creating");
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Statuses = new ObservableCollection<Status>(ctx.Statuses.ToList());
            Enterprises = new ObservableCollection<Enterprise>(ctx.Enterprises.ToList());
            Employees = new ObservableCollection<Employee>(ctx.Employees.ToList());
            Processes = new ObservableCollection<ProductionProcess>(ctx.ProductionProcesses.ToList());
            PropertyChanged += EnterprisesTasksPageViewModel_PropertyChanged;
            WeakReferenceMessenger.Default.RegisterAll(this);
            Logger.Trace("EnterprisesTasksPageViewModel created");
        }
        public void Receive(EnterpriseTableUpdateMessage message)
        {
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Logger.Trace("Receiving EnterpriseTableUpdateMessage {0}", message.Value);
            Enterprises = new ObservableCollection<Enterprise>(ctx.Enterprises.ToList());
        }
        public void Receive(ProductionProcessTableUpdate message)
        {
            Logger.Trace("Receiving ProductionProcessTableUpdate {0}", message.Value);
            if (SelectedEnterprise is not null)
                IsEnterpriseSelected = true;
            else
            {
                IsEnterpriseSelected = false;
                Tasks = new ObservableCollection<FarmlandTask>();
                Employees = new ObservableCollection<Employee>();
                Processes = new ObservableCollection<ProductionProcess>();
                Logger.Warn("Selected enterprise is null");
                return;
            }
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Tasks = new ObservableCollection<FarmlandTask>(ctx.Tasks.Where(t => t.ProductionProcess.EnterpriseId == SelectedEnterprise.EnterpriseId)
                .Include(t => t.Employee).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());
            Employees = new ObservableCollection<Employee>(ctx.Employees.Where(e => e.EnterpriseId == SelectedEnterprise.EnterpriseId).ToList());
            Processes = new ObservableCollection<ProductionProcess>(ctx.ProductionProcesses.Where(pp => pp.EnterpriseId == SelectedEnterprise.EnterpriseId).ToList());
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
                        Tasks = new ObservableCollection<FarmlandTask>();
                        Employees = new ObservableCollection<Employee>();
                        Processes = new ObservableCollection<ProductionProcess>();
                        Logger.Warn("Selected enterprise is null");
                        return;
                    }
                    using var ctx = new ApplicationDbContext();
                    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    Tasks = new ObservableCollection<FarmlandTask>(ctx.Tasks.Where(t => t.ProductionProcess.EnterpriseId == SelectedEnterprise.EnterpriseId)
                        .Include(t => t.Employee).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());
                    Employees = new ObservableCollection<Employee>(ctx.Employees.Where(e => e.EnterpriseId == SelectedEnterprise.EnterpriseId).ToList());
                    Processes = new ObservableCollection<ProductionProcess>(ctx.ProductionProcesses.Where(pp => pp.EnterpriseId == SelectedEnterprise.EnterpriseId).ToList());
                    SortAndFilterTasks();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        [ObservableProperty] private bool _isEnterpriseSelected;
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
        [ObservableProperty] private bool _isEdit;
        [ObservableProperty] private string _titleText;
        [ObservableProperty] private string _buttonApplyText;
        private DateTime _startDate;
        private DateTime _endDate;
        private Employee? _selectedEmployee;
        private ProductionProcess? _selectedProductionProcess;
        private Status? _selectedStatus;
        private string _status;
        private string _description;
        [CustomValidation(typeof(EnterprisesTasksPageViewModel), nameof(ValidateStartDateTime))]
        public DateTime AssignmentDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value, true);
        }
        [CustomValidation(typeof(EnterprisesTasksPageViewModel), nameof(ValidateEndDateTime))]
        public DateTime DueDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value, true);
        }
        [NotEmpty]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value, true);
        }
        [ShouldBeSelected("Необходимо выбрать сотрудника")]
        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value, true);
        }
        [ShouldBeSelected("Необходимо выбрать тип процесса")]
        public ProductionProcess? SelectedProductionProcess
        {
            get => _selectedProductionProcess;
            set => SetProperty(ref _selectedProductionProcess, value, true);
        }
        [ShouldBeSelected("Необходимо выбрать статус задачи")]
        public Status? SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value, true);
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
                    return new ValidationResult("Дата окончания не может быть раньше даты начала");
            }
            return new ValidationResult("Ошибка!");
        }
        public static ValidationResult ValidateStartDateTime(DateTime startDateTime, ValidationContext context)
        {
            EnterprisesTasksPageViewModel instance = (EnterprisesTasksPageViewModel)context.ObjectInstance;
            if (startDateTime == instance.AssignmentDate)
            {
                if (startDateTime <= instance.DueDate)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Дата назначения не может быть позже даты окончания");
            }
            return new ValidationResult("Ошибка!");
        }

        #endregion

        private void SortAndFilterTasks()
        {
            try
            {
                Logger.Trace("Start tasks sorting");
                var notCompletedTasks = Tasks.Where(t => t.Status.Number != 1).OrderBy(t => t.AssignmentDate).ToList();
                var completedTasks = Tasks.Where(t => t.Status.Number == 1).OrderBy(t => t.AssignmentDate).ToList();
                Tasks.Clear();
                foreach (var task in notCompletedTasks.Concat(completedTasks))
                {
                    Tasks.Add(task);
                }
                Logger.Trace("End tasks sorting");

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        [RelayCommand]
        private void OnApplyChangesAtTasks()
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
                Logger.Trace("Closing dialog and clear all props");
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
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnAddTask()
        {
            try
            {
                Logger.Info("Addition new task");
                using var ctx = new ApplicationDbContext();
                var task = new FarmlandTask()
                {
                    Employee = ctx.Employees.Find(SelectedEmployee.EmployeeId),
                    ProductionProcess = ctx.ProductionProcesses.Find(SelectedProductionProcess.ProcessId),
                    Status = ctx.Statuses.Find(SelectedStatus.StatusId),
                    AssignmentDate = AssignmentDate.Copy(),
                    DueDate = DueDate.Copy(),
                    Description = Description.Trim()
                };
                ctx.Tasks.Add(task);
                ctx.SaveChanges();
                Tasks.Add(task);
                Logger.Info("Added new task: {0}", task.ToString());
                WeakReferenceMessenger.Default.Send(new TaskAddMessage(task));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnEditTask()
        {
            try
            {
                Logger.Info("Editing task");
                using var ctx = new ApplicationDbContext();
                var task = ctx.Tasks.First(t => t.TaskId == SelectedTask.TaskId);
                task.AssignmentDate = AssignmentDate.Copy();
                task.DueDate = DueDate.Copy();
                task.Status = SelectedStatus.Copy();
                task.Description = Description.Trim();
                task.Employee = SelectedEmployee.Copy();
                task.ProductionProcess = SelectedProductionProcess.Copy();
                ctx.Tasks.Update(task);
                ctx.SaveChanges();
                Tasks = new ObservableCollection<FarmlandTask>(ctx.Tasks.Where(t => t.ProductionProcess.EnterpriseId == SelectedEnterprise.EnterpriseId)
                    .Include(t => t.Employee).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());
                Logger.Info("Edited task: {0}", task.ToString());
                WeakReferenceMessenger.Default.Send(new TaskEditMessage(task));

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        [RelayCommand]
        public void OnDeleteTask()
        {
            try
            {
                Logger.Info("Deleting task");
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedTask is null)
                {
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.Tasks.Remove(SelectedTask);
                ctx.SaveChanges();
                Logger.Info("Deleted task: {0}", SelectedTask.ToString());
                Tasks.Remove(SelectedTask);
                closeDialogCommand.Execute(null, null);
                WeakReferenceMessenger.Default.Send(new TaskDeleteMessage(SelectedTask));
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
                if (SelectedTask is null)
                {
                    IsEdit = false;
                    return;
                }
                TitleText = "Редактирование задания";
                ButtonApplyText = "Сохранить";
                _endDate=DateTime.MaxValue;
                AssignmentDate = SelectedTask.AssignmentDate;
                DueDate = SelectedTask.DueDate;
                Description = SelectedTask.Description;
                SelectedEmployee = SelectedTask.Employee.Copy();
                SelectedProductionProcess = SelectedTask.ProductionProcess.Copy();
                SelectedStatus = SelectedTask.Status;
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
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        [RelayCommand]
        private async Task OnPrintReport() => await Task.Run(() =>
        {
            WeakReferenceMessenger.Default.Send(new WaitProcessMessage(true));
            if (SelectedEnterprise is null)
            {
                Logger.Warn("Selected enterprise is null");
                WeakReferenceMessenger.Default.Send(new WaitProcessMessage(false));
                return;
            }
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Файл Excel (*.xlsx)|*.xlsx",
                Title = "Выберите куда сохранить файл"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                Logger.Trace("The dialog box is closed. The path is received: {0}", path);
                EnterpriseTaskReportGenerator.GenerateReportToExcel(path, SelectedEnterprise.Copy());
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
