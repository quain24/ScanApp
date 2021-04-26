using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Queries.GetUserVersion
{
    public record GetUserVersionCommand(string UserName) : IRequest<Result<Version>>;

    internal class GetUserVersionCommandHandler : IRequestHandler<GetUserVersionCommand, Result<Version>>
    {
        private readonly IUserInfo _userInfo;

        public GetUserVersionCommandHandler(IUserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public async Task<Result<Version>> Handle(GetUserVersionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userInfo.GetUserConcurrencyStamp(request.UserName, cancellationToken).ConfigureAwait(false);

                return string.IsNullOrEmpty(result)
                    ? new Result<Version>(ErrorType.NotFound).SetOutput(Version.Empty())
                    : new Result<Version>().SetOutput(Version.Create(result));
            }
            catch (OperationCanceledException ex)
            {
                return new Result<Version>(ErrorType.Cancelled, ex).SetOutput(Version.Empty());
            }
        }
    }
}