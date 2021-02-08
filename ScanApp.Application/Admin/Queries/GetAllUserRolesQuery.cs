using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries
{
    public class GetAllUserRolesQuery : IRequest<List<IdentityRole>>
    {
    }

    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, List<IdentityRole>>
    {
        private readonly IServiceScopeFactory _factory;

        public GetAllUserRolesQueryHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<IdentityRole>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                var result = await roleManager.Roles.ToListAsync(cancellationToken);
                return result;
            }
        }
    }
}