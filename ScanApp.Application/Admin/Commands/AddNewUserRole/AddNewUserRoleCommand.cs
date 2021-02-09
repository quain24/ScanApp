using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers.Result;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleCommand : IRequest<Result>
    {
        public string RoleName { get; private set; }

        public AddNewUserRoleCommand(string roleName)
        {
            RoleName = roleName;
        }
    }

    public class AddNewUserRoleHandler : IRequestHandler<AddNewUserRoleCommand, Result>
    {
        private readonly IServiceScopeFactory _factory;

        public AddNewUserRoleHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<Result> Handle(AddNewUserRoleCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var result = await roleManager.CreateAsync(new IdentityRole(request.RoleName));
                return result.Succeeded ? new Result(ResultType.Created) : new Result(ErrorType.NotValid, result.Errors.Select(e => e.Code + " | " + e.Description).ToArray());
            }
        }
    }
}