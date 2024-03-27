using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
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
    public partial class ProcessesPageViewModel : ObservableValidator, IRecipient<EnterpriseTableUpdateMessage>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [ObservableProperty]
        bool _isEdit = false;

        public ProcessesPageViewModel()
        {
            _logger.Trace("ProcessesPageViewModel creating");
            using var ctx = new ApplicationDbContext();
            ProductionProcesses = new(ctx.ProductionProcesses);
            WeakReferenceMessenger.Default.RegisterAll(this);
            Enterprises = new(ctx.Enterprises);
            _logger.Trace("ProcessesPageViewModel created");
        }
        public void Receive(EnterpriseTableUpdateMessage message)
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                _logger.Trace("Receiving EnterpriseTableUpdateMessage {0}", message.Value);
                Enterprises = new(ctx.Enterprises.ToList());

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
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
            _logger.Debug("Initiated change application attempt");
            ValidateAllProperties();
            if (HasErrors)
            {
                _logger.Warn("Failed change application attempt. Data errors.");
                return;
            }

            if (IsEdit)
                OnEditProductionProcess();
            else
                OnAddProductionProcess();
            OnCloseDialogAndClearProps();
        }
        [RelayCommand]
        private void OnCloseDialogAndClearProps()
        {
            try
            {
                _logger.Trace("Closing dialog and clear all props");
                IsEdit = false;
                Name = string.Empty;
                Description = string.Empty;
                Cost = 0;
                SelectedEnterprise = null;
                ClearErrors();
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                closeDialogCommand.Execute(null, null);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
        private void OnAddProductionProcess()
        {
            try
            {
                _logger.Info("Addition new process");
                using var ctx = new ApplicationDbContext();
                var process = new ProductionProcess(Name.Trim(), Description.Trim(), Cost, SelectedEnterprise?.EnterpriseID ?? 1)
                {
                    Enterprise = ctx.Enterprises.First(e => e.EnterpriseID == SelectedEnterprise.EnterpriseID)
                };
                ctx.ProductionProcesses.Add(process);
                ctx.SaveChanges();
                ProductionProcesses.Add(process);
                _logger.Info("Added new process: {0}", process.ToString());
                WeakReferenceMessenger.Default.Send(new ProductionProcessTableUpdate(process));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
        private void OnEditProductionProcess()
        {
            try
            {
                _logger.Info("Editing process");
                using var ctx = new ApplicationDbContext();
                SelectedProductionProcess.Name = Name.Trim();
                SelectedProductionProcess.Description = Description.Trim();
                SelectedProductionProcess.Cost = Cost;
                SelectedProductionProcess.Enterprise = ctx.Enterprises.First(e => e.EnterpriseID == SelectedEnterprise.EnterpriseID);
                SelectedProductionProcess.EnterpriseID = SelectedEnterprise?.EnterpriseID ?? 1;

                ctx.ProductionProcesses.Update(SelectedProductionProcess);
                ctx.SaveChanges();
                _logger.Info("Edited process: {0}", SelectedProductionProcess.ToString());
                WeakReferenceMessenger.Default.Send(new ProductionProcessTableUpdate(SelectedProductionProcess));

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [RelayCommand]
        private void OnDeleteProductionProcess()
        {
            try
            {
                _logger.Info("Deleting process");
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedProductionProcess is null)
                {
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.ProductionProcesses.Remove(SelectedProductionProcess);
                ctx.SaveChanges();
                _logger.Info("Deleted process: {0}", SelectedProductionProcess.ToString());
                WeakReferenceMessenger.Default.Send(new ProductionProcessTableUpdate(SelectedProductionProcess));
                ProductionProcesses.Remove(SelectedProductionProcess);
                closeDialogCommand.Execute(null, null);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [RelayCommand]
        private void OnOpenEditDialog()
        {
            try
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
        [RelayCommand]
        private void OnOpenAddDialog()
        {
            try
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

    }

}
