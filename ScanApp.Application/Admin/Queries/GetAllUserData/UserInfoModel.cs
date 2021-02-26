using ScanApp.Common.Extensions;
using System;

namespace ScanApp.Application.Admin.Queries.GetAllUserData
{
    public sealed class UserInfoModel
    {
        public UserInfoModel()
        {
        }

        public UserInfoModel(UserInfoModel userInfo)
        {
            _ = userInfo ?? throw new ArgumentNullException(nameof(userInfo));

            Name = userInfo.Name;
            LockoutEnd = userInfo.LockoutEnd;
            Email = userInfo.Email;
            Location = userInfo.Location;
            Phone = userInfo.Phone;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        public DateTime? LockoutEndDate() => DateTimeExtensions.ConvertFromDateTimeOffset(LockoutEnd);

        public TimeSpan? LockoutEndTime()
        {
            if (!LockoutEnd.HasValue)
                return null;
            var time = LockoutEndDate().Value;
            return new TimeSpan(time.Hour, time.Minute, time.Second);
        }

        public bool EqualWithDate(UserInfoModel other)
        {
            return Equals(other) && LockoutEnd == other.LockoutEnd;
        }

        public bool EqualWithoutDate(UserInfoModel other)
        {
            if (other is null)
                return false;

            return Name == other.Name &&
                   Email == other.Email &&
                   Location == other.Location &&
                   Phone == other.Phone;
        }
    }
}