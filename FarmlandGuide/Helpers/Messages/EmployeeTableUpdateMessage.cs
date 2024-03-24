﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class EmployeeTableUpdateMessage : ValueChangedMessage<Employee>
    {
        public EmployeeTableUpdateMessage(Employee employee) : base(employee)
        {
        }
    }
}
