using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Queries.GetUserRoles
{
    /// <summary>
    /// Represents a query used to request user's with name <paramref name="UserName"/> basic roles informations
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version()"/> to be compared to a concurrency stamp in data source.</param>
    public record GetUserRolesQuery(string UserName, Version Version) : IRequest<Result<List<BasicRoleModel>>>;

    internal class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<List<BasicRoleModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public GetUserRolesQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<BasicRoleModel>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                var user = await ctx.Users
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

                var roles = await ctx.UserRoles
                    .AsNoTracking()
                    .Where(u => u.UserId.Equals(user.Id))
                    .Join(ctx.Roles, role => role.RoleId, identityRole => identityRole.Id,
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