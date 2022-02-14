
using RymService;
using SpotifyService;

namespace Test
{
    class Program
    {

        static async Task Main(string[] args)
        {
            bool exit;
            var service = new SpotifyService.SpotifyService();
            var crawler = new RymCrawler();

            Thread.Sleep(2000);
            Console.Clear();

            do
            {
                Console.WriteLine("Welcome to the majestic RYM TO SPOTIFY application.\n\r");

                //Inputs
                var parameters = new SearchParameters();
                Console.WriteLine("Insert Year: ");
                parameters.Year = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Insert Genre: ");
                parameters.Genre = Console.ReadLine().Replace(" ", "-");
                Console.WriteLine("How many albums do you want in your playlist? (max 40): ");
                parameters.Count = Convert.ToInt32(Console.ReadLine());

                List<string> topcharts = await crawler.GetTopCharts(parameters);
                int notFoundCount = await service.CreatePlaylist($"{parameters.Year} - {parameters.Genre}", topcharts);

                Console.WriteLine($"Total albums not found: {notFoundCount}");
                Console.WriteLine($"You've added {topcharts.Count - notFoundCount} to your playlist");

                Console.WriteLine("Press Y to continue");
                exit = Console.ReadLine().ToUpper() != "Y";
            }
            while (!exit);

        }
    }

}
