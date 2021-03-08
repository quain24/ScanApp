namespace ScanApp.Store.Features.Admin.FillUserRoles
{
    public class FillUserRolesAction
    {
        public string UserName { get; }

        public FillUserRolesAction(string userName)
        {
            UserName = userName;
        }
    }
}