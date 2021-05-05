using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin
{
    /// <summary>
    /// Represents model of a user, typically used in GUI or as a return type when using <see cref="MediatR.Mediator"/>
    /// </summary>
    public class BasicUserModel : IComparable<BasicUserModel>, IEquatable<BasicUserModel>
    {
        /// <summary>
        /// Gets or sets name of user
        /// </summary>
        /// <value>User name if set, otherwise <see langword="null"/></value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Domain.ValueObjects.Version"/> of user
        /// </summary>
        /// <value><seealso cref="Domain.ValueObjects.Version"/> of this user if set, otherwise <see cref="Domain.ValueObjects.Version.Empty()"/></value>
        public Version Version { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="BasicUserModel"/>
        /// </summary>
        /// <remarks>If no <paramref name="version"/> is provided, <seealso cref="Domain.ValueObjects.Version.Empty"/> will be used</remarks>
        /// <param name="name">User name</param>
        /// <param name="version">User <see cref="Version"/></param>
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
            return Equals((BasicUserModel)obj);
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