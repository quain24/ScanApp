using ScanApp.Application.Admin.Commands.EditUserData;

namespace ScanApp.Store.Features.Admin.ModifyUser
{
    public class ModifyUserDataSuccessAction
    {
        public EditUserDto Data { get; }

        public ModifyUserDataSuccessAction(EditUserDto data)
        {
            Data = data;
        }
    }
}