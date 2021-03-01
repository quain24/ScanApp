using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommand : IRequest<Result<ConcurrencyStamp>>
    {
        public EditUserDto UserData { get; }

        public EditUserDataCommand(EditUserDto userData)
        {
            UserData = userData;
        }
    }

    public class EditUserDataCommandHandler : IRequestHandler<EditUserDataCommand, Result<ConcurrencyStamp>>
    {
        private readonly IUserManager _userManager;

        public EditUserDataCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<ConcurrencyStamp>> Handle(EditUserDataCommand request, CancellationToken cancellationToken)
        {
            return await _userManager.EditUserData(request.UserData).ConfigureAwait(false);
        }
    }
}