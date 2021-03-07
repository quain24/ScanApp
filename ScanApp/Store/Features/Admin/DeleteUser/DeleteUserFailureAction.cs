namespace ScanApp.Store.Features.Admin.DeleteUser
{
    public class DeleteUserFailureAction : FailureAction
    {
        public DeleteUserFailureAction(Error error) : base(error)
        {
        }
    }
}