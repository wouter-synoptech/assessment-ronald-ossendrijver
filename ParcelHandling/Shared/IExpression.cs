using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParcelHandling.Shared
{
    public interface IExpression
    {
        public bool Evaluate(IDictionary<string, object> values);
    }
}
