namespace ScanApp.Store.Features.Admin.DeleteUser
{
    public class DeleteUserSuccessAction
    {
        public string DeletedUserName { get; }

        public DeleteUserSuccessAction(string deletedUserName)
        {
            DeletedUserName = deletedUserName;
        }
    }
}