namespace ScanApp.Store.Features.Admin.GetUserData
{
    public class GetUserDataFailureAction : FailureAction
    {
        public GetUserDataFailureAction(Error error) : base(error)
        {
        }
    }
}