using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ParcelHandling.Shared
{
    public class Parcel : IDispatchable
    {
        public string? Id { get; set; }

        [XmlElement("Receipient")]
        public Recipient? Receipient { get; set; }

        [XmlElement("Weight")]
        public float Weight { get; set; }

        [XmlElement("Value")]
        public float Value { get; set; }

        public ParcelState State { get; set; }

        [JsonIgnore]
        public bool Authorized => State == ParcelState.Authorized;

        [JsonIgnore]
        public bool Handled => State == ParcelState.Handled;

        public IDictionary<string, object> GetCharacteristics()
        {
            return new Dictionary<string, object>()
            {
                { "Weight", Weight },
                { "Value", Value },
                { "Authorized", Authorized },
                { "Handled", Handled },
                { "Status", State },
            };
        }
    }
}
