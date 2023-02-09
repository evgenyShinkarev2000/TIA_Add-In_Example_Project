using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Entity<T>
    {
        public Entity(T id)
        {
            Id = id;
        }

        public T Id { get; }
    }
}
