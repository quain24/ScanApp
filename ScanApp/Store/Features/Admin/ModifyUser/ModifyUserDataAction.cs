using ScanApp.Application.Admin.Commands.EditUserData;

namespace ScanApp.Store.Features.Admin.ModifyUser
{
    public class ModifyUserDataAction
    {
        public EditUserDto UserData { get; }

        public ModifyUserDataAction(EditUserDto userData)
        {
            UserData = userData;
        }
    }
}