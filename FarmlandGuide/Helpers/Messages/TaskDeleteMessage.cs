using CommunityToolkit.Mvvm.Messaging.Messages;
using Task = FarmlandGuide.Models.Entities.Task;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class TaskDeleteMessage : ValueChangedMessage<Task>
    {
        public TaskDeleteMessage(Task task) : base(task)
        {
        }
    }
}
