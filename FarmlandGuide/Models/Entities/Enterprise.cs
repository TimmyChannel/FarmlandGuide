using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FarmlandGuide.Models.Entities
{
    public partial class Enterprise : ObservableObject
    {
        public int EnterpriseId { get; set; }
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _address;
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
