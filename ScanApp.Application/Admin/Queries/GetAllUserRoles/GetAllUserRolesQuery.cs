using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers;
using ScanApp.Application.Common.Helpers.Result;

namespace ScanApp.Application.Admin.Queries.GetAllUserRoles
{
    public class GetAllUserRolesQuery : IRequest<Result<List<IdentityRole>>>
    {
    }

    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, Result<List<IdentityRole>>>
    {
        private readonly IServiceScopeFactory _factory;

        public GetAllUserRolesQueryHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<Result<List<IdentityRole>>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                var result = await roleManager.Roles.ToListAsync(cancellationToken);
                return new Result<List<IdentityRole>>(ResultType.Ok).SetOutput(result);
            }
        }
    }
}