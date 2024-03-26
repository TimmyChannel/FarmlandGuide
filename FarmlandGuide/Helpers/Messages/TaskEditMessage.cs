using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = FarmlandGuide.Models.Task;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class TaskEditMessage : ValueChangedMessage<Task>
    {
        public TaskEditMessage(Task task) : base(task)
        {
        }
    }
}
