using CommandLineParser.Arguments;

namespace ScanApp.Common
{
    /// <summary>
    /// Provides place for <see cref="CommandLineParser"/> configuration
    /// </summary>
    public class CmdLineParserArguments
    {
        /// <summary>
        /// Force try re-seed initial ScanApp database
        /// </summary>
        [SwitchArgument('f', "force", false, Description = "Force try re-seed initial ScanApp database")]
        public bool force;
    }
}