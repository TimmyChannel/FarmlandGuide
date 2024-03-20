using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int EnterpriseID { get; set; }
        public int RoleID { get; set; }
        public string EmployeeName { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public string FIO { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string ResidentialAddress { get; set; }
        public string WorkSchedule { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public Role Role { get; set; }
        public Enterprise Enterprise { get; set; }
        public ICollection<WorkSession> WorkSessions { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public Employee(string name, string surname, string patronymic, string position, string workSchedule)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            FIO = name + ' ' + surname + ' ' + patronymic;
            Position = position;
            WorkSchedule = workSchedule;
        }
    }
}
