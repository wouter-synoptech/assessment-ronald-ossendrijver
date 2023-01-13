using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParcelHandling.Shared
{
    public class Parcel : IDispatchable
    {
        [XmlElement("Receipient")]
        public Recipient? Receipient { get; set; }

        [XmlElement("Weight")]
        public float Weight { get; set; }

        [XmlElement("Value")]
        public float Value { get; set;}

        public bool Authorized { get; set; }

        public IDictionary<string, object> GetCharacteristics()
        {
            return new Dictionary<string, object>()
            {
                { "Weight", Weight },
                { "Value", Value },
                { "Authorized", Authorized },
            };
        }
    }
}
