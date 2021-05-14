using ScanApp.Domain.Entities;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Queries.GetAllUserData
{
    /// <summary>
    /// Represents collection of user data, typically returned by one of the queries.
    /// </summary>
    public sealed class UserInfoModel
    {
        /// <summary>
        /// Creates new instance of <see cref="UserInfoModel"/>.
        /// </summary>
        public UserInfoModel()
        {
            Version = Version.Empty();
        }

        /// <summary>
        /// Creates new deep copy instance of <see cref="UserInfoModel"/> from provided <paramref name="userInfo"/>.
        /// </summary>
        /// <param name="userInfo">Instance from which copy will be created.</param>
        public UserInfoModel(UserInfoModel userInfo)
        {
            _ = userInfo ?? throw new ArgumentNullException(nameof(userInfo));

            Name = userInfo.Name;
            LockoutEnd = userInfo.LockoutEnd;
            Email = userInfo.Email;
            Phone = userInfo.Phone;
            Location = userInfo.Location;
            Version = userInfo.Version;
        }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        /// <value>Name of user if set, otherwise <see langword="null"/>.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets user email address.
        /// </summary>
        /// <value>Email address of user if set, otherwise <see langword="null"/>.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets user phone number.
        /// </summary>
        /// <value>Phone number of user if set, otherwise <see langword="null"/>.</value>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets user <see cref="ScanApp.Domain.Entities.Location"/>.
        /// </summary>
        /// <value>User <see cref="ScanApp.Domain.Entities.Location"/> if set, otherwise <see langword="null"/>.</value>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets user <see cref="ScanApp.Domain.ValueObjects.Version()"/>.
        /// </summary>
        /// <value>User <see cref="ScanApp.Domain.ValueObjects.Version()"/> if set.
        /// In default implementation, until not set otherwise, this property returns <see cref="Domain.ValueObjects.Version.Empty"/>.</value>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets users lockout end date.
        /// </summary>
        /// <value>End date of users lockout.</value>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets users lockout end date.
        /// </summary>
        /// <value>End date of users lockout.</value>
        public DateTime? LockoutEndDate() => LockoutEnd?.LocalDateTime;

        /// <summary>
        /// Compares 2 <see cref="UserInfoModel"/> instances by all properties including <see cref="LockoutEnd"/>.
        /// </summary>
        /// <remarks>This comparison does not take instance reference under consideration.</remarks>
        /// <param name="other">Instance of <see cref="UserInfoModel"/> to which this instance will be compared to.</param>
        /// <returns><see langword="True"/> if instances are equal; Otherwise <see langword="False"/>.</returns>
        public bool EqualWithDate(UserInfoModel other)
        {
            return EqualWithoutDate(other) && LockoutEnd == other.LockoutEnd;
        }

        /// <summary>
        /// Compares 2 <see cref="UserInfoModel"/> instances by all properties <strong>excluding</strong> <see cref="LockoutEnd"/>.
        /// </summary>
        /// <remarks>This comparison does not take instance reference under consideration.</remarks>
        /// <param name="other">Instance of <see cref="UserInfoModel"/> to which this instance will be compared to.</param>
        /// <returns><see langword="True"/> if instances are equal; Otherwise <see langword="False"/>.</returns>
        public bool EqualWithoutDate(UserInfoModel other)
        {
            if (other is null)
                return false;

            return string.Equals(Name, other.Name) &&
                   string.Equals(Email, other.Email) &&
                   string.Equals(Phone, other.Phone) &&
                   Equals(Location?.Id, other.Location?.Id) &&
                   Version == other.Version;
        }
    }
}