using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParcelHandling.Shared
{
    [XmlRoot(ElementName = "Parcel")]
    public class Parcel : IDispatchable
    {
        [XmlElement(ElementName = "Recipient")]
        public Recipient? Recipient { get; set; }

        [XmlElement(ElementName = "Weight")]
        public float Weight { get; set; }

        [XmlElement(ElementName = "Value")]
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
