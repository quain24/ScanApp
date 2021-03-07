using ScanApp.Application.Admin.Commands.AddUser;

namespace ScanApp.Store.Features.Admin.AddNewUser
{
    public class AddNewUserAction
    {
        public AddUserDto UserModel { get; }

        public AddNewUserAction(AddUserDto userModel)
        {
            UserModel = userModel;
        }
    }
}