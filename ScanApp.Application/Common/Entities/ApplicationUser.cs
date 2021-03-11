using Microsoft.AspNetCore.Identity;

namespace ScanApp.Application.Common.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int LocationId { get; set; }
    }
}