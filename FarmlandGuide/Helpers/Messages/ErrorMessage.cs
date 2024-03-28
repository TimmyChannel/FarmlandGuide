using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class ErrorMessage : ValueChangedMessage<string>
    {
        public ErrorMessage(string info) : base(info)
        {
        }
    }
}
