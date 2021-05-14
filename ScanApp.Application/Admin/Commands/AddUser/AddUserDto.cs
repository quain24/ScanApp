using ScanApp.Domain.Entities;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    /// <summary>
    /// Represents set of data used to create a new user in database.
    /// </summary>
    public class AddUserDto
    {
        /// <summary>
        /// Gets or sets users name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets new users password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets new users phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets new users email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets new users <see cref="Domain.Entities.Location"/>.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets value indicating if new user can be locked out if admin policy says so.
        /// </summary>
        public bool CanBeLockedOut { get; set; } = true;
    }
}