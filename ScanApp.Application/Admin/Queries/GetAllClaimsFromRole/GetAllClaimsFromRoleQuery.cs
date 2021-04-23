using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllClaimsFromRole
{
    public record GetAllClaimsFromRoleQuery(string RoleName) : IRequest<Result<List<ClaimModel>>>;

    internal class GetAllClaimsFromRoleQueryHandler : IRequestHandler<GetAllClaimsFromRoleQuery, Result<List<ClaimModel>>>
    {
        private readonly IRoleManager _roleManager;

        public GetAllClaimsFromRoleQueryHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public Task<Result<List<ClaimModel>>> Handle(GetAllClaimsFromRoleQuery request, CancellationToken cancellationToken)
        {
            return _roleManager.GetAllClaimsFromRole(request.RoleName);
        }
    }
}