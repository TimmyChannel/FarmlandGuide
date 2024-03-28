using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class LogoutMessage : ValueChangedMessage<bool>
    {
        public LogoutMessage(bool value) : base(value)
        {
        }
    }
}
