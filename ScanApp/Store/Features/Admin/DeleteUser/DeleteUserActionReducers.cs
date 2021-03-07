using Fluxor;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Store.Features.Admin.DeleteUser
{
    public static class DeleteUserActionReducers
    {
        [ReducerMethod]
        public static AdminState ReduceDeleteUserAction(AdminState state, DeleteUserAction _) =>
            state with { IsLoading = true };

        [ReducerMethod]
        public static AdminState ReduceDeleteUserSuccessAction(AdminState state, DeleteUserSuccessAction _)
        {
            var users = state.UserNames.Remove(state.SelectedUserName).Sort();
            return state with
            {
                UserNames = users,
                IsLoading = false,
                CurrentError = null,
                SelectedUserName = string.Empty,
                SelectedUserVersion = Version.Empty()
            };
        }

        public static AdminState ReduceDeleteUserFailureAction(AdminState state, DeleteUserFailureAction action)
        {
            return state with
            {
                IsLoading = false,
                CurrentError = action.Error
            };
        }
    }
}