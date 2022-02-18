namespace RymPlaylist.Service.Models
{
    public class ChartsSearchParameters
    {
        public string? Genre { get; set; }
        public int? Year { get; set; }
        public string? Descriptor { get; set; }
        public int Count { get; set; }
    }
}
