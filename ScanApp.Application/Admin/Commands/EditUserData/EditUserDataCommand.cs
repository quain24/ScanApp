using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Extensions;
using ScanApp.Application.Common.Helpers.Result;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommand : IRequest<Result>
    {
        public EditUserDto UserData { get; }

        public EditUserDataCommand(EditUserDto userData)
        {
            UserData = userData;
        }
    }

    public class EditUserDataCommandHandler : IRequestHandler<EditUserDataCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EditUserDataCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(EditUserDataCommand request, CancellationToken cancellationToken)
        {
            var data = request.UserData;

            var user = await _userManager.FindByNameAsync(data.Name).ConfigureAwait(false);
            if (user is null)
                return new Result(ErrorType.NotFound);

            if (string.IsNullOrWhiteSpace(data.NewName) is false && data.NewName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) is false)
                user.UserName = data.NewName;
            if (data.Email?.Equals(user.Email, StringComparison.OrdinalIgnoreCase) is false)
                user.Email = data.Email;
            if (data.Location?.Equals(user.Location, StringComparison.OrdinalIgnoreCase) is false)
                user.Location = data.Location;
            if (data.Phone?.Equals(user.PhoneNumber, StringComparison.OrdinalIgnoreCase) is false)
                user.PhoneNumber = data.Phone;

            return (await _userManager.UpdateAsync(user).ConfigureAwait(false)).AsResult();
        }
    }
}