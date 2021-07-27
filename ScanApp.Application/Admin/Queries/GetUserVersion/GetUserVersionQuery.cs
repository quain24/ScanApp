using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Queries.GetUserVersion
{
    /// <summary>
    /// Represents a query used to request user's with name <paramref name="UserName"/> current <see cref="ScanApp.Domain.ValueObjects.Version()"/>
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user.</param>
    public record GetUserVersionQuery(string UserName) : IRequest<Result<Version>>;

    internal class GetUserVersionQueryHandler : IRequestHandler<GetUserVersionQuery, Result<Version>>
    {
        private readonly IUserInfo _userInfo;

        public GetUserVersionQueryHandler(IUserInfo userInfo)
        {
            _userInfo = userInfo ?? throw new ArgumentNullException(nameof(userInfo));
        }

        public async Task<Result<Version>> Handle(GetUserVersionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userInfo.GetUserConcurrencyStamp(request.UserName, cancellationToken).ConfigureAwait(false);

                return string.IsNullOrEmpty(result)
                    ? new Result<Version>(ErrorType.NotFound).SetOutput(Version.Empty)
                    : new Result<Version>().SetOutput(Version.Create(result));
            }
            catch (OperationCanceledException ex)
            {
                return new Result<Version>(ErrorType.Canceled, ex).SetOutput(Version.Empty);
            }
        }
    }
}