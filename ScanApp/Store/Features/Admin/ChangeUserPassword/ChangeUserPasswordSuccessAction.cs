using ScanApp.Domain.ValueObjects;

namespace ScanApp.Store.Features.Admin.ChangeUserPassword
{
    public class ChangeUserPasswordSuccessAction
    {
        public Version Stamp { get; }

        public ChangeUserPasswordSuccessAction(Version stamp)
        {
            Stamp = stamp;
        }
    }
}