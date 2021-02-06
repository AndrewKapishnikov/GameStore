using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Contractors
{
     public class DataSteps
    {
        public string ServiceName { get; }

        public int Step { get; }

        private decimal servicePrice = default;
        public decimal ServicePrice => servicePrice;

        public bool IsFinal { get; }

        //This is what will be hidden in the View, these are the parameters that accumulate as you type at each step
        private readonly Dictionary<string, string> parameters;
        public IReadOnlyDictionary<string, string> Parameters => parameters;

        //This is what you need to enter in the next step.
        private readonly List<Field> fields;
        public IReadOnlyList<Field> Fields => fields;

        public static DataSteps CreateFirst(string serviceName)
        {
            return new DataSteps(serviceName, step: 1, isFinal: false, parameters: null);
        }

        public static DataSteps CreateNext(string serviceName, int step, IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return new DataSteps(serviceName, step, isFinal: false, parameters);
        }

        public static DataSteps CreateLast(string serviceName, int step, IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return new DataSteps(serviceName, step, isFinal: true, parameters);
        }

        public DataSteps AddField(Field field)
        {
            fields.Add(field);
            return this;
        }

        public DataSteps AddParameter(string name, string value)
        {
            parameters.Add(name, value);

            return this;
        }

        public DataSteps AddServicePrice(decimal price)
        {
            servicePrice = price;
            return this;
        }

        private DataSteps(string serviceName,
                          int step,
                          bool isFinal,
                          IReadOnlyDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException(nameof(serviceName));

            if (step < 1)
                throw new ArgumentOutOfRangeException(nameof(step));

            ServiceName = serviceName;
            Step = step;
            IsFinal = isFinal;

            if (parameters == null)
                this.parameters = new Dictionary<string, string>();
            else
                this.parameters = parameters.ToDictionary(p => p.Key, p => p.Value);

            fields = new List<Field>();
        }

    

     }
}
