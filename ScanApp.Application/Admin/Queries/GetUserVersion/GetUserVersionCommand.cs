using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetUserVersion
{
    public class GetUserVersionCommand : IRequest<Result<Version>>
    {
        public string UserName { get; }

        public GetUserVersionCommand(string userName)
        {
            UserName = userName;
        }
    }

    public class GetUserVersionCommandHandler : IRequestHandler<GetUserVersionCommand, Result<Version>>
    {
        private readonly IUserInfo _userInfo;

        public GetUserVersionCommandHandler(IUserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public async Task<Result<Version>> Handle(GetUserVersionCommand request, CancellationToken cancellationToken)
        {
            var result = await _userInfo.GetUserConcurrencyStamp(request.UserName).ConfigureAwait(false);

            return string.IsNullOrEmpty(result)
                ? new Result<Version>(ErrorType.NotFound).SetOutput(Version.Empty())
                : new Result<Version>().SetOutput(Version.Create(result));
        }
    }
}