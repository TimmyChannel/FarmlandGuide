using CommunityToolkit.Mvvm.Messaging.Messages;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers
{
    public sealed class SelectedEmployeeMessage : ValueChangedMessage<Employee>
    {
        public SelectedEmployeeMessage(Employee employee) : base(employee)
        {
        }
    }
}
