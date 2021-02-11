﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Helpers.Result;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleCommand : IRequest<Result>
    {
        public string RoleName { get; private set; }

        public AddNewUserRoleCommand(string roleName)
        {
            RoleName = roleName;
        }
    }

    public class AddNewUserRoleHandler : IRequestHandler<AddNewUserRoleCommand, Result>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddNewUserRoleHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(AddNewUserRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(request.RoleName)).ConfigureAwait(false);
            return result.Succeeded ? new Result(ResultType.Created) : new Result(ErrorType.NotValid, result.Errors.Select(e => e.Code + " | " + e.Description).ToArray());
        }
    }
}