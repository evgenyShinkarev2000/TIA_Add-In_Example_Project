using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLViewLibrary
{
    internal class ComboBoxItemVM<T> : Entity<T>
    {
        public string Content { get; }
        public ComboBoxItemVM(T id, string content) : base(id)
        {
            Content = content;
        }
    }
}
