using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Extensions;
using ScanApp.Application.Common.Helpers.Result;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserCommand : IRequest<Result<string>>
    {
        public AddUserDto NewUser { get; private set; }

        public AddUserCommand(AddUserDto newUser)
        {
            NewUser = newUser;
        }
    }

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AddUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<string>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var newUser = new ApplicationUser()
            {
                Email = request.NewUser.Email,
                Location = request.NewUser.Location,
                PhoneNumber = request.NewUser.Phone,
                UserName = request.NewUser.Name
            };

            var identityResult = await _userManager.CreateAsync(newUser, request.NewUser.Password);
            return identityResult.AsResult(newUser.Id);
        }
    }
}
