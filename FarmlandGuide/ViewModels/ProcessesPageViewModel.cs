using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Helpers.Validators;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using NLog;
using NLog.Targets;
using Enterprise = FarmlandGuide.Models.Entities.Enterprise;
using ProductionProcess = FarmlandGuide.Models.Entities.ProductionProcess;

namespace FarmlandGuide.ViewModels
{
    public partial class ProcessesPageViewModel : ObservableValidator, IRecipient<EnterpriseTableUpdateMessage>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [ObservableProperty] private bool _isEdit;

        public ProcessesPageViewModel()
        {
            Logger.Trace("ProcessesPageViewModel creating");
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ProductionProcesses = new ObservableCollection<ProductionProcess>(ctx.ProductionProcesses.Include(pp => pp.Enterprise));
            WeakReferenceMessenger.Default.RegisterAll(this);
            Enterprises = new ObservableCollection<Enterprise>(ctx.Enterprises);
            Logger.Trace("ProcessesPageViewModel created");
        }
        public void Receive(EnterpriseTableUpdateMessage message)
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                Logger.Trace("Receiving EnterpriseTableUpdateMessage {0}", message.Value);
                Enterprises = new ObservableCollection<Enterprise>(ctx.Enterprises.ToList());
                ProductionProcesses = new ObservableCollection<ProductionProcess>(ctx.ProductionProcesses.Include(pp => pp.Enterprise));

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        [ObservableProperty] private string _titleText;
        [ObservableProperty] private string _buttonApplyText;
        private ObservableCollection<Enterprise> _enterprises;
        private ProductionProcess? _selectedProductionProcess;
        private Enterprise? _selectedEnterprise;
        private ObservableCollection<ProductionProcess> _productionProcesses;
        private string _name;
        private string _description;
        private decimal _cost;

        public ObservableCollection<Enterprise> Enterprises
        {
            get => _enterprises;
            set => SetProperty(ref _enterprises, value, true);
        }
        public ProductionProcess? SelectedProductionProcess
        {
            get => _selectedProductionProcess;
            set => SetProperty(ref _selectedProductionProcess, value, true);
        }

        [ShouldBeSelected("Необходимо выбрать предприятие")]
        public Enterprise? SelectedEnterprise
        {
            get => _selectedEnterprise;
            set => SetProperty(ref _selectedEnterprise, value, true);
        }

        public ObservableCollection<ProductionProcess> ProductionProcesses
        {
            get => _productionProcesses;
            set => SetProperty(ref _productionProcesses, value, true);
        }

        [NotEmpty]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, true);
        }

        [NotEmpty]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value, true);
        }
        [LessOrEqualThenValidation(0)]
        public decimal Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value, true);
        }

        [RelayCommand]
        private void OnApplyChangesAtProductionProcess()
        {
            Logger.Debug("Initiated change application attempt");
            ValidateAllProperties();
            if (HasErrors)
            {
                Logger.Warn("Failed change application attempt. Data errors.");
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
                Logger.Trace("Closing dialog and clear all props");
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
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnAddProductionProcess()
        {
            try
            {
                Logger.Info("Addition new process");
                using var ctx = new ApplicationDbContext();
                var process = new ProductionProcess(Name.Trim(), Description.Trim(), Cost, SelectedEnterprise?.EnterpriseId ?? 1)
                {
                    Enterprise = ctx.Enterprises.First(e => e.EnterpriseId == SelectedEnterprise.EnterpriseId)
                };
                ctx.ProductionProcesses.Add(process);
                ctx.SaveChanges();
                ProductionProcesses.Add(process);
                Logger.Info("Added new process: {0}", process.ToString());
                WeakReferenceMessenger.Default.Send(new ProductionProcessTableUpdate(process));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        private void OnEditProductionProcess()
        {
            try
            {
                Logger.Info("Editing process");
                using var ctx = new ApplicationDbContext();
                SelectedProductionProcess.Name = Name.Trim();
                SelectedProductionProcess.Description = Description.Trim();
                SelectedProductionProcess.Cost = Cost;
                SelectedProductionProcess.Enterprise = ctx.Enterprises.First(e => e.EnterpriseId == SelectedEnterprise.EnterpriseId);
                SelectedProductionProcess.EnterpriseId = SelectedEnterprise?.EnterpriseId ?? 1;

                ctx.ProductionProcesses.Update(SelectedProductionProcess);
                ctx.SaveChanges();
                Logger.Info("Edited process: {0}", SelectedProductionProcess.ToString());
                WeakReferenceMessenger.Default.Send(new ProductionProcessTableUpdate(SelectedProductionProcess));

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        [RelayCommand]
        private void OnDeleteProductionProcess()
        {
            try
            {
                Logger.Info("Deleting process");
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedProductionProcess is null)
                {
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                ctx.ProductionProcesses.Remove(SelectedProductionProcess);
                ctx.SaveChanges();
                Logger.Info("Deleted process: {0}", SelectedProductionProcess.ToString());
                WeakReferenceMessenger.Default.Send(new ProductionProcessTableUpdate(SelectedProductionProcess));
                ProductionProcesses.Remove(SelectedProductionProcess);
                closeDialogCommand.Execute(null, null);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
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
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
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
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

    }

}
