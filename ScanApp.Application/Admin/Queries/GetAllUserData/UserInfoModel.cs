using System;

namespace ScanApp.Application.Admin.Queries.GetAllUserData
{
    public class UserInfoModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public DateTimeOffset? LockoutEndDate { get; set; }
    }
}