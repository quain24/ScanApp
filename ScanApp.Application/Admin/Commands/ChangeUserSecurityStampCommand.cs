using MediatR;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ScanApp.Application.Admin.Commands
{
    public class ChangeUserSecurityStampCommand : IRequest<bool>
    {
        public string UserName { get; }

        public ChangeUserSecurityStampCommand(string userName)
        {
            UserName = userName;
        }
    }

    public class ChangeUserSecurityStampCommandHandler : IRequestHandler<ChangeUserSecurityStampCommand, bool>
    {
        private readonly IServiceScopeFactory _factory;

        public ChangeUserSecurityStampCommandHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> Handle(ChangeUserSecurityStampCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var manager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var user = await manager.FindByNameAsync(request.UserName).ConfigureAwait(false);
                var result = await manager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
                return result.Succeeded;
            }
        }
    }
}