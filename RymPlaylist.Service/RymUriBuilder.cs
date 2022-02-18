using RymPlaylist.Service.Models;

namespace RymPlaylist.Service
{
    public static class RymUriBuilder
    {
        private const string AllTime = "/all-time";
        private const string BaseUri = "https://rateyourmusic.com/charts/top/album";

        public static string BuildUri(ChartsSearchParameters parameters)
        {
            string result = BaseUri;

            if (parameters.Year.HasValue)
            {
                result += $"/{parameters.Year}";
            }
            else
            {
                result += AllTime;
            }

            if (!string.IsNullOrWhiteSpace(parameters.Genre))
            {
                result += $"/g:{parameters.Genre}";
            }

            if (!string.IsNullOrWhiteSpace(parameters.Descriptor))
            {
                result += $"/d:{parameters.Descriptor}";
            }

            result += "/";

            return result;
        }

    }
}
