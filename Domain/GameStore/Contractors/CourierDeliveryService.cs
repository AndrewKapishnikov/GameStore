using System;
using System.Collections.Generic;

namespace GameStore.Contractors
{
    public class CourierDeliveryService : IDeliveryService
    {
        public string Name => "Courier";

        public string Title => "Доставка товаров курьером";

        public decimal DeliveryPrice => 300m;

        public DataSteps FirstStep(Order order)
        {
            return DataSteps.CreateFirst(Name)
                       .AddParameter("orderId", order.Id.ToString());
        }

        public Delivery GetDelivery(DataSteps data)
        {
            if (data.ServiceName != Name) 
                throw new InvalidOperationException("Invalid payment form.");

            return new Delivery(Name, Title, DeliveryPrice, data.Parameters);
        }

        public DataSteps NextStep(int step, IReadOnlyDictionary<string, string> values)
        {
            if (step != 1)
                throw new InvalidOperationException("Invalid cash payment step.");

            return DataSteps.CreateLast(Name, step + 1, values);
        }
    }
}
