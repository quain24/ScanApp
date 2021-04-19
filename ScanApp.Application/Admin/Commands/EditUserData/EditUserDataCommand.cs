using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public record EditUserDataCommand(string Name, Version Version) : IRequest<Result<Version>>
    {
        public string NewName { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public Location Location { get; init; }
    }

    internal class EditUserDataCommandHandler : IRequestHandler<EditUserDataCommand, Result<Version>>
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
                Version = request.Version,
                NewName = request.NewName,
                Location = request.Location
            };

            return await _userManager.EditUserData(data).ConfigureAwait(false);
        }
    }
}