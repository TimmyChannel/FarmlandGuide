using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public class Enterprise
    {
        public int EnterpriseID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public ICollection<ProductionProcess> ProductionProcesses { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public Enterprise(string name, string address)
        {
            Name = name;
            Address = address;
        }
    }
}
