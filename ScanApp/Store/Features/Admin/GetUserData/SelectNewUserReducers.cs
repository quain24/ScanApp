using Fluxor;

namespace ScanApp.Store.Features.Admin.GetUserData
{
    public static class SelectNewUserReducers
    {
        [ReducerMethod]
        public static AdminState ReduceGetUserDataAction(AdminState state, GetUserDataAction _)
        {
            return state with { IsLoading = true };
        }

        [ReducerMethod]
        public static AdminState ReduceGetUserDataSuccessAction(AdminState state, GetUserDataSuccessAction action)
        {
            return state with
            {
                SelectedUserName = action.User.Name,
                SelectedUserVersion = action.User.Version,
                IsLoading = false,
                CurrentError = null
            };
        }

        [ReducerMethod]
        public static AdminState ReduceGetUserDataFailureAction(AdminState state, GetUserDataFailureAction action)
        {
            return state with { IsLoading = false, CurrentError = action.Error };
        }
    }
}