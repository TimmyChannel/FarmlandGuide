using CommunityToolkit.Mvvm.Messaging.Messages;
using ProductionProcess = FarmlandGuide.Models.Entities.ProductionProcess;

namespace FarmlandGuide.Helpers.Messages
{
    public sealed class ProductionProcessTableUpdate : ValueChangedMessage<ProductionProcess>
    {
        public ProductionProcessTableUpdate(ProductionProcess process) : base(process)
        {
        }
    }
}
