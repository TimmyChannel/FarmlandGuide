using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class LogoutMessage : ValueChangedMessage<bool>
    {
        public LogoutMessage(bool value) : base(value)
        {
        }
    }
}
