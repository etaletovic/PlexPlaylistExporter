using System.Xml.Serialization;

namespace PlexPlaylistExported.PlexClient.Contracts.Models
{
    public class MediaContainer
    {
        [XmlElement("Server")]
        public List<Server> Servers { get; set; }

        [XmlElement("Device")]
        public List<Device> Devices { get; set; }

        [XmlAttribute("machineIdentifier")]
        public string MachineIdentifier { get; set; }

        [XmlAttribute("identifier")]
        public string Identifier { get; set; }

        [XmlAttribute("friendlyName")]
        public string FriendlyName { get; set; }
    }
}
