using HtmlAgilityPack;
using RymPlaylist.Service.Models;

namespace RymPlaylist.Service
{
    public class RymCrawler
    {
        public static async Task<List<string>> GetTopCharts(ChartsSearchParameters parameters)
        {
            List<string> chartResult = new();
            string uri = RymUriBuilder.BuildUri(parameters);
            Console.WriteLine(uri);

            HtmlDocument htmlDocument = await GetHtmlPage(uri);

            for (int i = 1; i <= parameters.Count; i++)
            {
                var xpath = $"//div[@id='pos{i}'][1]/div[@class='topcharts_textbox_top'][1]/div[@class='topcharts_item_title'][1]/a[@class='release'][1]/text()[1]";
                var result = htmlDocument.DocumentNode.SelectSingleNode(xpath)?.InnerText;

                xpath = $"//div[@id='pos{i}'][1]/div[@class='topcharts_textbox_top'][1]/div[@class='topcharts_item_artist_newmusicpage topcharts_item_artist'][1]/a[@class='artist'][1]/text()[1]";
                result += " " + htmlDocument.DocumentNode.SelectSingleNode(xpath)?.InnerText;

                if (!string.IsNullOrWhiteSpace(result))
                {
                    chartResult.Add(result);
                    Console.WriteLine($"{i}. {result}");
                }
            }

            return chartResult;
        }

        private static async Task<HtmlDocument> GetHtmlPage(string uri)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Chrome/65.0.3325.181");

            var content = await client.GetStringAsync(uri);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);

            return htmlDocument;
        }
    }
}
