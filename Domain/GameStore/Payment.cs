using System;
using System.Collections.Generic;


namespace GameStore
{
    public class Payment
    {
        public string NamePayment { get; }
        public string Description { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }
        public Payment(string namePayment,
                            string description,
                            IReadOnlyDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(namePayment))
                throw new ArgumentException(nameof(namePayment));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(description));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            NamePayment = namePayment;
            Description = description;
            Parameters = parameters;
        }
    }
}
