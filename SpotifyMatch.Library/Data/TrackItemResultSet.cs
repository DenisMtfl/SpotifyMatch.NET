using System.Collections.Generic;

namespace SpotifyMatch.Data
{
    public class TrackItemResultSet
    {
        public IEnumerable<FoundTracks> FoundTracks { get; internal set; }
        public string Filename { get; internal set; }
        public int Id { get; internal set; }
    }
}