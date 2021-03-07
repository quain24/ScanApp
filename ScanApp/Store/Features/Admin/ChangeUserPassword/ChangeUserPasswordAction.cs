namespace ScanApp.Store.Features.Admin.ChangeUserPassword
{
    public class ChangeUserPasswordAction
    {
        public string NewPassword { get; }

        public ChangeUserPasswordAction(string newPassword)
        {
            NewPassword = newPassword;
        }
    }
}