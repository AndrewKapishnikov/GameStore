using System.Collections.Generic;

namespace GameStore.Contractors
{
    public interface IPaymentService
    {
        string Name { get; }
        string Title { get; }
        DataSteps FirstStep(Order order);
        DataSteps NextStep(int step, IReadOnlyDictionary<string, string> values);
        Payment GetPayment(DataSteps data);
    }
}
