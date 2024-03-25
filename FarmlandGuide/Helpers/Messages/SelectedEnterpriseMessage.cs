using CommunityToolkit.Mvvm.Messaging.Messages;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class SelectedEnterpriseMessage : ValueChangedMessage<Enterprise>
    {
        public SelectedEnterpriseMessage(Enterprise enterprise) : base(enterprise)
        {
        }
    }
}
