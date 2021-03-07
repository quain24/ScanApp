using Fluxor;

namespace ScanApp.Store.Features.Admin.ModifyUser
{
    public static class ModifyUserDataReducers
    {
        [ReducerMethod]
        public static AdminState ReduceModifyUserDataAction(AdminState state, ModifyUserDataAction _) =>
            state with { IsLoading = true };

        [ReducerMethod]
        public static AdminState ReduceModifyUserDataSuccessAction(AdminState state, ModifyUserDataSuccessAction action) =>
            state with
            {
                IsLoading = false,
                CurrentError = null,
                SelectedUserName = action.Data.NewName,
                SelectedUserVersion = action.Data.Version,
                UserNames = state.UserNames.Replace(action.Data.Name, action.Data.NewName).Sort()
            };

        [ReducerMethod]
        public static AdminState ReduceModifyUserDataFailureAction(AdminState state, ModifyUserDataFailureAction action) =>
            state with { IsLoading = false, CurrentError = action.Error };
    }
}