using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.Configuration;

namespace SpotifyService
{
    public class SpotifyService
    {
        private readonly string ClientID = ConfigurationManager.AppSettings["ClientID"].ToString();
        private readonly string ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();

        public static SpotifyClient SpotifyClient { get; set; }
        private static PrivateUser _loggedUser { get; set; }
        private static EmbedIOAuthServer _server { get; set; }



        public SpotifyService()
        {
            _server = new EmbedIOAuthServer(
                     new Uri("http://localhost:5000/callback"),
                     5000
                   );
            StartServer();
        }

        private async void StartServer()
        {
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;

            var request = new LoginRequest(_server.BaseUri, ClientID, LoginRequest.ResponseType.Code)
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
            await _server.Stop();

            AuthorizationCodeTokenResponse token = await new OAuthClient().RequestToken(
              new AuthorizationCodeTokenRequest(ClientID, ClientSecret, response.Code, _server.BaseUri)
            );

            var config = SpotifyClientConfig.CreateDefault().WithToken(token.AccessToken, token.TokenType);
            SpotifyClient = new SpotifyClient(config);

            _loggedUser = await SpotifyClient.UserProfile.Current();
        }

        public async Task<int> CreatePlaylist(string name, List<string> albums)
        {
            var playlistCreateRequest = new PlaylistCreateRequest(name);
            var fullPlaylist = await SpotifyClient.Playlists.Create(_loggedUser.Id, playlistCreateRequest);
            int notFoundCount = 0;

            //albums.ForEach(async x => AddTracksToPlaylist(await GetAlbumTracks(x), fullPlaylist.Id));

            foreach (var album in albums)
            {
                var tracks = await GetAlbumTracks(album);

                if (tracks != null)
                {
                    AddTracksToPlaylist(tracks, fullPlaylist.Id);
                }
                else
                {
                    Console.WriteLine($"{album} not found \n\r");
                    notFoundCount++;
                }
            }

            return notFoundCount;
        }

        private async Task<Paging<SimpleTrack>> GetAlbumTracks(string searchTerm)
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

        private async void AddTracksToPlaylist(Paging<SimpleTrack> tracks, string playlistId)
        {
            var playlistItemRequest = new PlaylistAddItemsRequest(tracks.Items.Select(x => x.Uri).ToList());
            await SpotifyClient.Playlists.AddItems(playlistId, playlistItemRequest);
        }
    }
}
