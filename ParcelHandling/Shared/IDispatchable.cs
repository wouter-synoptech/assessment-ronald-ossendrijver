using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelHandling.Shared
{
    public interface IDispatchable
    {
        public IDictionary<string, object> GetCharacteristics();
    }
}
