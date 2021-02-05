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
        public string User { get; }

        public ChangeUserSecurityStampCommand(string userName)
        {
            User = userName;
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
                var user = await manager.FindByNameAsync("test");
                var result = await manager.UpdateSecurityStampAsync(user);
                return result.Succeeded;
            }
        }
    }
}