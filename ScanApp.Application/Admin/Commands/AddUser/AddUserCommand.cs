using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    /// <summary>
    /// Represents a command used to request for adding a new user to application's data source
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="NewUser">Data of new user to be processed.</param>
    public record AddUserCommand(AddUserDto NewUser) : IRequest<Result<BasicUserModel>>;

    internal class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<BasicUserModel>>
    {
        private readonly IUserManager _userManager;

        public AddUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<Result<BasicUserModel>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var data = request.NewUser;
            return await _userManager.AddNewUser(data.Name, data.Password, data.Email, data.Phone, data.Location).ConfigureAwait(false);
        }
    }
}