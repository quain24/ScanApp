using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Models.Admin
{
    /// <summary>
    /// Represents simplified version of user role, typically used as GUI model
    /// </summary>
    public class RoleModel
    {
        /// <summary>
        /// Gets or sets role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets role <see cref="Domain.ValueObjects.Version"/>
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Get or set role state - is it active
        /// </summary>
        public bool IsActive { get; set; }
    }
}