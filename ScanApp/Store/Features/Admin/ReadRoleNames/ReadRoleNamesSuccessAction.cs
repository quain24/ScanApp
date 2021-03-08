using System.Collections.Generic;

namespace ScanApp.Store.Features.Admin.ReadRoleNames
{
    public class ReadRoleNamesSuccessAction
    {
        public List<string> RoleNames { get; }

        public ReadRoleNamesSuccessAction(List<string> roleNames)
        {
            RoleNames = roleNames;
        }
    }
}