using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = FarmlandGuide.Models.Task;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class TaskDeleteMessage : ValueChangedMessage<Task>
    {
        public TaskDeleteMessage(Task task) : base(task)
        {
        }
    }
}
