using CommunityToolkit.Mvvm.Messaging.Messages;
using WorkSession = FarmlandGuide.Models.Entities.WorkSession;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class WorkSessionAddMessage : ValueChangedMessage<WorkSession>
    {
        public WorkSessionAddMessage(WorkSession workSession) : base(workSession)
        {
        }
    }
}
