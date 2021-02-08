using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    public class ChangeUserSecurityStampCommand : IRequest<Result>
    {
        public string UserName { get; }

        public ChangeUserSecurityStampCommand(string userName)
        {
            UserName = userName;
        }
    }

    public class ChangeUserSecurityStampCommandHandler : IRequestHandler<ChangeUserSecurityStampCommand, Result>
    {
        private readonly IServiceScopeFactory _factory;

        public ChangeUserSecurityStampCommandHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<Result> Handle(ChangeUserSecurityStampCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var manager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var user = await manager.FindByNameAsync(request.UserName).ConfigureAwait(false);
                var result = await manager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
                return result.Succeeded ? new Result(ResultType.Ok) : new Result(ErrorType.NotValid, result.Errors.Select(e => e.Code + " | " + e.Description).ToArray());
            }
        }
    }
}