using System.Collections.Generic;

namespace GameStore.Contractors
{
    public interface IDeliveryService
    {
        string Name { get; }
        string Title { get; }
        decimal DeliveryPrice { get; }
        DataSteps FirstStep(Order order);
        DataSteps NextStep(int step, IReadOnlyDictionary<string, string> values);
        Delivery GetDelivery(DataSteps data);
    }
}
