using Fluxor;
using System.Collections.Immutable;

namespace ScanApp.Store.Features.Admin.ReadUserNames
{
    public static class UserNamesReducers
    {
        [ReducerMethod]
        public static AdminState ReduceReadUserNamesAction(AdminState state, ReadUserNamesAction _)
        {
            return state with { IsLoading = true };
        }

        [ReducerMethod]
        public static AdminState ReduceReadUserNamesSuccessAction(AdminState state, ReadUserNamesSuccessAction action)
        {
            return state with
            {
                UserNames = action.UserNames.ToImmutableList(),
                CurrentError = null,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static AdminState ReduceReadUserNamesFailureAction(AdminState state, ReadUserNamesFailureAction action)
        {
            return state with
            {
                CurrentError = action.Error,
                IsLoading = false
            };
        }
    }
}