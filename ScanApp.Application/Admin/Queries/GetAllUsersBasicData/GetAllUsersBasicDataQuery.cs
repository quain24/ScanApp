using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public record GetAllUsersBasicDataQuery : IRequest<Result<List<BasicUserModel>>>;

    internal class GetAllUsersBasicDataQueryHandler : IRequestHandler<GetAllUsersBasicDataQuery, Result<List<BasicUserModel>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllUsersBasicDataQueryHandler> _logger;

        public GetAllUsersBasicDataQueryHandler(IApplicationDbContext context, ILogger<GetAllUsersBasicDataQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<BasicUserModel>>> Handle(GetAllUsersBasicDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.Users
                    .AsNoTracking()
                    .Select(u => new BasicUserModel(u.UserName, Version.Create(u.ConcurrencyStamp)))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                return new Result<List<BasicUserModel>>().SetOutput(data);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return new Result<List<BasicUserModel>>(ErrorType.Unknown, "Something went wrong when tried to retrieve basic user data.", ex);
            }
        }
    }
}