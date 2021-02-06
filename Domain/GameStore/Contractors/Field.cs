using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Contractors
{
    public abstract class Field
    {
        public string LabelName { get; }

        public string Name { get; }
    
        protected Field(string label, string name)
        {
            LabelName = label;
            Name = name;
         }
    }

    public class ChoiceField : Field
    {
        public IReadOnlyDictionary<string, string> Items { get; }

        public ChoiceField(string label, string name, IReadOnlyDictionary<string, string> items)
                              : base(label, name)
        {
            Items = items;
        }
    }
}
