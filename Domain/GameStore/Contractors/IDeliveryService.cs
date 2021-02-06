using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Contractors
{
    public interface IDeliveryService
    {
        string Name { get; }
        string Title { get; }
        decimal DeliveryPrice { get; }
        DataSteps FirstForm(Order order);
        DataSteps NextForm(int step, IReadOnlyDictionary<string, string> values);
        Delivery GetDelivery(DataSteps form);
    }
}
