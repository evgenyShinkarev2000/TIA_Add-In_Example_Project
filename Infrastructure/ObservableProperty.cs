using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class DependentProperty : Attribute
    {
        public readonly IReadOnlyCollection<string> ObservablePropertyNames;
        public DependentProperty(params string[] observablePropertyNames)
        {
            ObservablePropertyNames = observablePropertyNames;
        }
    }
}
