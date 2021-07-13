using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Depots
{
    public class TrailerTypeModel : IEquatable<TrailerTypeModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Version Version { get; set; }

        public bool Equals(TrailerTypeModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TrailerTypeModel)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Id);
            hashCode.Add(Name, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(Version);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(TrailerTypeModel left, TrailerTypeModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TrailerTypeModel left, TrailerTypeModel right)
        {
            return !Equals(left, right);
        }
    }
}