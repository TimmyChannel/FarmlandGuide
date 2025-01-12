﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Helpers.Validators;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using NPOI.Util;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using NLog;
using NLog.Targets;
using Employee = FarmlandGuide.Models.Entities.Employee;
using WorkSession = FarmlandGuide.Models.Entities.WorkSession;

namespace FarmlandGuide.ViewModels
{
    public partial class WorkSessionsViewModel : ObservableValidator, IRecipient<SelectedEmployeeMessage>, IRecipient<EmployeeTableUpdateMessage>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public WorkSessionsViewModel()
        {
            Logger.Trace("WorkSessionsViewModel creating");
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Employees = new ObservableCollection<Employee>(ctx.Employees.ToList());
            WeakReferenceMessenger.Default.RegisterAll(this);
            PropertyChanged += WorkSessionsViewModel_PropertyChanged;
            Logger.Trace("WorkSessionsViewModel` created");
        }

        private void WorkSessionsViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(SelectedEmployee))
                {
                    SelectedEmployeeIndex = Employees.IndexOf(SelectedEmployee ?? Employees.First());
                    if (SelectedEmployee is not null)
                        Debug.WriteLine(SelectedEmployee.ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        [ObservableProperty] private int _selectedEmployeeIndex;

        [ObservableProperty] private bool _isEdit;

        [ObservableProperty] private bool _isEmployeeSelected;

        [ObservableProperty] private string _titleText;

        [ObservableProperty] private string _buttonApplyText;

        [ObservableProperty] private ObservableCollection<Employee> _employees;

        [ObservableProperty] private ObservableCollection<WorkSession> _workSessions;
        [ObservableProperty] private WorkSession _selectedWorkSession;
        private DateTime _startDate;
        private DateTime _endDate;
        private DateTime _startTime;
        private DateTime _endTime;
        private string _actionType;
        private Employee? _selectedEmployee;
        public void Receive(SelectedEmployeeMessage message)
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                Logger.Trace("Receiving SelectedEmployeeMessage {0}", message.Value);
                if (message.Value.WorkSessions is not null)
                {
                    WorkSessions = new ObservableCollection<WorkSession>(ctx.WorkSessions.Include(ws => ws.Employee).Where(ws =>
                    message.Value.WorkSessions.Select(ws => ws.SessionId).Contains(ws.SessionId)).ToList());
                }
                else
                {
                    WorkSessions = new ObservableCollection<WorkSession>();
                }
                SelectedEmployee = message.Value.Copy();
                IsEmployeeSelected = true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(EmployeeTableUpdateMessage message)
        {
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            Employees = new ObservableCollection<Employee>(ctx.Employees.ToList());
            Logger.Trace("Receiving EmployeeTableUpdateMessage {0}", message.Value);
        }


        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidateStartDateTime))]
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value, true);
        }
        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidateStartDateTime))]
        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value, true);
        }
        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidateEndDateTime))]
        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value, true);
        }
        [CustomValidation(typeof(WorkSessionsViewModel), nameof(ValidateEndDateTime))]
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value, true);
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
                    return new ValidationResult("Дата окончания не может быть раньше даты начала");
            }
            if (endDateTime == instance.EndTime)
            {
                if ((endDateTime > instance.StartTime && instance.EndDate == instance.StartDate) || instance.EndDate > instance.StartDate)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Время окончания не может быть раньше времени начала");
            }
            return new ValidationResult("Ошибка!");
        }
        public static ValidationResult ValidateStartDateTime(DateTime startDateTime, ValidationContext context)
        {
            WorkSessionsViewModel instance = (WorkSessionsViewModel)context.ObjectInstance;
            if (startDateTime == instance.StartDate)
            {
                if (startDateTime <= instance.EndDate)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Дата начала не может быть позже даты окончания");
            }
            if (startDateTime == instance.StartTime)
            {
                if ((startDateTime < instance.EndDate && instance.EndDate == instance.StartDate) || instance.EndDate > instance.StartDate)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Время начала не может быть позже времени окончания");
            }
            return new ValidationResult("Ошибка!");
        }

        #endregion

        [NotEmpty]
        public string ActionType
        {
            get => _actionType;
            set => SetProperty(ref _actionType, value, true);
        }

        [RelayCommand]
        private void OnApplyChangesAtWorkSessions()
        {
            Logger.Debug("Initiated change application attempt");
            ValidateAllProperties();
            if (HasErrors)
            {
                Logger.Warn("Failed change application attempt. Data errors.");
                return;
            }

            if (IsEdit)
                OnEditWorkSession();
            else
                OnAddWorkSession();
            OnCloseDialogAndClearProps();
        }
        [ShouldBeSelected("Необходимо выбрать сотрудника")]
        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value, true);
        }
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            try
            {
                Logger.Trace("Closing dialog and clear all props");
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
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnAddWorkSession()
        {
            try
            {
                Logger.Info("Addition new session");
                using var ctx = new ApplicationDbContext();
                var session = new WorkSession()
                {
                    StartDateTime = StartDate.Add(StartTime.TimeOfDay),
                    EndDateTime = EndDate.Add(EndTime.TimeOfDay),
                    Type = ActionType.Trim(),
                };
                var employee = ctx.Employees.First(e => e.EmployeeId == SelectedEmployee.EmployeeId);
                session.Employee = employee;
                ctx.WorkSessions.Add(session);
                ctx.SaveChanges();
                Logger.Info("Added new session: {0}", session.ToString());
                WeakReferenceMessenger.Default.Send(new WorkSessionAddMessage(session));
                WorkSessions.Add(session);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnEditWorkSession()
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                Logger.Info("Editing session");
                var editedWorkSession = ctx.WorkSessions.First(ws => ws.SessionId == SelectedWorkSession.SessionId);
                editedWorkSession.StartDateTime = StartDate.Add(StartTime.TimeOfDay).Copy();
                editedWorkSession.EndDateTime = EndDate.Add(EndTime.TimeOfDay).Copy();
                editedWorkSession.Type = ActionType.Trim();
                int previousEmployeeId = editedWorkSession.EmployeeId;
                editedWorkSession.Employee = ctx.Employees.First(e => e.EmployeeId == SelectedEmployee.EmployeeId);
                ctx.WorkSessions.Update(editedWorkSession);
                ctx.SaveChanges();

                Logger.Info("Edited session: {0}", editedWorkSession.ToString());

                WorkSessions = new ObservableCollection<WorkSession>(ctx.WorkSessions.Include(ws => ws.Employee).Where(ws => ws.EmployeeId == previousEmployeeId).ToList());
                WeakReferenceMessenger.Default.Send(new WorkSessionEditMessage(editedWorkSession.Copy()));

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }

        }

        [RelayCommand]
        public void OnDeleteWorkSession()
        {
            try
            {
                Logger.Info("Deleting session");
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedWorkSession is null)
                {
                    Logger.Warn("Session is unselected");
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.WorkSessions.Remove(SelectedWorkSession);
                ctx.SaveChanges();
                WeakReferenceMessenger.Default.Send(new WorkSessionDeleteMessage(SelectedWorkSession));
                Logger.Info("Deleted session: {0}", SelectedWorkSession.ToString());
                WorkSessions.Remove(SelectedWorkSession);
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
                if (SelectedWorkSession is null)
                {
                    IsEdit = false;
                    return;
                }
                TitleText = "Редактирование рабочей сессии";
                ButtonApplyText = "Сохранить";
                _endDate=DateTime.MaxValue;;
                _endTime=DateTime.MaxValue;;
                StartDate = SelectedWorkSession.StartDateTime.Date;
                StartTime = new DateTime(1, 1, 1, SelectedWorkSession.StartDateTime.Hour, SelectedWorkSession.StartDateTime.Minute, 0);
                EndTime = new DateTime(1, 1, 1, SelectedWorkSession.EndDateTime.Hour, SelectedWorkSession.EndDateTime.Minute, 0);
                EndDate = SelectedWorkSession.EndDateTime.Date;
                ActionType = SelectedWorkSession.Type;
                SelectedEmployee = SelectedWorkSession.Employee;
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
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

    }
}
