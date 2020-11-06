namespace SpotifyMatch.Extensions
{
    public static class StringExtensions
    {
        public static string CleanString(this string str)
        {
            return str.Trim().Replace("\0", "");
        }
    }
}