using CommunityToolkit.Mvvm.Messaging.Messages;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = FarmlandGuide.Models.Task;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class TaskAddMessage : ValueChangedMessage<Task>
    {
        public TaskAddMessage(Task task) : base(task)
        {
        }
    }
}
