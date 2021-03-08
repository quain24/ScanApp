using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Queries.GetAllUsersBasicData
{
    public class BasicUserModel
    {
        public string UserName { get; set; }
        public Version Version { get; set; }

        public BasicUserModel(string userName, Version version)
        {
            UserName = userName;
            Version = version;
        }
    }
}