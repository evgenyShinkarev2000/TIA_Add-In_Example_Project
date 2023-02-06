using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLViewLibrary
{
    internal class ViewNameAttribute : Attribute
    {
        public readonly string Name;
        public ViewNameAttribute(string name)
        {
            Name = name;
        }
    }
}
