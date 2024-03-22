using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmlandGuide.Models;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FarmlandGuide.ViewModels
{
    public partial class EnterpisesPageViewModel : ObservableObject, IDataErrorInfo
    {
        bool _hadValue = false;
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case nameof(Name):
                        if (_hadValue && string.IsNullOrWhiteSpace((Name ?? "").ToString()))
                            error = "Название не может быть пустым.";
                        break;
                    case nameof(Address):
                        if (_hadValue && string.IsNullOrWhiteSpace((Address ?? "").ToString()))
                            error = "Адрес не может быть пустым.";
                        break;
                }
                _hadValue = true;
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        public EnterpisesPageViewModel()
        {
            using var ctx = new ApplicationDbContext();
            Enterprises = new(ctx.Enterprises);
            PropertyChanged += EnterpisesPageViewModel_PropertyChanged;
        }

        private void EnterpisesPageViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedEnterprise) && SelectedEnterprise is not null)
            {
                Name = SelectedEnterprise.Name;
                Address = SelectedEnterprise.Address;
            }
        }

        [ObservableProperty]
        Enterprise? _selectedEnterprise;
        [ObservableProperty]
        ObservableCollection<Enterprise> _enterprises;
        [ObservableProperty]
        string _name;
        [ObservableProperty]
        string _address;

        [RelayCommand]
        private void OnAddEnterprise()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Address)) return;

            using var ctx = new ApplicationDbContext();
            var enterpise = new Enterprise(Name, Address);
            ctx.Enterprises.Add(enterpise);
            ctx.SaveChanges();
            Enterprises.Add(enterpise);
            Debug.WriteLine($"Added enterprise name: {Name} and address: {Address}");

            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);

            _hadValue = false;
            Name = string.Empty;
            Address = string.Empty;
        }

        [RelayCommand]
        private void OnEditEnterprise()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Address) || SelectedEnterprise is null) return;

            using var ctx = new ApplicationDbContext();

            SelectedEnterprise.Name = Name.Copy();
            SelectedEnterprise.Address = Address.Copy();
            ctx.Enterprises.Update(SelectedEnterprise);
            ctx.SaveChanges();
            Debug.WriteLine($"Edited enterprise name: {Name} and address: {Address}");

            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);

            _hadValue = false;
            Name = string.Empty;
            Address = string.Empty;
        }

        [RelayCommand]
        private void OnDeleteEnterprise()
        {
            if (SelectedEnterprise is null)
                return;
            using var ctx = new ApplicationDbContext();
            ctx.Enterprises.Remove(SelectedEnterprise);
            ctx.SaveChanges();
            Enterprises.Remove(SelectedEnterprise);

            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);

        }
    }
}
