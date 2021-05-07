using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin
{
    /// <summary>
    /// Represents simplified version of user role, typically used as <see cref="MediatR.Mediator"/> return or send model
    /// </summary>
    public class BasicRoleModel : IComparable<BasicRoleModel>
    {
        /// <summary>
        /// Gets name of the role
        /// </summary>
        /// <value><see cref="String"/> containing role name if any; Otherwise <see langword="null"/></value>
        public string Name { get; }

        /// <summary>
        /// Gets <see cref="ScanApp.Domain.ValueObjects.Version"/> of the role
        /// </summary>
        /// <value>Roles <see cref="ScanApp.Domain.ValueObjects.Version"/> if set; Otherwise <see cref="Domain.ValueObjects.Version.Empty"/></value>
        public Version Version { get; }

        /// <summary>
        /// Creates new instance of <see cref="BasicRoleModel"/>
        /// </summary>
        /// <remarks>If no <paramref name="version"/> is provided, <seealso cref="Domain.ValueObjects.Version.Empty"/> will be used</remarks>
        /// <param name="roleName">Name of role</param>
        /// <param name="version">Version of role</param>
        public BasicRoleModel(string roleName, Version version)
        {
            Name = roleName;
            Version = version ?? Version.Empty();
        }

        public int CompareTo(BasicRoleModel other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.CompareOrdinal(Name, other.Name);
        }
    }
}