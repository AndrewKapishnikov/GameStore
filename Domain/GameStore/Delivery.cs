using System;
using System.Collections.Generic;

namespace GameStore
{
    public class Delivery
    {
        public string NameDelivery { get; }

        public string Description { get; }

        public decimal DeliveryPrice { get; }

        public IReadOnlyDictionary<string, string> Parameters { get; }

        public Delivery(string nameDelivery,
                             string description,
                             decimal price,
                             IReadOnlyDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(nameDelivery))
                throw new ArgumentException(nameof(nameDelivery));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(description));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            NameDelivery = nameDelivery;
            Description = description;
            DeliveryPrice = price;
            Parameters = parameters;
        }
    }
}
