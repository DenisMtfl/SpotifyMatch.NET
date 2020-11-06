using Id3;
using SpotifyMatch.Data;
using SpotifyMatch.Extensions;
using SpotifyMatch.Library;
using SpotifyMatch.Library.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SpotifyMatch.Logic
{
    public class TrackFinder
    {
        private string BaseDir { get; set; }
        private string[] Files { get; set; }


        public TrackFinder(string directory)
        {
            BaseDir = directory;
            Files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
        }

        public int FileCount()
        {
            return Files.Length;
        }


        public IEnumerable<TrackItemResultSet> GetResultFromDirectory()
        {
            if (string.IsNullOrEmpty(BaseDir))
                throw new DirectoryNotFoundException();

            int id = 1;
            foreach (TrackResponse trackResponse in Find(BaseDir))
            {
                if (trackResponse.FoundTracks.Count > 0)
                {
                    yield return new TrackItemResultSet()
                    {
                        Id = id,
                        Filename = trackResponse.Filename,
                        FoundTracks = trackResponse.FoundTracks.Select(x => new FoundTracks { Name = x.Name, Id = x.Id })
                    };
                }
                else
                {
                    yield return new TrackItemResultSet() { Id = id, Filename = trackResponse.Filename, FoundTracks = null };
                }

                id++;

                Thread.Sleep(50);
            }
        }

        private IEnumerable<TrackResponse> Find(string SearchPath)
        {
            if (SearchPath is null)
            {
                throw new ArgumentNullException(nameof(SearchPath));
            }

            foreach (var file in Files)
            {
                string title = string.Empty;
                string artist = string.Empty;
                string search = string.Empty;

                using var mp3 = new Mp3(file);
                var id3Versions = mp3.AvailableTagVersions;
                if (id3Versions.Count() > 0)
                {
                    var tag = mp3.GetTag(id3Versions.Last());

                    title = (tag.Title.Value == null) ? string.Empty : tag.Title.Value.CleanString();
                    artist = tag.Artists.Value.Count() > 0 ? tag.Artists.Value.FirstOrDefault().CleanString() : string.Empty;
                }

                if (string.IsNullOrEmpty(title))
                {
                    search = Path.GetFileNameWithoutExtension(file);
                }
                else
                    search = $"{artist} - {title}";


                var searchResponse = Spotify.SearchSong(search);
                if (searchResponse != null)
                {
                    yield return new TrackResponse() { FoundTracks = searchResponse, Filename = search };
                }
            }
        }
    }
}