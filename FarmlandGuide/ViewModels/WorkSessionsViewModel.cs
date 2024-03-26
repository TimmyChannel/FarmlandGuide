using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Helpers.Validators;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.ViewModels
{
    public partial class WorkSessionsViewModel : ObservableValidator, IRecipient<SelectedEmployeeMessage>, IRecipient<EmployeeTableUpdateMessage>
    {
        public WorkSessionsViewModel()
        {
            using var db = new ApplicationDbContext();
            Employees = new(db.Employees.AsNoTracking().ToList());
            WeakReferenceMessenger.Default.RegisterAll(this);
            this.PropertyChanged += WorkSessionsViewModel_PropertyChanged;
        }

        private void WorkSessionsViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedEmployee))
                Debug.WriteLine(SelectedEmployee.ToString());
        }

        [ObservableProperty]
        bool _isEdit = false;

        [ObservableProperty]
        bool _isEmployeeSelected = false;

        [ObservableProperty]
        string _titleText;

        [ObservableProperty]
        string _buttonApplyText;

        [ObservableProperty]
        ObservableCollection<Employee> _employees;

        [ObservableProperty]
        ObservableCollection<WorkSession> _workSessions;
        [ObservableProperty]
        WorkSession _selectedWorkSession;
        DateTime _startDate;
        DateTime _endDate;
        DateTime _startTime;
        DateTime _endTime;
        string _actionType;
        Employee? _selectedEmployee;
        public void Receive(SelectedEmployeeMessage message)
        {
            using var db = new ApplicationDbContext();

            if (message.Value.WorkSessions is not null)
            {
                WorkSessions = new(db.WorkSessions.AsNoTracking().Include(ws => ws.Employee).Where(ws =>
                message.Value.WorkSessions.Select(ws => ws.SessionID).Contains(ws.SessionID)).ToList());
            }
            else
            {
                WorkSessions = new();
            }
            SelectedEmployee = message.Value.Copy();
            IsEmployeeSelected = true;
        }

        public void Receive(EmployeeTableUpdateMessage message)
        {
            using var db = new ApplicationDbContext();
            Employees = new(db.Employees.AsNoTracking().ToList());

        }


        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidatStartDateTime))]
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                SetProperty(ref _startDate, value, true);
            }
        }
        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidatStartDateTime))]
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                SetProperty(ref _startTime, value, true);
            }
        }
        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidateEndDateTime))]
        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                SetProperty(ref _endTime, value, true);
            }
        }
        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidateEndDateTime))]
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                SetProperty(ref _endDate, value, true);
            }
        }
        #region TimeValidators

        public static ValidationResult ValidateEndDateTime(DateTime endDateTime, ValidationContext context)
        {
            WorkSessionsViewModel instance = (WorkSessionsViewModel)context.ObjectInstance;
            if (endDateTime == instance.EndDate)
            {
                if (endDateTime >= instance.StartDate)
                    return ValidationResult.Success;
                else
                    return new("Дата окончания не может быть раньше даты начала");
            }
            if (endDateTime == instance.EndTime)
            {
                if ((endDateTime > instance.StartTime && instance.EndDate == instance.StartDate) || instance.EndDate > instance.StartDate)
                    return ValidationResult.Success;
                else
                    return new("Время окончания не может быть раньше времени начала");
            }
            return new("Ошибка!");
        }
        public static ValidationResult ValidatStartDateTime(DateTime startDateTime, ValidationContext context)
        {
            WorkSessionsViewModel instance = (WorkSessionsViewModel)context.ObjectInstance;
            if (startDateTime == instance.StartDate)
            {
                if (startDateTime <= instance.EndDate)
                    return ValidationResult.Success;
                else
                    return new("Дата начала не может быть позже даты окончания");
            }
            if (startDateTime == instance.StartTime)
            {
                if ((startDateTime < instance.EndDate && instance.EndDate == instance.StartDate) || instance.EndDate > instance.StartDate)
                    return ValidationResult.Success;
                else
                    return new("Время начала не может быть позже времени окончания");
            }
            return new("Ошибка!");
        }

        #endregion

        [NotEmpty]
        public string ActionType
        {
            get { return _actionType; }
            set
            {
                SetProperty(ref _actionType, value, true);
            }
        }

        [RelayCommand]
        private void OnApplayChangesAtWorkSessions()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            if (IsEdit)
                OnEditWorkSession();
            else
                OnAddWorkSession();
            OnCloseDialogAndClearProps();
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
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            IsEdit = false;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(1);
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue.AddHours(12);
            ActionType = string.Empty;
            SelectedEmployee = null;
            ClearErrors();
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);
        }
        private void OnAddWorkSession()
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                var session = new WorkSession()
                {
                    StartDateTime = StartDate.Add(StartTime.TimeOfDay),
                    EndDateTime = EndDate.Add(EndTime.TimeOfDay),
                    Type = ActionType.Trim(),
                    Employee = ctx.Employees.First(e => e.EmployeeID == SelectedEmployee.EmployeeID),
                };
                ctx.WorkSessions.Add(session);
                ctx.SaveChanges();
                WeakReferenceMessenger.Default.Send(new WorkSessionAddMessage(session));
                WorkSessions.Add(session);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        private void OnEditWorkSession()
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                Debug.WriteLine($"Start edititng Work Session ID: {SelectedWorkSession.SessionID} StartTime: {SelectedWorkSession.StartDateTime}" +
                    $"EndTime: {SelectedWorkSession.EndDateTime}\n Action Type: {SelectedWorkSession.Type} EmployeeID: {SelectedWorkSession.EmployeeID}\n" +
                    $"New Data: ID: {SelectedWorkSession.SessionID} StartTime: {StartDate.Add(StartTime.TimeOfDay)}" +
                    $"EndTime: {EndDate.Add(EndTime.TimeOfDay)}\n Action Type: {ActionType} EmployeeID: {SelectedEmployee.EmployeeID}\n");

                var editedWorkSession = ctx.WorkSessions.AsNoTracking().First(ws => ws.SessionID == SelectedWorkSession.SessionID);
                editedWorkSession.StartDateTime = StartDate.Add(StartTime.TimeOfDay).Copy();
                editedWorkSession.EndDateTime = EndDate.Add(EndTime.TimeOfDay).Copy();
                editedWorkSession.Type = ActionType.Trim();
                int previousEmployeeID = editedWorkSession.EmployeeID;
                editedWorkSession.Employee = ctx.Employees.AsNoTracking().First(e => e.EmployeeID == SelectedEmployee.EmployeeID);
                ctx.WorkSessions.Update(editedWorkSession);
                ctx.SaveChanges();

                Debug.WriteLine($"Edit completed Work Session ID: {editedWorkSession.SessionID} StartTime: {editedWorkSession.StartDateTime}" +
    $"EndTime: {editedWorkSession.StartDateTime}\n Action Type: {editedWorkSession.Type} EmployeeID: {editedWorkSession.EmployeeID}");

                WorkSessions = new(ctx.WorkSessions.AsNoTracking().Include(ws => ws.Employee).Where(ws => ws.EmployeeID == previousEmployeeID).ToList());
                WeakReferenceMessenger.Default.Send(new WorkSessionEditMessage(editedWorkSession.Copy()));

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }

        [RelayCommand]
        public void OnDeleteWorkSession()
        {
            try
            {
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedWorkSession is null)
                {
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.WorkSessions.Remove(SelectedWorkSession);
                ctx.SaveChanges();
                WeakReferenceMessenger.Default.Send(new WorkSessionDeleteMessage(SelectedWorkSession));
                WorkSessions.Remove(SelectedWorkSession);
                closeDialogCommand.Execute(null, null);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }

        [RelayCommand]
        private void OnOpenEditDialog()
        {
            if (SelectedWorkSession is null)
            {
                IsEdit = false;
                return;
            }
            TitleText = "Редактирование рабочей сессии";
            ButtonApplyText = "Сохранить";
            EndDate = SelectedWorkSession.EndDateTime.Date;
            StartDate = SelectedWorkSession.StartDateTime.Date;
            EndTime = new DateTime(1, 1, 1, SelectedWorkSession.EndDateTime.Hour, SelectedWorkSession.EndDateTime.Minute, 0);
            StartTime = new DateTime(1, 1, 1, SelectedWorkSession.StartDateTime.Hour, SelectedWorkSession.StartDateTime.Minute, 0);
            ActionType = SelectedWorkSession.Type;
            SelectedEmployee = SelectedWorkSession.Employee;
            IsEdit = true;
        }

        [RelayCommand]
        private void OnOpenAddDialog()
        {
            TitleText = "Добавление новой рабочей сессии";
            ButtonApplyText = "Добавить";
            IsEdit = false;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(1);
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue.AddHours(12);
            ActionType = string.Empty;
            SelectedEmployee = null;
            ClearErrors();
        }

    }
}
