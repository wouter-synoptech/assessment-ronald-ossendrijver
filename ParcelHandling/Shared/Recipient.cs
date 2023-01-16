using System.Xml.Serialization;

namespace ParcelHandling.Shared
{
    public class Recipient
    {
        [XmlElement("Name")]
        public string? Name { get; set; }

        [XmlElement("Address")]
        public Address? Address { get; set; }

        public override string ToString()
        {
            return $"{Name} : {Address}";
        }
    }
}
