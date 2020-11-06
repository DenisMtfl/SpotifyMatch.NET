using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;

namespace SpotifyMatch.Library.Auth
{
    public class SpotifyAPI
    {
        private readonly string _clientId;
        private readonly string _secretId;
        private Token token;
        private readonly AuthorizationCodeAuth auth;
        private SpotifyWebAPI api;

        public SpotifyAPI() { }

        public SpotifyAPI(string clientId, string secretId, string redirectUrl = "http://localhost:4002", Boolean authed = false)
        {
            _clientId = clientId;
            _secretId = secretId;

            if (!authed)
            {

                System.Diagnostics.Debug.WriteLine("Authorizing for the first time");
                auth = new AuthorizationCodeAuth(
                    _clientId,
                    _secretId,
                    redirectUrl,
                    redirectUrl,
                    Scope.UserReadPrivate | Scope.UserReadCurrentlyPlaying | Scope.UserTopRead | Scope.Streaming | Scope.UserModifyPlaybackState | Scope.UserLibraryModify | Scope.UserReadPlaybackState | Scope.PlaylistReadPrivate | Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.UserLibraryRead
                );

                auth.AuthReceived += async (sender, payload) =>
                {
                    auth.Stop();
                    token = await auth.ExchangeCode(payload.Code);
                    api = new SpotifyWebAPI()
                    {
                        TokenType = token.TokenType,
                        AccessToken = token.AccessToken
                    };
                    SpotifyAuthToken.TokenType = api.TokenType;
                    SpotifyAuthToken.AccessToken = api.AccessToken;
                };
                auth.Start();
                auth.OpenBrowser();
                authed = true;
            }
        }
        public void Authenticate()
        {
            Token newToken = auth.RefreshToken(token.RefreshToken).Result;
            api.TokenType = newToken.TokenType;
            api.AccessToken = newToken.AccessToken;

            SpotifyAuthToken.TokenType = api.TokenType;
            SpotifyAuthToken.AccessToken = api.AccessToken;
        }
    }
}
