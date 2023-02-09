using Infastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XMLViewLibrary
{
    internal interface IDataProvider : IDisposable
    {
        event Action<IEnumerable<Data>> DataUpdated;
        event Action<object> Failed;
    }
}
