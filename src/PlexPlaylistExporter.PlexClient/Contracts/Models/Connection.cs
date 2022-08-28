using System.Xml.Serialization;

namespace PlexPlaylistExporter.PlexClient.Contracts.Models
{
    public class Connection
    {
        [XmlAttribute("protocol")]
        public string Protocol { get; set; }

        [XmlAttribute("address")]
        public string Address { get; set; }

        [XmlAttribute("port")]
        public int Port { get; set; }

        [XmlAttribute("uri")]
        public string Uri { get; set; }

        [XmlAttribute("local")]
        public int Local { get; set; }

        [XmlAttribute("relay")]
        public int Relay { get; set; }
    }
}
