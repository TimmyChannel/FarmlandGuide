using CommunityToolkit.Mvvm.Messaging.Messages;
using Employee = FarmlandGuide.Models.Entities.Employee;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class LoggedUserMessage : ValueChangedMessage<Employee>
    {
        public LoggedUserMessage(Employee employee) : base(employee)
        {
        }
    }
}
