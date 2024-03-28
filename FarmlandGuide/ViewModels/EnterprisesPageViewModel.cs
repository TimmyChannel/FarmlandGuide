using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Helpers.Validators;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using NLog;
using NLog.Targets;
using Enterprise = FarmlandGuide.Models.Entities.Enterprise;

namespace FarmlandGuide.ViewModels;

public partial class EnterprisesPageViewModel : ObservableValidator
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public EnterprisesPageViewModel()
    {
        Logger.Trace("EnterprisesPageViewModel creating");
        using var ctx = new ApplicationDbContext();
        ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        Enterprises = new ObservableCollection<Enterprise>(ctx.Enterprises);
        Logger.Trace("EnterprisesPageViewModel created");
    }

    [ObservableProperty] private bool _isEdit;

    [ObservableProperty] private string _titleText;

    [ObservableProperty] private string _buttonApplyText;

    [ObservableProperty] private Enterprise? _selectedEnterprise;

    [ObservableProperty] private ObservableCollection<Enterprise> _enterprises;

    private string _name;
    private string _address;

    [NotEmpty]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value, true);
    }

    [NotEmpty]
    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value, true);
    }

    [RelayCommand]
    private void OnApplyChangesAtEnterprises()
    {
        Logger.Debug("Initiated change application attempt");
        ValidateAllProperties();
        if (HasErrors)
        {
            Logger.Warn("Failed change application attempt. Data errors.");
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
            Logger.Trace("Closing dialog and clear all props");
            IsEdit = false;
            Name = string.Empty;
            Address = string.Empty;
            ClearErrors();
            var closeDialogCommand = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            closeDialogCommand.Execute(null, null);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Something went wrong");
            WeakReferenceMessenger.Default.Send(
                new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
        }
    }

    private void OnAddEnterprise()
    {
        try
        {
            Logger.Info("Addition new enterprise");
            using var ctx = new ApplicationDbContext();
            var enterprise = new Enterprise(Name.Trim(), Address.Trim());
            ctx.Enterprises.Add(enterprise);
            ctx.SaveChanges();
            Enterprises.Add(enterprise);
            Logger.Info("Added new enterprise: {0}", enterprise.ToString());
            WeakReferenceMessenger.Default.Send(new EnterpriseTableUpdateMessage(enterprise));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Something went wrong");
            WeakReferenceMessenger.Default.Send(
                new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
        }
    }

    private void OnEditEnterprise()
    {
        try
        {
            Logger.Info("Editing enterprise");
            using var ctx = new ApplicationDbContext();
            SelectedEnterprise.Name = Name.Trim();
            SelectedEnterprise.Address = Address.Trim();
            ctx.Enterprises.Update(SelectedEnterprise);
            ctx.SaveChanges();
            Logger.Info("Edited enterprise: {0}", SelectedEnterprise.ToString());
            WeakReferenceMessenger.Default.Send(new EnterpriseTableUpdateMessage(SelectedEnterprise));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Something went wrong");
            WeakReferenceMessenger.Default.Send(
                new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
        }
    }

    [RelayCommand]
    private void OnDeleteEnterprise()
    {
        try
        {
            Logger.Info("Deleting enterprise");
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
            Logger.Info("Deleted enterpise: {0}", SelectedEnterprise.ToString());
            Enterprises.Remove(SelectedEnterprise);
            closeDialogCommand.Execute(null, null);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Something went wrong");
            WeakReferenceMessenger.Default.Send(
                new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
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
            Logger.Error(ex, "Something went wrong");
            WeakReferenceMessenger.Default.Send(
                new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
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
            Logger.Error(ex, "Something went wrong");
            WeakReferenceMessenger.Default.Send(
                new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
        }
    }
}