using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Queries.GetAllUsersBasicData
{
    /// <summary>
    /// Represents a query used to request all of basic user data (as <see cref="ScanApp.Application.Admin.BasicUserModel"/>)
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    public record GetAllUsersBasicDataQuery : IRequest<Result<List<BasicUserModel>>>;

    internal class GetAllUsersBasicDataQueryHandler : IRequestHandler<GetAllUsersBasicDataQuery, Result<List<BasicUserModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public GetAllUsersBasicDataQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<BasicUserModel>>> Handle(GetAllUsersBasicDataQuery request, CancellationToken cancellationToken)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            var data = await ctx.Users
                .AsNoTracking()
                .Select(u => new BasicUserModel(u.UserName, Version.Create(u.ConcurrencyStamp)))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new Result<List<BasicUserModel>>().SetOutput(data);
        }
    }
}