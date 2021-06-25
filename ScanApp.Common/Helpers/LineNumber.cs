using System.Runtime.CompilerServices;

namespace ScanApp.Common.Helpers
{
    public static class LineNumber
    {
        private static int GetLine([CallerLineNumber] int line = 0) => line;

        /// <summary>
        /// Gets line number on which this method was called.
        /// </summary>
        /// <returns>Line number.</returns>
        public static int Get => GetLine();
    }
}