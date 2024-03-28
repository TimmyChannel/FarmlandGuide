using CommunityToolkit.Mvvm.Messaging.Messages;
using Employee = FarmlandGuide.Models.Entities.Employee;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class SelectedEmployeeMessage : ValueChangedMessage<Employee>
    {
        public SelectedEmployeeMessage(Employee employee) : base(employee)
        {
        }
    }
}
