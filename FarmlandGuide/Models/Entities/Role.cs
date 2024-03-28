using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FarmlandGuide.Models.Entities
{
    public partial class Role : ObservableObject
    {
        [ObservableProperty] private int _roleId;
        [ObservableProperty] private string _name;
        public ICollection<Employee> Employees { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
