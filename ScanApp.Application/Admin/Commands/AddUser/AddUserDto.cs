using ScanApp.Domain.Entities;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Location Location { get; set; }
    }
}