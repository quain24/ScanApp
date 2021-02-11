using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllUserRoles
{
    public class GetAllUserRolesQuery : IRequest<Result<List<IdentityRole>>>
    {
    }

    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, Result<List<IdentityRole>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetAllUserRolesQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<List<IdentityRole>>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _roleManager.Roles.ToListAsync(cancellationToken);
            return new Result<List<IdentityRole>>(ResultType.Ok).SetOutput(result);
        }
    }
}