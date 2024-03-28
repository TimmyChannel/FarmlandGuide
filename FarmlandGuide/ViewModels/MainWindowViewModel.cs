using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors;
using NLog;
using NLog.Targets;
using EventTrigger = System.Windows.EventTrigger;

namespace FarmlandGuide.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, IRecipient<LoggedUserMessage>, IRecipient<WaitProcessMessage>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [ObservableProperty] private string _employeeFIO;
        [ObservableProperty] private string _role;
        [ObservableProperty] private bool _isAdministrator;
        [ObservableProperty] private bool _isWait;
        [ObservableProperty] private string _errorMessage;
        [ObservableProperty] private bool _callError;
        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [RelayCommand]
        private void OnResetError()
        {
            CallError = false;
        }
        public void Receive(LoggedUserMessage message)
        {
            try
            {
                Logger.Trace("Receiving LoggedUserMessage {0}", message.Value);
                var employee = message.Value;
                EmployeeFIO = employee.ToString();
                Role = employee.Role.Name;
                IsAdministrator = employee.Role.RoleId == 1;

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }

        public void Receive(WaitProcessMessage message)
        {
            Logger.Trace("Receiving WaitProcessMessage {0}", message.Value);
            IsWait = message.Value;
        }

    }
}
