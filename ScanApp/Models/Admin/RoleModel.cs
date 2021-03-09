using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Models.Admin
{
    public class RoleModel
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public bool IsActive { get; set; }
    }
}