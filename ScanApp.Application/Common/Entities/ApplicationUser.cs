using Microsoft.AspNetCore.Identity;

namespace ScanApp.Application.Common.Entities
{
    /// <summary>
    /// Represents user entity, used throughout whole application.<br/>
    /// it is implemented even though its empty, so in the future in case of extending user by adding new custom properties no additional work on framework will be needed.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
    }
}