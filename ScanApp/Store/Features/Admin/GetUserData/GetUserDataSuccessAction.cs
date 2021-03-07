using ScanApp.Application.Admin.Queries.GetAllUserData;

namespace ScanApp.Store.Features.Admin.GetUserData
{
    public class GetUserDataSuccessAction
    {
        public UserInfoModel User { get; }

        public GetUserDataSuccessAction(UserInfoModel user)
        {
            User = user;
        }
    }
}