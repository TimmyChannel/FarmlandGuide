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

namespace FarmlandGuide.ViewModels
{
    public partial class EnterprisesTasksPageViewModel : ObservableObject
    {
        public List<FarmlandTask> Tasks { get => TempModels.Tasks; }
        public List<string> Enterprises => TempModels.Enterprises.Select(x => x.Name).ToList();
        public EnterprisesTasksPageViewModel()
        {
            // Инициализация коллекции задач

            // Пример вызова метода для сортировки и фильтрации (если необходимо)
            SortAndFilterTasks();
        }
        [RelayCommand]
        private void OnPrint()
        {
        }
        // Метод для добавления новой задачи
        public void AddTask(FarmlandTask task)
        {
            Tasks.Add(task);
            SortAndFilterTasks(); // Пересортировать и отфильтровать задачи после добавления новой
        }

        // Метод для редактирования существующей задачи
        public void EditTask(int taskId, FarmlandTask updatedTask)
        {
            var task = Tasks.FirstOrDefault(t => t.TaskID == taskId);
            if (task != null)
            {
                task.EmployeeID = updatedTask.EmployeeID;
                task.AssignmentDate = updatedTask.AssignmentDate;
                task.DueDate = updatedTask.DueDate;
                task.Status = updatedTask.Status;
                task.Description = updatedTask.Description;

                SortAndFilterTasks(); // Пересортировать и отфильтровать задачи после редактирования
            }
        }

        // Метод для удаления задачи
        public void RemoveTask(int taskId)
        {
            var taskToRemove = Tasks.FirstOrDefault(t => t.TaskID == taskId);
            if (taskToRemove != null)
            {
                Tasks.Remove(taskToRemove);
                SortAndFilterTasks(); // Пересортировать и отфильтровать задачи после удаления
            }
        }

        private void SortAndFilterTasks()
        {
            // Сортировка задач: сначала все кроме выполненных, затем выполненные
            var notCompletedTasks = Tasks.Where(t => t.Status != "Исполнено").OrderBy(t => t.AssignmentDate).ToList();
            var completedTasks = Tasks.Where(t => t.Status == "Исполнено").OrderBy(t => t.AssignmentDate).ToList();

            // Очистка текущей коллекции задач
            Tasks.Clear();

            // Добавление задач обратно в коллекцию с учетом нового порядка
            foreach (var task in notCompletedTasks.Concat(completedTasks))
            {
                Tasks.Add(task);
            }
        }
    }
}
