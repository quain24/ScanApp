using ScanApp.Domain.ValueObjects;
using System.Collections.Immutable;

namespace ScanApp.Store.Features.Admin
{
    public record AdminState : RootState
    {
        public AdminState(bool isLoading = false, Error currentError = null) : base(isLoading, currentError)
        {
        }

        public string SelectedUserName { get; init; } = string.Empty;
        public Version SelectedUserVersion { get; init; } = Version.Empty();
        public ImmutableList<string> UserNames { get; init; } = ImmutableList<string>.Empty;

        public string SelectedRoleName { get; init; } = string.Empty;
        public Version SelectedRoleVersion { get; init; } = Version.Empty();
        public ImmutableList<string> RoleNames { get; init; } = ImmutableList<string>.Empty;
    }
}