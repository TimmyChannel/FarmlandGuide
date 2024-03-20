using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public class ProductionProcess
    {
        public int ProcessID { get; set; }
        public int EnterpriseID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public Enterprise Enterprise { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ProductionProcess(string name, string description, decimal cost)
        {
            Name = name;
            Description = description;
            Cost = cost;
        }
    }
}
