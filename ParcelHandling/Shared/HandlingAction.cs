using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelHandling.Shared
{
    public class HandlingAction
    {
        public string? Action { get; set; }

        public ParcelState Result { get; set; }
    }
}
