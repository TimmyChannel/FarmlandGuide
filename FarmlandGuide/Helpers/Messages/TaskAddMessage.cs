using CommunityToolkit.Mvvm.Messaging.Messages;
using Task = FarmlandGuide.Models.Entities.Task;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class TaskAddMessage : ValueChangedMessage<Task>
    {
        public TaskAddMessage(Task task) : base(task)
        {
        }
    }
}
