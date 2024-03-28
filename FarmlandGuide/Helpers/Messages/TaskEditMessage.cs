using CommunityToolkit.Mvvm.Messaging.Messages;
using Task = FarmlandGuide.Models.Entities.Task;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class TaskEditMessage : ValueChangedMessage<Task>
    {
        public TaskEditMessage(Task task) : base(task)
        {
        }
    }
}
