using System.Runtime.CompilerServices;

namespace ScanApp.Common.Helpers
{
    public static class LineNumber
    {
        /// <summary>
        /// Gets line number on which this method was called.
        /// </summary>
        /// <param name="line">Should be left untouched - must be present because C# spec.</param>
        /// <returns>Line number.</returns>
        public static int Get([CallerLineNumber] int line = 0) => line;
    }
}