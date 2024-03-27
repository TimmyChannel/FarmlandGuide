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
        public ProductionProcess(string name, string description, decimal cost, int enterpriseID)
        {
            Name = name;
            Description = description;
            Cost = cost;
            EnterpriseID = enterpriseID;
            this.PropertyChanging += ProductionProcess_PropertyChanging;
            this.PropertyChanged += ProductionProcess_PropertyChanged;
        }

        private void ProductionProcess_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName == nameof(Enterprise) && Enterprise is not null)
            {
                Enterprise.PropertyChanged -= Enterprise_PropertyChanged;
            }
        }

        private void ProductionProcess_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Enterprise) && Enterprise is not null)
            {
                Enterprise.PropertyChanged += Enterprise_PropertyChanged;
            }
        }

        private void Enterprise_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Enterprise));
        }
        public override string ToString()
        {
            return Name + " " + Cost;
        }
    }
}
