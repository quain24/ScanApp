using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin
{
    public class BasicUserModel : IComparable<BasicUserModel>, IEquatable<BasicUserModel>
    {
        public string Name { get; set; }
        public Version Version { get; set; }

        public BasicUserModel(string name, Version version)
        {
            Name = name;
            Version = version ?? Version.Empty();
        }

        public int CompareTo(BasicUserModel other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(BasicUserModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BasicUserModel) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Version);
        }

        public static bool operator ==(BasicUserModel left, BasicUserModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BasicUserModel left, BasicUserModel right)
        {
            return !Equals(left, right);
        }
    }
}