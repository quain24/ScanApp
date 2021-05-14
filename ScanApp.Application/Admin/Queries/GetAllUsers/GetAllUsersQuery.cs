using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllUsers
{
    /// <summary>
    /// Represents a query used to request all user data (as <see cref="ScanApp.Application.Common.Entities.ApplicationUser"/>)
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    public record GetAllUsersQuery : IRequest<Result<List<ApplicationUser>>>;

    internal class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<ApplicationUser>>>
    {
        private readonly IContextFactory _contextFactory;

        public GetAllUsersQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<ApplicationUser>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                var users = await ctx
                    .Users
                    .AsNoTracking()
                    .OrderBy(u => u.UserName)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                return new Result<List<ApplicationUser>>(ResultType.Ok).SetOutput(users);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<List<ApplicationUser>>(ErrorType.Cancelled, ex);
            }
        }
    }
}