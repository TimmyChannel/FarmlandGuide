using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Task = FarmlandGuide.Models.Task;

namespace FarmlandGuide.ViewModels
{
    public class EnterprisesTasksPageViewModel
    {
        public List<Task> Tasks { get => TempModels.Tasks; }
        public List<string> Enterprises => TempModels.Enterprises.Select(x => x.Name).ToList(); 
        public EnterprisesTasksPageViewModel()
        {
            // Инициализация коллекции задач

            // Пример вызова метода для сортировки и фильтрации (если необходимо)
            SortAndFilterTasks();
        }

        // Метод для добавления новой задачи
        public void AddTask(Task task)
        {
            Tasks.Add(task);
            SortAndFilterTasks(); // Пересортировать и отфильтровать задачи после добавления новой
        }

        // Метод для редактирования существующей задачи
        public void EditTask(int taskId, Task updatedTask)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.ResponsiblePerson = updatedTask.ResponsiblePerson;
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
            var taskToRemove = Tasks.FirstOrDefault(t => t.Id == taskId);
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
