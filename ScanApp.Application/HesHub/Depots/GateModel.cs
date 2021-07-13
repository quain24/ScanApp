using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Depots
{
    public class GateModel : IEquatable<GateModel>
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public Version Version { get; set; }

        public bool Equals(GateModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Number == other.Number && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GateModel) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Number, Version);
        }
    }
}