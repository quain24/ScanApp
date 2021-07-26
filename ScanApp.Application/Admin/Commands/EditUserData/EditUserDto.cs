using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDto
    {
        public EditUserDto(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string NewName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Location Location { get; set; }
        public Version Version { get; set; } = Version.Empty;
    }
}