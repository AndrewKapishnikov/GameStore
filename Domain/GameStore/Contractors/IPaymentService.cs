using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
