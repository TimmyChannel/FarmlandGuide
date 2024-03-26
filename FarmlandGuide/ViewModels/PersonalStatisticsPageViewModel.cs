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
        [ObservableProperty]
        ObservableCollection<Task> _tasks;
        [ObservableProperty]
        ObservableCollection<WorkSession> _workSessions;
        int _currentEmployeeId;
        public PersonalStatisticsPageViewModel()
        {
            Tasks = new();
            WorkSessions = new();
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(LoggedUserMessage message)
        {
            using var ctx = new ApplicationDbContext();
            _currentEmployeeId = message.Value.EmployeeID;
            Tasks = new(ctx.Tasks.Include(t => t.Status).Include(t => t.ProductionProcess).Where(t => t.EmployeeID == _currentEmployeeId).ToList());
            WorkSessions = new(ctx.WorkSessions.Where(ws => ws.EmployeeID == _currentEmployeeId).ToList());
            SortAndFilterTasks();
        }

        public void Receive(TaskAddMessage message)
        {
            if (message.Value.EmployeeID == _currentEmployeeId)
            {
                using var ctx = new ApplicationDbContext();
                var task = ctx.Tasks.Find(message.Value.TaskID);
                Tasks.Add(task);
                SortAndFilterTasks();

            }
        }

        public void Receive(TaskEditMessage message)
        {
            if (message.Value.EmployeeID == _currentEmployeeId)
            {
                using var ctx = new ApplicationDbContext();
                var task = ctx.Tasks.Find(message.Value.TaskID);
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

        public void Receive(TaskDeleteMessage message)
        {
            if (message.Value.EmployeeID == _currentEmployeeId)
            {
                using var ctx = new ApplicationDbContext();
                var taskToRemove = ctx.Tasks.Find(message.Value.TaskID);
                if (taskToRemove == null) 
                    return;
                Tasks.Remove(taskToRemove);
                SortAndFilterTasks();
            }
        }

        public void Receive(WorkSessionAddMessage message)
        {
            if (message.Value.EmployeeID == _currentEmployeeId)
            {
                using var ctx = new ApplicationDbContext();
                var session = ctx.WorkSessions.Find(message.Value.SessionID);
                WorkSessions.Add(session);
            }
        }

        public void Receive(WorkSessionEditMessage message)
        {
            if (message.Value.EmployeeID == _currentEmployeeId)
            {
                using var ctx = new ApplicationDbContext();
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

        public void Receive(WorkSessionDeleteMessage message)
        {
            if (message.Value.EmployeeID == _currentEmployeeId)
            {
                using var ctx = new ApplicationDbContext();
                var sessionToRemove = ctx.WorkSessions.Find(message.Value.SessionID);
                if (sessionToRemove != null)
                    WorkSessions.Remove(sessionToRemove);
                else 
                    return;
            }
        }

        private void SortAndFilterTasks()
        {
            var notCompletedTasks = Tasks.Where(t => t.Status.Number != 1).OrderBy(t => t.AssignmentDate).ToList();
            var completedTasks = Tasks.Where(t => t.Status.Number == 1).OrderBy(t => t.AssignmentDate).ToList();
            Tasks.Clear();
            foreach (var task in notCompletedTasks.Concat(completedTasks))
            {
                Tasks.Add(task);
            }
        }

    }
}
