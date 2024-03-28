using CommunityToolkit.Mvvm.Messaging.Messages;
using Employee = FarmlandGuide.Models.Entities.Employee;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class EmployeeTableUpdateMessage : ValueChangedMessage<Employee>
    {
        public EmployeeTableUpdateMessage(Employee employee) : base(employee)
        {
        }
    }
}
