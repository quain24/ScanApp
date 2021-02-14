using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Application.Common.Extensions;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommand : IRequest<Result>
    {
        public string RoleId { get; }
        public string NewClaimName { get; }
        public string ClaimValue { get; }

        public AddClaimToRoleCommand(string roleId, string newClaimName, string claimValue)
        {
            RoleId = roleId;
            NewClaimName = newClaimName;
            ClaimValue = claimValue;
        }
    }

    public class AddClaimToRoleCommandHandler : IRequestHandler<AddClaimToRoleCommand, Result>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IApplicationDbContext _context;

        public AddClaimToRoleCommandHandler(RoleManager<IdentityRole> roleManager, IApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<Result> Handle(AddClaimToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId).ConfigureAwait(false);

            if (role is null)
            {
                return new Result(ErrorType.NotFound, "Role not found!");
            }

            if (await _context.RoleClaims
                .Where(r => r.RoleId == role.Id &&
                            string.Equals(r.ClaimType, request.NewClaimName) &&
                            string.Equals(r.ClaimValue, request.ClaimValue))
                .FirstOrDefaultAsync(cancellationToken) is not null)
            {
                return new Result(ErrorType.Duplicated, $"Role {role.Name} already have claim {request.NewClaimName} with value {request.ClaimValue}");
            }

            var claim = new IdentityRoleClaim<string>()
            {
                ClaimType = request.NewClaimName,
                RoleId = role.Id,
                ClaimValue = request.ClaimValue
            };

            var result = await _roleManager.AddClaimAsync(role, claim.ToClaim()).ConfigureAwait(false);

            if (result.IsConcurrencyFailure())
            {
                return new Result(ErrorType.ConcurrencyFailure, result.CombineErrors());
            }

            return result.Succeeded
                ? new Result(ResultType.Created)
                : new Result(ErrorType.NotValid, result.CombineErrors());
        }
    }
}