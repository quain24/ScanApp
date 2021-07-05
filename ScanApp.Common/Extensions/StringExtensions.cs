namespace ScanApp.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Shortens given <paramref name="source"/> to a more display-friendly version of given <paramref name="length"/>.<br/>
        /// If shortened string have last whitespace after 2/3 of given max <paramref name="length"/>, it will be further truncated to this space location.
        /// </summary>
        /// <param name="source">Source <see cref="string"/>.</param>
        /// <param name="length">Maximum length allowed before truncation.</param>
        /// <returns>
        /// <para>
        /// <see cref="string"/> of given <paramref name="length"/> with additional '...' at the end if there is no whitespace in last third of truncated string.
        /// </para>
        /// <para>
        /// <see cref="string"/> of length of last whitespace location with additional '...' at the end if there was whitespace in last third of truncated string.
        /// </para>
        /// </returns>
        public static string Truncate(this string source, int length = 15)
        {
            if (source is null) return null;
            if (source.Length <= length) return source;
            if (string.IsNullOrWhiteSpace(source)) return source[..length] + "...";

            var tmp = source[..length];
            if (tmp.LastIndexOf(' ') > (length / 3) * 2)
            {
                return tmp[..tmp.LastIndexOf(' ')] + "...";
            }

            return source[..length] + "...";
        }
    }
}