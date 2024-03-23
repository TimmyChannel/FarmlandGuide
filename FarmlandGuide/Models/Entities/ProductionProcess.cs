using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public partial class ProductionProcess : ObservableObject
    {
        public int ProcessID { get; set; }
        public int EnterpriseID { get; set; }
        [ObservableProperty]
        string _name;
        [ObservableProperty]
        string _description;
        [ObservableProperty]
        decimal _cost;
        [ObservableProperty]
        Enterprise _enterprise;
        public ICollection<Task> Tasks { get; set; }
        public ProductionProcess(string name, string description, decimal cost)
        {
            Name = name;
            Description = description;
            Cost = cost;
        }
    }
}
