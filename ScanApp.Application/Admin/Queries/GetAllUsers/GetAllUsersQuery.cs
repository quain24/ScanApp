using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers;

namespace ScanApp.Application.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<Result<List<ApplicationUser>>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<ApplicationUser>>>
    {
        private readonly IServiceScopeFactory _factory;

        public GetAllUsersQueryHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<Result<List<ApplicationUser>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var manager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var users = await manager.Users.ToListAsync(cancellationToken).ConfigureAwait(false);
                return new Result<List<ApplicationUser>>(ResultType.Ok).SetOutput(users);
            }
        }
    }
}