using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Queries.GetUserRoles
{
    public record GetUserRolesQuery(string UserName, Version Version) : IRequest<Result<List<BasicRoleModel>>>;

    internal class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<List<BasicRoleModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<BasicRoleModel>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            try
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
            catch (OperationCanceledException ex)
            {
                return new Result<List<BasicRoleModel>>(ErrorType.Cancelled, ex);
            }
        }
    }
}