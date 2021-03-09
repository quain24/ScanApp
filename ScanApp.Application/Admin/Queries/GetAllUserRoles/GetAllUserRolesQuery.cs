using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllUserRoles
{
    public class GetAllUserRolesQuery : IRequest<Result<List<BasicRoleModel>>>
    {
    }

    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, Result<List<BasicRoleModel>>>
    {
        private readonly IRoleManager _roleManager;

        public GetAllUserRolesQueryHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<Result<List<BasicRoleModel>>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            return _roleManager.GetAllRoles();
        }
    }
}