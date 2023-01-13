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
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "ShippingDate")]
        public DateTime ShippingDate { get; set; }

        [XmlElement(ElementName = "parcels")]
        public List<Parcel> Parcels { get; set; }
    }
}
