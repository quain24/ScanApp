using Fluxor;

namespace ScanApp.Store.Features.Admin.ChangeUserPassword
{
    public static class ChangeUserPasswordReducers
    {
        [ReducerMethod]
        public static AdminState ReduceChangePasswordAction(AdminState state, ChangeUserPasswordAction _) =>
            state with { IsLoading = true };

        [ReducerMethod]
        public static AdminState ReduceChangePasswordSuccessAction(AdminState state, ChangeUserPasswordSuccessAction action)
        {
            return state with
            {
                IsLoading = false,
                CurrentError = null,
                SelectedUserVersion = action.Stamp
            };
        }

        [ReducerMethod]
        public static AdminState ReduceChangePasswordFailureAction(AdminState state, ChangeUserPasswordFailureAction action)
        {
            return state with
            {
                IsLoading = false,
                CurrentError = action.Error
            };
        }
    }
}