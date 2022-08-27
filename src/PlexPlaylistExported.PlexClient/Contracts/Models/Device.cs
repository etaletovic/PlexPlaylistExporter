using System.Xml.Serialization;

namespace PlexPlaylistExported.PlexClient.Contracts.Models
{
    public class Device
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("product")]
        public string Product { get; set; }

        [XmlAttribute("productVersion")]
        public string ProductVersion { get; set; }

        [XmlAttribute("platform")]
        public string Platform { get; set; }

        [XmlAttribute("platformVersion")]
        public string PlatformVersion { get; set; }

        [XmlAttribute("device")]
        public string DeviceString { get; set; }

        [XmlAttribute("createdAt")]
        public long CreatedAt { get; set; }

        [XmlAttribute("lastSeenAt")]
        public long LastSeenAt { get; set; }

        [XmlAttribute("owned")]
        public int Owned { get; set; }

        [XmlAttribute("synced")]
        public int Synced { get; set; }

        [XmlAttribute("clientIdentifier")]
        public string ClientIdentifier { get; set; }

        [XmlAttribute("provides")]
        public string Provides { get; set; }

        [XmlAttribute("accessToken")]
        public string AccessToken { get; set; }

        [XmlAttribute("publicAddress")]
        public string PublicAddress { get; set; }

        [XmlAttribute("httpsRequired")]
        public int HttpsRequired { get; set; }

        [XmlAttribute("relay")]
        public int Relay { get; set; }

        [XmlAttribute("dnsRebindingProtection")]
        public int DnsRebindingProtection { get; set; }

        [XmlAttribute("natLoopbackSupported")]
        public int NatLoopbackSupported { get; set; }

        [XmlAttribute("publicAddressMatches")]
        public int PublicAddressMatches { get; set; }

        [XmlAttribute("presence")]
        public int Presence { get; set; }

        [XmlElement("Connection")]
        public List<Connection> Connections { get; set; }
    }
}
