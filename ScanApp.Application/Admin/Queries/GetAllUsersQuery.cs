using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ScanApp.Application.Admin.Queries
{
    public class GetAllUsersQuery : IRequest<IList<ApplicationUser>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IList<ApplicationUser>>
    {
        private readonly IServiceScopeFactory _factory;

        public GetAllUsersQueryHandler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task<IList<ApplicationUser>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            using (var scope = _factory.CreateScope())
            {
                var manager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var user = await manager.Users.ToListAsync(cancellationToken).ConfigureAwait(false);
                return user;
            }
        }
    }
}