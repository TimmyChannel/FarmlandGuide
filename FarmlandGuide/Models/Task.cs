using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string ResponsiblePerson { get; set; } // ФИО ответственного
        public DateTime AssignmentDate { get; set; } // Дата назначения
        public DateTime DueDate { get; set; } // Срок выполнения
        public string Status { get; set; } // Статус задачи
        public string Description { get; set; } // Описание задачи

        // Конструктор
        public Task(int id, string responsiblePerson, DateTime assignmentDate, DateTime dueDate, string status, string description)
        {
            Id = id;
            ResponsiblePerson = responsiblePerson;
            AssignmentDate = assignmentDate;
            DueDate = dueDate;
            Status = status;
            Description = description;
        }
    }
}
