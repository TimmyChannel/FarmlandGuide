using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using NLog;
using NLog.Targets;
using Task = FarmlandGuide.Models.Entities.Task;
using WorkSession = FarmlandGuide.Models.Entities.WorkSession;

namespace FarmlandGuide.ViewModels
{
    public partial class PersonalStatisticsPageViewModel : ObservableObject, IRecipient<LoggedUserMessage>, IRecipient<TaskAddMessage>, IRecipient<TaskEditMessage>, IRecipient<TaskDeleteMessage>,
        IRecipient<WorkSessionAddMessage>, IRecipient<WorkSessionEditMessage>, IRecipient<WorkSessionDeleteMessage>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [ObservableProperty] private ObservableCollection<Task> _tasks;
        [ObservableProperty] private ObservableCollection<WorkSession> _workSessions;
        private int _currentEmployeeId;
        public PersonalStatisticsPageViewModel()
        {
            Logger.Trace("PersonalStatisticsPageViewModel creating");
            Tasks = new ObservableCollection<Task>();
            WorkSessions = new ObservableCollection<WorkSession>();
            WeakReferenceMessenger.Default.RegisterAll(this);
            Logger.Trace("PersonalStatisticsPageViewModel created");
        }

        public void Receive(LoggedUserMessage message)
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                Logger.Trace("Receiving LoggedUserMessage {0}", message.Value);
                _currentEmployeeId = message.Value.EmployeeId;
                Tasks = new ObservableCollection<Task>(ctx.Tasks.Include(t => t.Status).Include(t => t.ProductionProcess).Where(t => t.EmployeeId == _currentEmployeeId).ToList());
                WorkSessions = new ObservableCollection<WorkSession>(ctx.WorkSessions.Where(ws => ws.EmployeeId == _currentEmployeeId).ToList());
                SortAndFilterTasks();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(TaskAddMessage message)
        {
            try
            {
                if (message.Value.EmployeeId == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    Logger.Trace("Receiving TaskAddMessage {0}", message.Value);
                    var task = ctx.Tasks.Include(t => t.Status).First(t => t.TaskId == message.Value.TaskId);
                    Tasks.Add(task);
                    SortAndFilterTasks();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(TaskEditMessage message)
        {
            try
            {
                if (message.Value.EmployeeId == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    Logger.Trace("Receiving TaskEditMessage {0}", message.Value);
                    var task = ctx.Tasks.Include(t => t.Status).First(t => t.TaskId == message.Value.TaskId);
                    var taskToRemove = Tasks.FirstOrDefault(t => t.TaskId == task.TaskId);
                    if (taskToRemove != null)
                        Tasks.Remove(taskToRemove);
                    Tasks.Add(task);
                    SortAndFilterTasks();
                }
                else
                {
                    var taskToRemove = Tasks.FirstOrDefault(t => t.TaskId == message.Value.TaskId);
                    if (taskToRemove != null)
                        Tasks.Remove(taskToRemove);
                    SortAndFilterTasks();

                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(TaskDeleteMessage message)
        {
            try
            {
                if (message.Value.EmployeeId == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    Logger.Trace("Receiving TaskDeleteMessage {0}", message.Value);
                    var taskToRemove = ctx.Tasks.Find(message.Value.TaskId);
                    if (taskToRemove == null)
                        return;
                    Tasks.Remove(taskToRemove);
                    SortAndFilterTasks();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(WorkSessionAddMessage message)
        {
            try
            {
                if (message.Value.EmployeeId == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    Logger.Trace("Receiving WorkSessionAddMessage {0}", message.Value);
                    var session = ctx.WorkSessions.Find(message.Value.SessionId);
                    WorkSessions.Add(session);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(WorkSessionEditMessage message)
        {
            try
            {
                if (message.Value.EmployeeId == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    Logger.Trace("Receiving WorkSessionEditMessage {0}", message.Value);
                    var session = ctx.WorkSessions.Find(message.Value.SessionId);
                    var sessionToRemove = WorkSessions.FirstOrDefault(ws => ws.SessionId == session.SessionId);
                    if (sessionToRemove != null)
                        WorkSessions.Remove(sessionToRemove);

                    WorkSessions.Add(session);
                }
                else
                {
                    var sessionToRemove = WorkSessions.FirstOrDefault(ws => ws.SessionId == message.Value.SessionId);
                    if (sessionToRemove != null)
                        WorkSessions.Remove(sessionToRemove);

                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(WorkSessionDeleteMessage message)
        {
            try
            {
                if (message.Value.EmployeeId == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    Logger.Trace("Receiving WorkSessionDeleteMessage {0}", message.Value);
                    var sessionToRemove = ctx.WorkSessions.Find(message.Value.SessionId);
                    if (sessionToRemove != null)
                        WorkSessions.Remove(sessionToRemove);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

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

    }
}
