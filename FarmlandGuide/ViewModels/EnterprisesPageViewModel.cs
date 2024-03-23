using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmlandGuide.Helpers.Validators;
using FarmlandGuide.Models;
using NPOI.OpenXmlFormats.Dml.Diagram;
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
    public partial class EnterpisesPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        bool _isEdit = false;

        public EnterpisesPageViewModel()
        {
            using var ctx = new ApplicationDbContext();
            Enterprises = new(ctx.Enterprises);
        }

        [ObservableProperty]
        string _titleText;

        [ObservableProperty]
        string _buttonApplyText;

        [ObservableProperty]
        Enterprise? _selectedEnterprise;

        [ObservableProperty]
        ObservableCollection<Enterprise> _enterprises;

        string _name;
        string _address;

        [NotEmpty]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value, true);
            }
        }

        [NotEmpty]
        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value, true);
            }
        }

        [RelayCommand]
        private void OnApplayChangesAtEnterprises()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            if (IsEdit)
                OnEditEnterprise();
            else
                OnAddEnterprise();
            OnCloseDialogAndClearProps();
        }
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            IsEdit = false;
            Name = string.Empty;
            Address = string.Empty;
            ClearErrors();
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);
        }
        private void OnAddEnterprise()
        {
            using var ctx = new ApplicationDbContext();
            var enterpise = new Enterprise(Name.Trim(), Address.Trim());
            ctx.Enterprises.Add(enterpise);
            ctx.SaveChanges();
            Enterprises.Add(enterpise);
            Debug.WriteLine($"Added enterprise name: {Name} and address: {Address}");
        }

        private void OnEditEnterprise()
        {
            using var ctx = new ApplicationDbContext();
            SelectedEnterprise.Name = Name.Trim().Copy();
            SelectedEnterprise.Address = Address.Trim().Copy();
            ctx.Enterprises.Update(SelectedEnterprise);
            ctx.SaveChanges();
            Debug.WriteLine($"Edited enterprise name: {Name} and address: {Address}");
        }

        [RelayCommand]
        private void OnDeleteEnterprise()
        {
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            if (SelectedEnterprise is null)
            {
                closeDialogCommand.Execute(null, null);
                return;
            }
            using var ctx = new ApplicationDbContext();
            ctx.Enterprises.Remove(SelectedEnterprise);
            ctx.SaveChanges();
            Enterprises.Remove(SelectedEnterprise);
            closeDialogCommand.Execute(null, null);
        }
        [RelayCommand]
        private void OnOpenEditDialog()
        {
            if (SelectedEnterprise is null)
                return;
            TitleText = "Редактирование производственного процесса";
            ButtonApplyText = "Сохранить";
            Name = SelectedEnterprise.Name;
            Address = SelectedEnterprise.Address;
            IsEdit = true;
        }
        [RelayCommand]
        private void OnOpenAddDialog()
        {
            TitleText = "Добавление нового производственного процесса";
            ButtonApplyText = "Добавить";
            IsEdit = false;
            Name = string.Empty;
            Address = string.Empty;
            ClearErrors();
        }
    }
}
