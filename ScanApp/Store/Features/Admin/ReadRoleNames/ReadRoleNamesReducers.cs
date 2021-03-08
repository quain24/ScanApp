using Fluxor;
using System.Collections.Immutable;

namespace ScanApp.Store.Features.Admin.ReadRoleNames
{
    public static class ReadRoleNamesReducers
    {
        [ReducerMethod]
        public static AdminState ReduceReadRoleNamesAction(AdminState state, ReadRoleNamesAction _) =>
            state with { IsLoading = true };

        [ReducerMethod]
        public static AdminState ReduceReadRoleNamesSuccessAction(AdminState state, ReadRoleNamesSuccessAction action) =>
            state with
            {
                IsLoading = false,
                CurrentError = null,
                RoleNames = action.RoleNames.ToImmutableList()
            };

        [ReducerMethod]
        public static AdminState ReduceReadRoleNamesFailureAction(AdminState state, ReadRoleNamesFailureNames action) =>
            state with
            {
                IsLoading = false,
                CurrentError = action.Error
            };
    }
}