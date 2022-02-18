using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.Configuration;

namespace RymPlaylist.Service
{
    public class SpotifyService
    {
        private readonly string ClientID = ConfigurationManager.AppSettings["ClientID"].ToString();
        private readonly string ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();

        public static SpotifyClient SpotifyClient { get; set; }
        private static PrivateUser LoggedUser { get; set; }
        private static EmbedIOAuthServer Server { get; set; }


        public SpotifyService()
        {
            Server = new EmbedIOAuthServer(
                     new Uri("http://localhost:5000/callback"),
                     5000
                   );
            StartServer();
        }

        private async void StartServer()
        {
            await Server.Start();

            Server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;

            var request = new LoginRequest(Server.BaseUri, ClientID, LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { "user-read-email", "playlist-modify-private", "playlist-modify-public" }
            };

            Uri url = request.ToUri();
            try
            {
                BrowserUtil.Open(url);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to open URL, manually open: {0}", url);
            }

        }

        private async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
        {
            await Server.Stop();

            AuthorizationCodeTokenResponse token = await new OAuthClient().RequestToken(
              new AuthorizationCodeTokenRequest(ClientID, ClientSecret, response.Code, Server.BaseUri)
            );

            var config = SpotifyClientConfig.CreateDefault().WithToken(token.AccessToken, token.TokenType);
            SpotifyClient = new SpotifyClient(config);

            LoggedUser = await SpotifyClient.UserProfile.Current();
        }

        public async Task<string> CreatePlaylist(string playlistName, List<string> albums)
        {
            var playlistCreateRequest = new PlaylistCreateRequest(playlistName);

            // adding tracks to tracksUri from GetAlbumTracks
            var tracksUri = new List<string>();

            foreach (var album in albums)
            {
                var tracks = await GetAlbumTracks(album);

                if (tracks != null)
                {
                    tracksUri.AddRange(tracks.Items.Select(x => x.Uri));
                }
            }

            if (tracksUri.Count > 0)
            {
                var fullPlaylist = await SpotifyClient.Playlists.Create(LoggedUser.Id, playlistCreateRequest);
                AddTracksToPlaylist(tracksUri, fullPlaylist.Id);
                return fullPlaylist.Uri;
            }

            return String.Empty;
        }

        private static async Task<Paging<SimpleTrack>> GetAlbumTracks(string searchTerm)
        {
            var search = new SearchRequest(SearchRequest.Types.Album, searchTerm);
            var album = await SpotifyClient.Search.Item(search);

            if (album != null && album.Albums != null && album.Albums.Items.Count > 0)
            {
                return await SpotifyClient.Albums.GetTracks(album.Albums.Items[0].Id);
            }
            else
            {
                return null;
            }
        }

        private static async void AddTracksToPlaylist(List<string> tracksUri, string playlistId)
        {
            var playlistItemRequest = new PlaylistAddItemsRequest(tracksUri);
            await SpotifyClient.Playlists.AddItems(playlistId, playlistItemRequest);
        }
    }
}
