using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models.Entities
{
    public class Status
    {
        public int StatusID { get; set; }
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
