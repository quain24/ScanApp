using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserCommand : IRequest<Result<BasicUserModel>>
    {
        public AddUserDto NewUser { get; }

        public AddUserCommand(AddUserDto newUser)
        {
            NewUser = newUser;
        }
    }

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<BasicUserModel>>
    {
        private readonly IUserManager _userManager;

        public AddUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<BasicUserModel>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var data = request.NewUser;
            return await _userManager.AddNewUser(data.Name, data.Password, data.Email, data.Location, data.Phone).ConfigureAwait(false);
        }
    }
}