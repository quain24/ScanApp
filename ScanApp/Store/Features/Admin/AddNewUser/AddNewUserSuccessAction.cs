namespace ScanApp.Store.Features.Admin.AddNewUser
{
    public class AddNewUserSuccessAction
    {
        public string NewUserName { get; }

        public AddNewUserSuccessAction(string newUserName)
        {
            NewUserName = newUserName;
        }
    }
}