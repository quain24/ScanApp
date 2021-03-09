using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin
{
    public class BasicUserModel : IComparable<BasicUserModel>
    {
        public string Name { get; set; }
        public Version Version { get; set; }

        public BasicUserModel(string name, Version version)
        {
            Name = name;
            Version = version;
        }

        public int CompareTo(BasicUserModel other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}