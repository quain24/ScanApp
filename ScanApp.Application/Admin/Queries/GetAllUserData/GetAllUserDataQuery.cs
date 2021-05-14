using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllUserData
{
    /// <summary>
    /// Represents a query used to request data of user with given <paramref name="UserName"/>
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">User name.</param>
    public record GetAllUserDataQuery(string UserName) : IRequest<Result<UserInfoModel>>;

    internal class GetAllUserDataQueryHandler : IRequestHandler<GetAllUserDataQuery, Result<UserInfoModel>>
    {
        private readonly IUserInfo _userInfo;

        public GetAllUserDataQueryHandler(IUserInfo userInfo)
        {
            _userInfo = userInfo ?? throw new ArgumentNullException(nameof(userInfo));
        }

        public async Task<Result<UserInfoModel>> Handle(GetAllUserDataQuery request, CancellationToken cancellationToken)
        {
            var info = await _userInfo.GetData(request.UserName).ConfigureAwait(false);
            return info is null
                ? new Result<UserInfoModel>(ErrorType.NotFound, $"User \"{request.UserName}\" was not found")
                : new Result<UserInfoModel>(ResultType.Ok, info);
        }
    }
}