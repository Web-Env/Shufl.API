namespace Shufl.API.DownloadModels.Auth
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SpotifyUsername { get; set; }

        public string SpotifyMarket { get; set; }
    }
}
