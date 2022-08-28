using System.Xml.Serialization;

namespace PlexPlaylistExporter.PlexClient.Contracts.Models
{
    public class Server
    {
        [XmlAttribute("accessToken")]
        public string AccessToken { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("address")]
        public string Address { get; set; }

        [XmlAttribute("port")]
        public int Port { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("scheme")]
        public string Scheme { get; set; }

        [XmlAttribute("host")]
        public string Host { get; set; }

        [XmlAttribute("localAddresses")]
        public string LocalAddresses { get; set; }

        [XmlAttribute("machineIdentifier")]
        public string MachineIdentifier { get; set; }

        [XmlAttribute("createdAt")]
        public long CreatedAt { get; set; }

        [XmlAttribute("updatedAt")]
        public long UpdatedAt { get; set; }

        [XmlAttribute("owned")]
        public int Owned { get; set; }

        [XmlAttribute("synced")]
        public int Synced { get; set; }
    }
}
