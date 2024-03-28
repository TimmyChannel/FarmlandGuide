using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FarmlandGuide.Models.Entities
{
    public partial class ProductionProcess : ObservableObject
    {
        public int ProcessId { get; set; }
        public int EnterpriseId { get; set; }

        [ObservableProperty] private string _name;
        [ObservableProperty] private string _description;
        [ObservableProperty] private decimal _cost;
        [ObservableProperty] private Enterprise _enterprise;
        public ICollection<Task> Tasks { get; set; }
        public ProductionProcess(string name, string description, decimal cost, int enterpriseId)
        {
            Name = name;
            Description = description;
            Cost = cost;
            EnterpriseId = enterpriseId;
            PropertyChanging += ProductionProcess_PropertyChanging;
            PropertyChanged += ProductionProcess_PropertyChanged;
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
