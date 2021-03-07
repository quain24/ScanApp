namespace ScanApp.Store.Features
{
    public record Error
    {
        public string ErrorCode { get; init; } = string.Empty;
        public string ErrorText { get; init; } = string.Empty;
    }
}