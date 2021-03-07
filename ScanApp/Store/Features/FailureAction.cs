namespace ScanApp.Store.Features
{
    public abstract class FailureAction
    {
        private const string Text = "unknown";
        public Error Error { get; }

        public FailureAction(Error error)
        {
            Error = error ?? new Error()
            {
                ErrorCode = Text,
                ErrorText = Text
            };
        }
    }
}