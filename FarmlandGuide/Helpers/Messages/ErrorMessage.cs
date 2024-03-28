using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class ErrorMessage : ValueChangedMessage<string>
    {
        public ErrorMessage(string info) : base(info)
        {
        }
    }
}
