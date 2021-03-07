namespace ScanApp.Store.Features
{
    public abstract record RootState
    {
        public RootState(bool isLoading, Error currentError) =>
            (IsLoading, CurrentError) = (isLoading, currentError);

        public bool IsLoading { get; init; }

        public Error CurrentError { get; init; }

        public bool HasCurrentErrors => CurrentError is not null;
    }
}