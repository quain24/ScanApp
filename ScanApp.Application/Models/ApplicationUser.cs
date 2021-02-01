using Microsoft.AspNetCore.Identity;

namespace ScanApp.Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Location { get; set; }
    }
}