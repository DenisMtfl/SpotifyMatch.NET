using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyMatch.Library.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyMatch.Library
{
    public class Spotify
    {
        public static List<FullTrack> SearchSong(string search)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = SpotifyAuthToken.AccessToken,
                    TokenType = SpotifyAuthToken.TokenType
                };

                var searcher = _spotify.SearchItems(search, SearchType.Track, limit: 10);
                return searcher.Tracks.Items;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/SearchSong");
            }

            return null;
        }

        public static FullTrack GetTrack(string Id)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = SpotifyAuthToken.AccessToken,
                    TokenType = SpotifyAuthToken.TokenType
                };

                return _spotify.GetTrack(Id);

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/LoveSong");
            }

            return null;
        }

        public static void LoveSong(string Id)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = SpotifyAuthToken.AccessToken,
                    TokenType = SpotifyAuthToken.TokenType
                };

                PlaybackContext context = _spotify.GetPlayingTrack();
                ErrorResponse response = _spotify.SaveTrack(Id);

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/LoveSong");
            }
        }

        public static void UnLoveSong(string Id)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = SpotifyAuthToken.AccessToken,
                    TokenType = SpotifyAuthToken.TokenType
                };

                PlaybackContext context = _spotify.GetPlayingTrack();
                ErrorResponse response = _spotify.RemoveSavedTracks(new List<string> { Id });

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/LoveSong");
            }
        }
    }
}
