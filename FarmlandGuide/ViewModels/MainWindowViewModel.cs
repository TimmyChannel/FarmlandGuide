using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, IRecipient<LoggedUserMessage>
    {
        [ObservableProperty]
        string _employeeFIO;
        [ObservableProperty]
        string _role;
        [ObservableProperty]
        bool _isAdministrator;
        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(LoggedUserMessage message)
        {
            var employee = message.Value;
            EmployeeFIO = employee.ToString();
            Role = employee.Role.Name;
            IsAdministrator = employee.Role.RoleID == 1;
        }
    }
}
