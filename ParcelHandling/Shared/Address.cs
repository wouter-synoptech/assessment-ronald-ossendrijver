using System.Xml.Serialization;

namespace ParcelHandling.Shared
{
    public class Address
    {
        [XmlElement(ElementName = "Street")]
        public string? Street { get; set; }

        [XmlElement(ElementName = "HouseNumber")]
        public string? HouseNumber { get; set; }

        [XmlElement(ElementName = "PostalCode")]
        public string? PostalCode { get; set; }

        [XmlElement(ElementName = "City")]
        public string? City { get; set; }

        public override string ToString()
        {
            return $"{Street} {HouseNumber} - {PostalCode} {City}";
        }

    }
}
