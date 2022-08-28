namespace PlexPlaylistExporter.PlexClient.Contracts.Models
{
    public class Location
    {
        public string code { get; set; }
        public bool european_union_member { get; set; }
        public string continent_code { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string time_zone { get; set; }
        public string postal_code { get; set; }
        public bool in_privacy_restricted_country { get; set; }
        public string subdivisions { get; set; }
        public string coordinates { get; set; }
    }
}
