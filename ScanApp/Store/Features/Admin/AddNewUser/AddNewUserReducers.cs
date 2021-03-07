using Fluxor;

namespace ScanApp.Store.Features.Admin.AddNewUser
{
    public static class AddNewUserReducers
    {
        [ReducerMethod]
        public static AdminState ReduceAddNewUserAction(AdminState state, AddNewUserAction _)
        {
            return state with { IsLoading = true };
        }

        [ReducerMethod]
        public static AdminState ReduceAddNewUserSuccessAction(AdminState state, AddNewUserSuccessAction action)
        {
            return state with
            {
                IsLoading = false,
                UserNames = state.UserNames.Add(action.NewUserName).Sort(),
                CurrentError = null
            };
        }

        [ReducerMethod]
        public static AdminState ReduceAddNewUserFailureAction(AdminState state, AddNewUserFailureAction action)
        {
            return state with
            {
                IsLoading = false,
                CurrentError = action.Error
            };
        }
    }
}