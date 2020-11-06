using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System.Collections.Generic;

namespace SpotifyMatch.Data
{
    public class TrackResponse
    {
        public string Filename { get; internal set; }
        public List<FullTrack> FoundTracks { get; internal set; }
    }
}