using CommunityToolkit.Mvvm.Messaging.Messages;
using Enterprise = FarmlandGuide.Models.Entities.Enterprise;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class EnterpriseTableUpdateMessage : ValueChangedMessage<Enterprise>
    {
        public EnterpriseTableUpdateMessage(Enterprise enterprise) : base(enterprise)
        {
        }
    }
}
