using Fluxor;

namespace ScanApp.Store.Features.Admin.FillUserRoles
{
    public static class FillUserRolesReducers
    {
        [ReducerMethod]
        public static AdminState ReduceFillUserRolesAction(AdminState state, FillUserRolesAction _) =>
            state with { IsLoading = false };

        [ReducerMethod]
        public static AdminState ReduceFillUserRolesSuccessAction(AdminState state, FillUserRolesSuccessAction action) =>
            state with
            {
                IsLoading = false,
                CurrentError = null,
                SelectedUserVersion = action.Version
            };

        [ReducerMethod]
        public static AdminState ReduceFillUserRolesFailureAction(AdminState state, FillUserRolesFailureAction action) =>
            state with { IsLoading = false, CurrentError = action.Error };
    }
}