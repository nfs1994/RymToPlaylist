namespace RymPlaylist.API.Models
{
    public class CreatePlaylistFromChartsRequestParameters
    {
        public string PlaylistName { get; set; }
        public string? Genre { get; set; }
        public int? Year { get; set; }
        public string? Descriptor { get; set; }
        public int Count { get; set; }
    }
}
