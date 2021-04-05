using System;
using System.Collections.Generic;

namespace GameStore
{
    //ValueObject Delivery
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

            if(nameDelivery.Length < 3 || nameDelivery.Length > 100)
                throw new ArgumentOutOfRangeException("NameDelivery out of range");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(description));

            if (price > 100000m || price < 0)
                throw new ArgumentOutOfRangeException("Price out of range");

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            NameDelivery = nameDelivery;
            Description = description;
            DeliveryPrice = price;
            Parameters = parameters;
        }
    }
}
