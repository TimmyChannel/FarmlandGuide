using CommunityToolkit.Mvvm.Messaging.Messages;
using WorkSession = FarmlandGuide.Models.Entities.WorkSession;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class WorkSessionDeleteMessage : ValueChangedMessage<WorkSession>
    {
        public WorkSessionDeleteMessage(WorkSession workSession) : base(workSession)
        {
        }
    }
}
