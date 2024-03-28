using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public partial class Role : ObservableObject
    {
        [ObservableProperty] private int _roleID;
        [ObservableProperty] private string _name;
        public ICollection<Employee> Employees { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
