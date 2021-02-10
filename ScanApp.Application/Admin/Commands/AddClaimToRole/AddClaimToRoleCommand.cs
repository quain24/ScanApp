using System.Linq;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers.Result;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommand : IRequest<Result>
    {
        public string RoleId { get; }
        public string NewClaimName { get; }

        public AddClaimToRoleCommand(string roleId, string newClaimName)
        {
            RoleId = roleId;
            NewClaimName = newClaimName;
        }
    }

    public class AddClaimToRoleCommandHandler : IRequestHandler<AddClaimToRoleCommand, Result>
    {
        private readonly IServiceScopeFactory _factory;

        public AddClaimToRoleCommandHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<Result> Handle(AddClaimToRoleCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                var role = await roleManager.FindByIdAsync(request.RoleId);

                if (role is null)
                {
                    return new Result(ErrorType.NotFound, "Role not found!");
                }

                var claim = new IdentityRoleClaim<string>()
                {
                    ClaimType = request.NewClaimName,
                    RoleId = role.Id,
                    ClaimValue = "CanAdd"
                };

                var result = await roleManager.AddClaimAsync(role, claim.ToClaim());
                return result.Succeeded
                    ? new Result(ResultType.Created)
                    : new Result(ErrorType.NotValid, result.Errors.Select(e => $"{e.Code} | {e.Description}"));
            }
        }
    }
}