
using RymPlaylist.Service;

namespace Test
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var service = new SpotifyService();

            Thread.Sleep(2000);
            Console.Clear();

            var testAlbums = new List<string>()
                {
                    "Tim Hecker Harmony In Ultraviolet"
                };

            string uri = await service.CreatePlaylist("test", testAlbums);
            Console.WriteLine(uri);
        }
    }

}
