using System.Collections.Generic;

namespace ScanApp.Store.Features.Admin.ReadUserNames
{
    public class ReadUserNamesSuccessAction
    {
        public List<string> UserNames { get; }

        public ReadUserNamesSuccessAction(List<string> userNames)
        {
            UserNames = userNames;
        }
    }
}