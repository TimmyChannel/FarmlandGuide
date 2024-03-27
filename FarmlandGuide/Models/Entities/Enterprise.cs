using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public partial class Enterprise : ObservableObject
    {
        public int EnterpriseID { get; set; }
        [ObservableProperty]
        string _name;
        [ObservableProperty]
        string _address;
        public ICollection<ProductionProcess> ProductionProcesses { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public Enterprise(string name, string address)
        {
            Name = name;
            Address = address;
        }
        public override string ToString()
        {
            return Name; 
        }
    }
}
