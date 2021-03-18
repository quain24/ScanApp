﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetUserRoles
{
    public class GetUserRolesQuery : IRequest<Result<List<BasicRoleModel>>>
    {
        public GetUserRolesQuery(string userName, Version version)
        {
            UserName = userName;
            Version = version;
        }

        public string UserName { get; }
        public Version Version { get; }
    }

    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<List<BasicRoleModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<BasicRoleModel>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserName.Equals(request.UserName))
                .Select(u => new
                {
                    u.Id,
                    Name = u.UserName,
                    Version = Version.Create(u.ConcurrencyStamp)
                })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
                return new Result<List<BasicRoleModel>>(ErrorType.NotFound, $"No user with name \"{request.UserName}\" exists.");
            if (user.Version != request.Version)
                return new Result<List<BasicRoleModel>>(ErrorType.ConcurrencyFailure);

            var roles = await _context.UserRoles
                .AsNoTracking()
                .Where(u => u.UserId.Equals(user.Id))
                .Join(_context.Roles, role => role.RoleId, identityRole => identityRole.Id,
                    (_, identityRole) => new BasicRoleModel(identityRole.Name, Version.Create(identityRole.ConcurrencyStamp)))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            roles.Sort();
            return new Result<List<BasicRoleModel>>(ResultType.Ok).SetOutput(roles);
        }
    }
}