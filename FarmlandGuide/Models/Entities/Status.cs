using System.Collections.Generic;

namespace FarmlandGuide.Models.Entities
{
    public class Status
    {
        public int StatusId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public Status(string name, int number)
        {
            Name = name;
            Number = number;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
