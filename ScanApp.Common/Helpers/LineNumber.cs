using System.Runtime.CompilerServices;

namespace ScanApp.Common.Helpers
{
    public static class LineNumber
    {
        /// <summary>
        /// Returns line number on which this method was called.
        /// </summary>
        /// <param name="line">Compile time resolved parameter - will not work if manually set.</param>
        /// <returns>Line number.</returns>
        public static int Get([CallerLineNumber] int line = 0) => line;
    }
}