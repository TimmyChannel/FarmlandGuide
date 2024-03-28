using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, IRecipient<LoggedUserMessage>, IRecipient<WaitProcessMessage>, IRecipient<ErrorMessage>
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [ObservableProperty] private string _employeeFIO;
        [ObservableProperty] private string _role;
        [ObservableProperty] private bool _isAdministrator;
        [ObservableProperty] private bool _isWait = false;
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
                _logger.Trace("Receiving LoggedUserMessage {0}", message.Value);
                var employee = message.Value;
                EmployeeFIO = employee.ToString();
                Role = employee.Role.Name;
                IsAdministrator = employee.Role.RoleID == 1;

            }
            catch (Exception ex)
{
    _logger.Error(ex, "Something went wrong");
    WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне последний файл из папки /Logs/ \n Текст ошибки: {ex.Message}"));
}
        }

        public void Receive(WaitProcessMessage message)
        {
            _logger.Trace("Receiving WaitProcessMessage {0}", message.Value);
            IsWait = message.Value;
        }

        public void Receive(ErrorMessage message)
        {
            ErrorMessage = message.Value;
            CallError = true;
        }
    }
}
