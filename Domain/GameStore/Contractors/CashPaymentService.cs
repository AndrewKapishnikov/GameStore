using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Contractors
{
    public class CashPaymentService : IPaymentService
    {
        public string Name => "Cash";

        public string Title => "Оплата наличными при получении товара";

        public DataSteps FirstStep(Order order)
        {
            return DataSteps.CreateFirst(Name)
                       .AddParameter("orderId", order.Id.ToString());
        }

        public DataSteps NextStep(int step, IReadOnlyDictionary<string, string> values)
        {
            if (step != 1)
                throw new InvalidOperationException("Invalid cash payment step.");

            return DataSteps.CreateLast(Name, step + 1, values);
        }

        public Payment GetPayment(DataSteps data)
        {
            if (data.ServiceName != Name) //remove check for optimization
            //if (data.ServiceName != Name || !data.IsFinal)
                    throw new InvalidOperationException("Invalid payment form.");

            return new Payment(Name, Title, data.Parameters);
        }
    }
}
