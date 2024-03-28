using CommunityToolkit.Mvvm.Messaging.Messages;
using WorkSession = FarmlandGuide.Models.Entities.WorkSession;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class WorkSessionEditMessage : ValueChangedMessage<WorkSession>
    {
        public WorkSessionEditMessage(WorkSession workSession) : base(workSession)
        {
        }
    }
}
