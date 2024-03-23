using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmlandGuide.Helpers.Validators;
using FarmlandGuide.Models;
using NPOI.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.ViewModels
{
    public partial class ProcessesPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        bool _isEdit = false;

        public ProcessesPageViewModel()
        {
            using var ctx = new ApplicationDbContext();
            ProductionProcesses = new(ctx.ProductionProcesses);
            Enterprises = new(ctx.Enterprises);
        }

        [ObservableProperty]
        string _titleText;
        [ObservableProperty]
        string _buttonApplyText;
        ObservableCollection<Enterprise> _enterprises;
        ProductionProcess? _selectedProductionProcess;
        Enterprise? _selectedEnterprise;
        ObservableCollection<ProductionProcess> _productionProcesses;
        string _name;
        string _description;
        decimal _cost;

        public ObservableCollection<Enterprise> Enterprises
        {
            get { return _enterprises; }
            set
            {
                SetProperty(ref _enterprises, value, true);
            }
        }
        public ProductionProcess? SelectedProductionProcess
        {
            get { return _selectedProductionProcess; }
            set
            {
                SetProperty(ref _selectedProductionProcess, value, true);
            }
        }

        [ShouldBeSelected("Необходимо выбрать пердприятие")]
        public Enterprise? SelectedEnterprise
        {
            get { return _selectedEnterprise; }
            set
            {
                SetProperty(ref _selectedEnterprise, value, true);
            }
        }

        public ObservableCollection<ProductionProcess> ProductionProcesses
        {
            get { return _productionProcesses; }
            set
            {
                SetProperty(ref _productionProcesses, value, true);
            }
        }

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
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value, true);
            }
        }
        [LessOrEqualThenValidation(0)]
        public decimal Cost
        {
            get { return _cost; }
            set
            {
                SetProperty(ref _cost, value, true);
            }
        }

        [RelayCommand]
        private void OnApplayChangesAtProductionProcess()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            if (IsEdit)
                OnEditProductionProcess();
            else
                OnAddProductionProcess();
            OnCloseDialogAndClearProps();
        }
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            IsEdit = false;
            Name = string.Empty;
            Description = string.Empty;
            Cost = 0;
            SelectedEnterprise = null;
            ClearErrors();
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);
        }
        private void OnAddProductionProcess()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var process = new ProductionProcess(Name, Description, Cost)
                {
                    EnterpriseID = SelectedEnterprise?.EnterpriseID ?? 1,
                    Enterprise = ctx.Enterprises.First(e => e.EnterpriseID == SelectedEnterprise.EnterpriseID)
                };
                ctx.ProductionProcesses.Add(process);
                ctx.SaveChanges();
                ProductionProcesses.Add(process);
            }
            Debug.WriteLine($"Added production process: {Name} {Description} {Cost} EnterpriseID: {SelectedEnterprise}");
        }
        private void OnEditProductionProcess()
        {
            using (var ctx = new ApplicationDbContext())
            {
                SelectedProductionProcess.Name = Name.Copy();
                SelectedProductionProcess.Description = Description.Copy();
                SelectedProductionProcess.Cost = Cost;
                SelectedProductionProcess.Enterprise = ctx.Enterprises.First(e => e.EnterpriseID == SelectedEnterprise.EnterpriseID);
                SelectedProductionProcess.EnterpriseID = SelectedEnterprise?.EnterpriseID ?? 1;

                ctx.ProductionProcesses.Update(SelectedProductionProcess);
                ctx.SaveChanges();
            }
            Debug.WriteLine($"Edited production process: {Name} {Description} {Cost}");
        }

        [RelayCommand]
        private void OnDeleteProductionProcess()
        {
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            if (SelectedProductionProcess is null)
            {
                closeDialogCommand.Execute(null, null);
                return;
            }
            using var ctx = new ApplicationDbContext();
            ctx.ProductionProcesses.Remove(SelectedProductionProcess);
            ctx.SaveChanges();
            ProductionProcesses.Remove(SelectedProductionProcess);
            closeDialogCommand.Execute(null, null);
        }

        [RelayCommand]
        private void OnOpenEditDialog()
        {
            if (SelectedProductionProcess is null)
                return;
            TitleText = "Редактирование производственного процесса";
            ButtonApplyText = "Сохранить";
            Name = SelectedProductionProcess.Name;
            Description = SelectedProductionProcess.Description;
            Cost = SelectedProductionProcess.Cost;
            SelectedEnterprise = SelectedProductionProcess.Enterprise;
            IsEdit = true;
        }
        [RelayCommand]
        private void OnOpenAddDialog()
        {
            TitleText = "Добавление нового производственного процесса";
            ButtonApplyText = "Добавить";
            IsEdit = false;
            Name = string.Empty;
            Description = string.Empty;
            Cost = 0;
            SelectedEnterprise = null;
            ClearErrors();
        }

    }

}
