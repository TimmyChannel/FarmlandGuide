using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class WaitProcessMessage : ValueChangedMessage<bool>
    {
        public WaitProcessMessage(bool value) : base(value)
        {
        }
    }
}
