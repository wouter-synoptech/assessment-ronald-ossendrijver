using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParcelHandling.Shared
{
    [XmlRoot(ElementName = "Container")]
    public class Container
    {
        [XmlElement("Id")]
        public string? Id { get; set; }

        [XmlElement("ShippingDate")]
        public DateTime ShippingDate { get; set; }

        [XmlArray("parcels")]
        public Parcel[]? Parcels { get; set; }
    }
}
