using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Application.Common.Entities;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommand : IRequest<Result<Version>>
    {
        public string Name { get; }
        public string NewName { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public int LocationId { get; init; }
        public Version ConcurrencyStamp { get; init; }

        public EditUserDataCommand(string name)
        {
            Name = name;
        }
    }

    public class EditUserDataCommandHandler : IRequestHandler<EditUserDataCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public EditUserDataCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<Version>> Handle(EditUserDataCommand request, CancellationToken cancellationToken)
        {
            var data = new EditUserDto(request.Name)
            {
                Phone = request.Phone,
                Email = request.Email,
                LocationId = request.LocationId,
                Version = request.ConcurrencyStamp,
                NewName = request.NewName
            };

            return await _userManager.EditUserData(data).ConfigureAwait(false);
        }
    }
}