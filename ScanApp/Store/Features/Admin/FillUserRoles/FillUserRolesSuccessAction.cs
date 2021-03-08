using System.Collections.Generic;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Store.Features.Admin.FillUserRoles
{
    public class FillUserRolesSuccessAction
    {
        public List<string> UserRoles { get; }
        public Version Version { get; }

        public FillUserRolesSuccessAction(List<string> userRoles, Version version)
        {
            UserRoles = userRoles;
            Version = version;
        }
    }
}