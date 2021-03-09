using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin
{
    public class BasicRoleModel : IComparable<BasicRoleModel>
    {
        public string Name { get; set; }
        public Version Version { get; set; }

        public BasicRoleModel(string roleName, Version version)
        {
            Name = roleName;
            Version = version;
        }

        public int CompareTo(BasicRoleModel other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.CompareOrdinal(Name, other.Name);
        }
    }
}