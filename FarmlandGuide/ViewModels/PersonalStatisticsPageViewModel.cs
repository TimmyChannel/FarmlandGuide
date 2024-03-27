using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = FarmlandGuide.Models.Task;

namespace FarmlandGuide.ViewModels
{
    public partial class PersonalStatisticsPageViewModel : ObservableObject, IRecipient<LoggedUserMessage>, IRecipient<TaskAddMessage>, IRecipient<TaskEditMessage>, IRecipient<TaskDeleteMessage>,
        IRecipient<WorkSessionAddMessage>, IRecipient<WorkSessionEditMessage>, IRecipient<WorkSessionDeleteMessage>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [ObservableProperty]
        ObservableCollection<Task> _tasks;
        [ObservableProperty]
        ObservableCollection<WorkSession> _workSessions;
        int _currentEmployeeId;
        public PersonalStatisticsPageViewModel()
        {
            _logger.Trace("PersonalStatisticsPageViewModel creating");
            Tasks = new();
            WorkSessions = new();
            WeakReferenceMessenger.Default.RegisterAll(this);
            _logger.Trace("PersonalStatisticsPageViewModel created");
        }

        public void Receive(LoggedUserMessage message)
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                _logger.Trace("Receiving LoggedUserMessage {0}", message.Value);
                _currentEmployeeId = message.Value.EmployeeID;
                Tasks = new(ctx.Tasks.AsNoTracking().Include(t => t.Status).Include(t => t.ProductionProcess).Where(t => t.EmployeeID == _currentEmployeeId).ToList());
                WorkSessions = new(ctx.WorkSessions.AsNoTracking().Where(ws => ws.EmployeeID == _currentEmployeeId).ToList());
                SortAndFilterTasks();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        public void Receive(TaskAddMessage message)
        {
            try
            {
                if (message.Value.EmployeeID == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    _logger.Trace("Receiving TaskAddMessage {0}", message.Value);
                    var task = ctx.Tasks.AsNoTracking().Include(t => t.Status).First(t => t.TaskID == message.Value.TaskID);
                    Tasks.Add(task);
                    SortAndFilterTasks();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        public void Receive(TaskEditMessage message)
        {
            try
            {
                if (message.Value.EmployeeID == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    _logger.Trace("Receiving TaskEditMessage {0}", message.Value);
                    var task = ctx.Tasks.AsNoTracking().Include(t => t.Status).First(t => t.TaskID == message.Value.TaskID);
                    var taskToRemove = Tasks.FirstOrDefault(t => t.TaskID == task.TaskID);
                    if (taskToRemove != null)
                        Tasks.Remove(taskToRemove);
                    Tasks.Add(task);
                    SortAndFilterTasks();
                }
                else
                {
                    var taskToRemove = Tasks.FirstOrDefault(t => t.TaskID == message.Value.TaskID);
                    if (taskToRemove != null)
                        Tasks.Remove(taskToRemove);
                    SortAndFilterTasks();

                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        public void Receive(TaskDeleteMessage message)
        {
            try
            {
                if (message.Value.EmployeeID == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    _logger.Trace("Receiving TaskDeleteMessage {0}", message.Value);
                    var taskToRemove = ctx.Tasks.Find(message.Value.TaskID);
                    if (taskToRemove == null)
                        return;
                    Tasks.Remove(taskToRemove);
                    SortAndFilterTasks();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        public void Receive(WorkSessionAddMessage message)
        {
            try
            {
                if (message.Value.EmployeeID == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    _logger.Trace("Receiving WorkSessionAddMessage {0}", message.Value);
                    var session = ctx.WorkSessions.Find(message.Value.SessionID);
                    WorkSessions.Add(session);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        public void Receive(WorkSessionEditMessage message)
        {
            try
            {
                if (message.Value.EmployeeID == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    _logger.Trace("Receiving WorkSessionEditMessage {0}", message.Value);
                    var session = ctx.WorkSessions.Find(message.Value.SessionID);
                    var sessionToRemove = WorkSessions.FirstOrDefault(ws => ws.SessionID == session.SessionID);
                    if (sessionToRemove != null)
                        WorkSessions.Remove(sessionToRemove);

                    WorkSessions.Add(session);
                }
                else
                {
                    var sessionToRemove = WorkSessions.FirstOrDefault(ws => ws.SessionID == message.Value.SessionID);
                    if (sessionToRemove != null)
                        WorkSessions.Remove(sessionToRemove);

                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        public void Receive(WorkSessionDeleteMessage message)
        {
            try
            {
                if (message.Value.EmployeeID == _currentEmployeeId)
                {
                    using var ctx = new ApplicationDbContext();
                    _logger.Trace("Receiving WorkSessionDeleteMessage {0}", message.Value);
                    var sessionToRemove = ctx.WorkSessions.Find(message.Value.SessionID);
                    if (sessionToRemove != null)
                        WorkSessions.Remove(sessionToRemove);
                    else
                        return;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

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

    }
}
