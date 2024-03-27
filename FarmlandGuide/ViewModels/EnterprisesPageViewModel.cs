using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
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
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public EnterpisesPageViewModel()
        {
            _logger.Trace("EnterpisesPageViewModel creating");
            using var ctx = new ApplicationDbContext();
            Enterprises = new(ctx.Enterprises);
            _logger.Trace("EnterpisesPageViewModel created");
        }

        [ObservableProperty]
        bool _isEdit = false;

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
            _logger.Debug("Initiated change application attempt");
            ValidateAllProperties();
            if (HasErrors)
            {
                _logger.Warn("Failed change application attempt. Data errors.");
                return;
            }

            if (IsEdit)
                OnEditEnterprise();
            else
                OnAddEnterprise();
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
                Address = string.Empty;
                ClearErrors();
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                closeDialogCommand.Execute(null, null);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
        private void OnAddEnterprise()
        {
            try
            {
                _logger.Info("Addition new enterprise");
                using var ctx = new ApplicationDbContext();
                var enterpise = new Enterprise(Name.Trim(), Address.Trim());
                ctx.Enterprises.Add(enterpise);
                ctx.SaveChanges();
                Enterprises.Add(enterpise);
                _logger.Info("Added new enterprise: {0}", enterpise.ToString());
                WeakReferenceMessenger.Default.Send(new EnterpriseTableUpdateMessage(enterpise));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        private void OnEditEnterprise()
        {
            try
            {
                _logger.Info("Editing enterprise");
                using var ctx = new ApplicationDbContext();
                SelectedEnterprise.Name = Name.Trim();
                SelectedEnterprise.Address = Address.Trim();
                ctx.Enterprises.Update(SelectedEnterprise);
                ctx.SaveChanges();
                _logger.Info("Edited enterprise: {0}", SelectedEnterprise.ToString());
                WeakReferenceMessenger.Default.Send(new EnterpriseTableUpdateMessage(SelectedEnterprise));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }

        [RelayCommand]
        private void OnDeleteEnterprise()
        {
            try
            {
                _logger.Info("Deleting enterprise");
                var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                if (SelectedEnterprise is null)
                {
                    closeDialogCommand.Execute(null, null);
                    return;
                }
                using var ctx = new ApplicationDbContext();
                ctx.Enterprises.Remove(SelectedEnterprise);
                ctx.SaveChanges();
                WeakReferenceMessenger.Default.Send(new EnterpriseTableUpdateMessage(SelectedEnterprise));
                _logger.Info("Deleted enterpise: {0}", SelectedEnterprise.ToString());
                Enterprises.Remove(SelectedEnterprise);
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
                if (SelectedEnterprise is null)
                    return;
                TitleText = "Редактирование производственного процесса";
                ButtonApplyText = "Сохранить";
                Name = SelectedEnterprise.Name;
                Address = SelectedEnterprise.Address;
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
                Address = string.Empty;
                ClearErrors();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
    }
}
